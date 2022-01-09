using Godot;
using System;
using System.Collections.Generic;

public class World : Node2D
{
	private static string _fireballPath;

	private Player _player;

	private string resourceName;
	private string nextLevelResource;

	public Player player
	{
		get
		{
			if(_player == null)
			{
				_player = (Player)GetNode("Player");
			}
			return _player;
		}
	}
	[Export]
    public string ResourceName { get => resourceName; set => resourceName = value; }
	[Export]
    public string NextLevelResource { get => nextLevelResource; set => nextLevelResource = value; }

    public bool compareFloatsWithError(float x, float y, float error = 0.1f)
	{
		return (Math.Abs(x - y) < error);
	}

	public void spawnProjectile(Vector2 position, float distance, float speed, KinematicBody2D target)
	{
		//GD.Print("spawning projectile with target ", target);
		try
		{
			PackedScene buffer = (PackedScene)ResourceLoader.Load("res://Scenes/Fireball.tscn");
			Projectile projectile = (Projectile)buffer.Instance();
			//GD.Print(projectile);
			projectile.Position = position;
			projectile.distance = distance;
			projectile.speed = speed;
			projectile.target = target;
			AddChild(projectile);
		}
		catch(Exception e)
		{
			GD.Print(e);
		}		
	}

	public void SaveGame()
	{
		var saveGame = new File();
		saveGame.Open("user://savegame.save", File.ModeFlags.Write);

		var saveNodes = GetTree().GetNodesInGroup("Persist");
		foreach (Node saveNode in saveNodes)
		{
			// Check the node is an instanced scene so it can be instanced again during load.
			if (saveNode.Filename.Empty())
			{
				GD.Print(String.Format("persistent node '{0}' is not an instanced scene, skipped", saveNode.Name));
				continue;
			}

			// Check the node has a save function.
			if (!saveNode.HasMethod("Save"))
			{
				GD.Print(String.Format("persistent node '{0}' is missing a Save() function, skipped", saveNode.Name));
				continue;
			}

			// Call the node's save function.
			var nodeData = saveNode.Call("Save");

			// Store the save dictionary as a new line in the save file.
			saveGame.StoreLine(JSON.Print(nodeData));
		}

		saveGame.Close();
	}
	public void LoadGame()
	{
		var saveGame = new File();
		if (!saveGame.FileExists("user://savegame.save"))
			return; // Error! We don't have a save to load.

		// We need to revert the game state so we're not cloning objects during loading.
		// This will vary wildly depending on the needs of a project, so take care with
		// this step.
		// For our example, we will accomplish this by deleting saveable objects.
		var saveNodes = GetTree().GetNodesInGroup("Persist");
		foreach (Node saveNode in saveNodes)
			saveNode.QueueFree();

		// Load the file line by line and process that dictionary to restore the object
		// it represents.
		saveGame.Open("user://savegame.save", File.ModeFlags.Read);

		while (saveGame.GetPosition() < saveGame.GetLen())
		{
			// Get the saved dictionary from the next line in the save file
			var nodeData = 
				new Godot.Collections.Dictionary<string, object>(
					(Godot.Collections.Dictionary)JSON.Parse(saveGame.GetLine()).Result);

			// Firstly, we need to create the object and add it to the tree and set its position.
			var newObjectScene = (PackedScene)ResourceLoader.Load(
				nodeData["Filename"].ToString());
			var newObject = (Node)newObjectScene.Instance();
			GetNode(nodeData["Parent"].ToString()).AddChild(newObject);
			newObject.Set("Position", new Vector2(
				(float)nodeData["PosX"], (float)nodeData["PosY"]));

			// Now we set the remaining variables.
			foreach (KeyValuePair<string, object> entry in nodeData)
			{
				string key = entry.Key.ToString();
				if (key == "Filename" || key == "Parent" || key == "PosX" || key == "PosY")
					continue;
				newObject.Set(key, entry.Value);
			}
		}

		saveGame.Close();
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        try
        {
			//LoadGame();
        }
		catch (Exception ex)
        {
			GD.Print(ex);
        }
		_player = (Player)GetNode("Player");
	}
}

using Godot;
using System;

public class LevelEnd : Area2D
{
    private World _world;
    public World world
	{
		get
		{
			if(_world == null)
			{
				_world = (World)GetTree().Root.GetNode("Node2D");
			}
			return _world;
		}
	}
    public void completeLevel(KinematicBody2D body)
    {
        if(body == world.player)
        {
            try
            {
                GetTree().Paused = true;
                PackedScene buffer = (PackedScene)ResourceLoader.Load("res://Scenes/LevelCompleteHUD.tscn");
                LevelCompleteHUD HUD = (LevelCompleteHUD)buffer.Instance();
                HUD.setScoreText(world.player.gameHUD.score);
                //world.player.gameHUD.Visible = false;
                world.AddChild(HUD);
                world.RemoveChild(world.player.gameHUD);
                Timer timer = (Timer)GetNode("Timer");
                timer.Start();
            }
            catch(Exception e)
            {
                GD.Print(e);
            }
        }
    }

    public void resetAll()
    {
        GetTree().ChangeScene("res://Scenes/MainMenu.tscn");
    }

//     // Called when the node enters the scene tree for the first time.
//     public override void _Ready()
//     {
        
//     }

// //  // Called every frame. 'delta' is the elapsed time since the previous frame.
// //  public override void _Process(float delta)
// //  {
// //      
// //  }
}

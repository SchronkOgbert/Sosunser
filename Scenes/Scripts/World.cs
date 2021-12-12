using Godot;
using System;

public class World : Node2D
{
	private static string _fireballPath;

	private Player _player;

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

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_player = (Player)GetNode("Player");
	}
}

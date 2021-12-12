using Godot;
using System;

public class Projectile : KinematicBody2D
{
	private float _distance;
	private float _speed;
	private KinematicBody2D _target;
	private Vector2 _velocity;
	private World _world;
	private float _destination;

	[Export]
	public float distance
	{
		get { return distance; }
		set { _distance = value; }
	}
	[Export]
	public float speed
	{
		get { return _speed; }
		set 
		{ 
			_speed = value; 
			velocity = new Vector2(_speed, 0);
		}
	}
	public Vector2 velocity
	{
		 get { return _velocity; }
		 set { _velocity = value; }
	}

	public KinematicBody2D target
	{
		get {return _target; }
		set { _target = value; }
	}

	private void handleMovement()
	{
		if(_world.compareFloatsWithError(this.Position.x, ))
		MoveAndSlide(_velocity);
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//GD.Print("projectile spawned at ", Position);
		_world = (World)GetTree().Root.GetNode("Node2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		//GD.Print("projectile at ", Position);
	}
}

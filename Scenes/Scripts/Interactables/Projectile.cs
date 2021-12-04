using Godot;
using System;

public class Projectile : Area2D
{
	private float _distance;
	private float _speed;
	private KinematicBody2D _target;

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
		set { _speed = value; }
	}

	public KinematicBody2D target
	{
		get {return _target; }
		set { _target = value; }
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		
	}
}

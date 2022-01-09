using Godot;
using System;

public class Projectile : Area2D
{
	private float _distance;
	private float _speed;
	private KinematicBody2D _target;
	private Vector2 _velocity;
	private World _world;
	private float _destination;
	private CollisionShape2D _collision;

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
			velocity = new Vector2(_speed * _distance, 0);
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

	private void handleMovement(float delta)
	{
		if(_world.compareFloatsWithError(this.Position.x, _destination, 5))
		{
			QueueFree();
			return;
		}
		MoveLocalX(_distance * _speed * delta);
	}

	public void _on_Fireball_body_entered(KinematicBody2D body)
	{
		//GD.Print("trying to send damage to player");
		//GD.Print(body, "->", _target);
		if(body == _target)
		{
			//GD.Print("sent damage to player");
			body.Call("takeDamage", 1);
		}
		QueueFree();
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//GD.Print("projectile spawned at ", Position);
		_world = (World)GetTree().Root.GetNode("Node2D");
		_collision = (CollisionShape2D)GetNode("CollisionShape2D");
		_destination = Position.x + _distance;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		handleMovement(delta);
		//GD.Print("projectile at ", Position);
	}
}

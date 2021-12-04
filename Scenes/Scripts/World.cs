using Godot;
using System;

public class World : Node2D
{

	public void spawnProjectile(Vector2 position, float distance, float speed, KinematicBody2D target)
	{
		Projectile projectile = GD.Load<Projectile>("res://Scenes/Player.tscn");
		projectile.Position = position;
		projectile.distance = distance;
		projectile.speed = speed;
		AddChild(projectile);
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		   
	}
}

using Godot;
using System;

public class Background : Sprite
{
	private Timer bounceTimer;
	private bool up = true;
	private Vector2 downPosition;
	private Vector2 defaultPosition;
	private Camera2D camera;
	private Vector2 cameraPos;
	// Called when the node enters the scene tree for the first time.
	public override void _Draw()
	{
		bounceTimer = (Timer)GetNode("CloudMovementTimer");
		defaultPosition = Position;
		downPosition = new Vector2(defaultPosition.x, defaultPosition.y + 16);
	}

	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		if(bounceTimer.TimeLeft == 0)
		{
			//GD.Print("moving clouds down");
			if(up)
			{
				this.Position = downPosition;
				bounceTimer.Start();
				up = false;
			}
			else
			{
				this.Position = defaultPosition;
				up = true;
				bounceTimer.Start();
			}
		}
	}
}

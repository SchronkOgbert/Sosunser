using Godot;
using System;

public class Platform : KinematicBody2D
{
    [Export]
    public Vector2 distance;
    [Export]
    public bool looping;
    [Export]
    public float movementSpeed;
    [Export]
    public bool changeCamera;
    [Export]
    public string cameraName;

    private Timer timer;

    private static int counter;

    private void resetCamera()
    {
        GD.Print("resetting camera");
        Camera2D camera = (Camera2D)GetTree().CurrentScene.GetNode("Player").GetNode("Camera2D");
        camera.Current = true;
    }

    private void movePlatform()
    {
        if(movementSpeed == 0)
        {
            Position = new Vector2(Position.x + distance.x,
            Position.y + distance.y);
        }
        else
        {
            GD.Print("moving");
            MoveAndSlide(distance / movementSpeed);
        }
    }

    public void activated()
    {
        GD.Print("platform activated");        
        if(changeCamera)
        {
            Camera2D camera = (Camera2D)GetNode(cameraName);
            camera.Current = true;
            timer.WaitTime = ((float)Math.Sqrt((Math.Pow(distance.x, 2) +
            Math.Pow(distance.y, 2))) + 0.2f) / 60;
            GD.Print(timer.WaitTime);
            timer.Start();
        }
        movePlatform();
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print(distance);
        timer = (Timer)GetNode("Timer");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if(movementSpeed > 0)
        {
            if(timer.TimeLeft < timer.WaitTime - 0.1f && 
            timer.TimeLeft >= 0.1f)
            {
                movePlatform();
                counter++;
                //GD.Print(distance / movementSpeed);
            }
        }
        //GD.Print(counter);
    }
}

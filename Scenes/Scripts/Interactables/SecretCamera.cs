using Godot;
using System;

public class SecretCamera : Area2D
{
    [Export]
    public int upperLimit, bottomLimit, leftLimit, rightLimit;
    public void activateSecret(KinematicBody2D body)
    {
        //GD.Print("Secret activated by: ", body);
        try
        {
            Camera2D camera = (Camera2D)body.GetNode("Camera2D");
            camera.LimitTop +=upperLimit;
            camera.LimitRight += rightLimit;
            camera.LimitBottom += bottomLimit;
            camera.LimitLeft += leftLimit;
        }
        catch(Exception e)
        {
            GD.Print(e);
        }
    }

    public void deactivateSecret(KinematicBody2D body)
    {
        Camera2D camera;
        if(body == null)
        {
            ((World)GetTree().Root.GetNode("Node2D")).player.GetNode("Camera2D");
        }
        //GD.Print("Secret activated by: ", body);
        try
        {
            camera = (Camera2D)body.GetNode("Camera2D");
            camera.LimitTop = -16;
            camera.LimitRight -= rightLimit;
            camera.LimitBottom -= bottomLimit;
            camera.LimitLeft -= leftLimit;
        }
        catch(Exception e)
        {
            GD.Print("When deactivating:\n", e);
            
        }
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

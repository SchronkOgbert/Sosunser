using Godot;
using System;

public class FallZone : Area2D
{

    public void _on_FallZone_body_entered(KinematicBody player)
    {
        GetTree().ChangeScene("res://Scenes/Level1.tscn");
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

}

using Godot;
using System;

public class FadePlatform : KinematicBody2D
{
    public void activate()
    {
        AnimationPlayer player = (AnimationPlayer)GetNode("AnimationPlayer");
        player.Play("fade_in");
    }

    // // Called when the node enters the scene tree for the first time.
    // public override void _Ready()
    // {
        
    // }

    // // Called every frame. 'delta' is the elapsed time since the previous frame.
    // public override void _Process(float delta)
    // {
        
    // }
}

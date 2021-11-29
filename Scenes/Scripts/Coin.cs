using Godot;
using System;

public class Coin : Area2D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ((AnimationPlayer)GetNode("AnimationPlayer")).Play("idle");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

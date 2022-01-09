using Godot;
using System;

public class UpgradeOrb : Area2D
{
    public void showMenu(KinematicBody2D body)
    {
        GetTree().Paused = true;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

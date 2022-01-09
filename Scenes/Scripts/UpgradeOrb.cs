using Godot;
using System;

public class UpgradeOrb : Area2D
{
    private World world;
    private World World
    {
        get
        {
            if(world == null)
            {
                world = (World)GetTree().Root.GetNode("Node2D");
            }
            return world;
        }
    }

    public void showMenu(KinematicBody2D body)  //in the future you'll be able to 
    {                                           //upgrade more than just hearts
        //GetTree().Paused = true;
        World.player.addHearts();
        QueueFree();
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

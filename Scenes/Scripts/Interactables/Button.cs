using Godot;
using System;

public class Button : Node2D
{
    [Export]
    public bool permanent = true;


    [Signal]
    public delegate void _on_Pressed_Signal();
    private bool canBePressed = true;

    private AnimationPlayer player;

    public void _onPressed(KinematicBody body)
    {
        GD.Print("pressed");
        if(!canBePressed)
        {
            return;
        }
        player.Play("press");
        canBePressed = false;
        EmitSignal(nameof(_on_Pressed_Signal));
    }

    public void _onUnpressed(KinematicBody body)
    {
        if(!permanent)
        {
            canBePressed = true;
            player.Play("unpress");
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        player = (AnimationPlayer)GetNode("AnimationPlayer");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

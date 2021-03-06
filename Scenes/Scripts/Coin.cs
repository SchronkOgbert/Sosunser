using Godot;
using System;

public class Coin : Area2D
{
    private bool collected = false;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ((AnimationPlayer)GetNode("AnimationPlayer")).Play("idle");
    }

    public void _on_Coin_body_entered(KinematicBody body)
    {
        if(!collected)
        {
            collected = true;
            body.CallDeferred("addCoins", 1, true);
            body.CallDeferred("getPoints", 5);
            ((AnimationPlayer)GetNode("AnimationPlayer")).Play("fade");
        }
    }

    public void _on_AnimationPlayer_animation_finished(string anim_name)
    {
        QueueFree();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        //((AnimationPlayer)GetNode("AnimationPlayer")).Play("idle");
    }
}

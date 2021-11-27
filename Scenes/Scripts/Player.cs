using Godot;
using System;

public class Player : KinematicBody2D
{
    //movement
    const float default_run_speed = 150;
    const float default_sprint_speed = 250;
    [Export]
    public float speed = 150;
    private Vector2 velocity;
    private float speedMultiplier = 1;
    
    //references
    private Sprite sprite;
    private AnimationPlayer player;

    // Called when the node enters the scene tree for the first time.

    public override void _Draw()
    {
        base._Draw();
        sprite = (Sprite)GetNode("Sprite");
        player = (AnimationPlayer)GetNode("AnimationPlayer");
    }

    public override void _Ready()
    {
        //player.Play("idle");
    }

    private void handle_movement(float delta)
    {
        if(Input.IsKeyPressed(68))
        {
            //right movement
            if(sprite.FlipH)
            {
                sprite.FlipH = false;
            }
            velocity.x = speed;
        }
        else if(Input.IsKeyPressed(65))
        {
            //left movement
            if(!sprite.FlipH)
            {
                sprite.FlipH = true;
            }
            velocity.x = -speed;
        }
        else
        {
            velocity = Vector2.Zero;
            player.Play("idle");
        }
        if(velocity.x != 0)
        {
            if(Input.IsKeyPressed(16777237))
            {
                speed = default_sprint_speed;
            }
            else
            {
                speed = default_run_speed;
            }
            MoveAndSlide(velocity * delta * 75);
            player.Play("run", -1, 2);
        }
        else
        {
            player.Play("idle");
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        handle_movement(delta);
    }
}

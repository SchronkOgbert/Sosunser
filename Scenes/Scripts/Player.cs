using Godot;
using System;

public class Player : KinematicBody2D
{
    //physics
    private float gravity = 9.8f;

    //movement
    private const float default_run_speed = 150;
    const float default_sprint_speed = 250;
    [Export]
    public float speed = 150;
    private Vector2 velocity;
    private float speedMultiplier = 1;
    private float jump = 5;
    private Vector2 fallVelocity; 
    private Vector2 terminalVelocity = new Vector2(0, 1000);
    
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

    private void checkMovement()
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
    }

    private void checkSprint()
    {
        if(Input.IsActionJustPressed("sprint"))
        {
            speed = default_sprint_speed;
        }
        if(Input.IsActionJustReleased("sprint"))
        {
            speed = default_run_speed;
        }
    }

    private void move(Vector2 velocity, float delta)
    {
        MoveAndSlide(velocity * delta * 75);
        player.Play("run", -1, 0.013f * speed);
    }

    private void handleMovement(float delta)
    {
        checkMovement();
        checkSprint();
        if(velocity.x != 0)
        {            
            move(this.velocity, delta);
        }
        else
        {
            player.Play("idle");
        }
    }

    private void handleGravity(float delta)
    {
        if(!IsOnFloor())
        {
            fallVelocity.y += 9.8f;
            if(fallVelocity.y > terminalVelocity.y)
            {
                fallVelocity.y = terminalVelocity.y;
            }
            MoveAndSlide(fallVelocity * delta * 75);
            //GD.Print("falling"); 
        }
        else
        {
            fallVelocity.y = 0;
            //GD.Print("not falling"); 
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        handleMovement(delta);
        handleGravity(delta);
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
    }
}

using Godot;
using System;

public class Player : KinematicBody2D
{
    //physics
    private const float GRAVITY = 20f;
    private const float JUMP_FORCE = -512;

    //movement
    private const float DEFAULT_RUN_SPEED = 150;
    private const float DEFAULT_SPRINT_SPEED = 250;
    [Export]
    public float speed = 150;
    private Vector2 velocity;
    private float speedMultiplier = 1;
    private Vector2 actualFallVelocity;
    private Vector2 terminalVelocity = new Vector2(0, 1000);
    private bool isJumping = false;
    
    //references
    private Sprite sprite;
    private AnimationPlayer player;
    private Timer jumpTimer;

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
        velocity.y = 1;
    }    

    private void Jump()
    {
        if(IsOnFloor())
        {
            isJumping = true;
            velocity.y = JUMP_FORCE;
            GD.Print("jumping");
            //player.Play("jump_start");
        }
    }

    private void longJump()
    {
        GD.Print("checking short jump");
        if(!Input.IsActionPressed("jump"))
        {
            velocity.y = 0;
            GD.Print("short jump");
        }
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
            velocity.x = 0;
            player.Play("idle");
        }
    }

    private void checkSprint()
    {
        if(Input.IsActionJustPressed("sprint"))
        {
            speed = DEFAULT_SPRINT_SPEED;
        }
        if(Input.IsActionJustReleased("sprint"))
        {
            speed = DEFAULT_RUN_SPEED;
        }
    }

    private void move(Vector2 velocity, float delta)
    {
        if(!isJumping)
        {
            if(velocity.x != 0)
            {
                player.Play("run", -1, 0.013f * speed);
            }
            else
            {
                player.Play("idle");
            }
        }
        MoveAndSlide(velocity * delta * 75, Vector2.Up);
    }

    private void handleMovement(float delta)
    {
        if(Input.IsActionJustPressed("jump"))
        {
            Jump();
        }
        else
        {
            checkMovement();
            checkSprint();
        }
        this.move(this.velocity, delta);        
    }
    private void handleGravity(float delta)
    {
        if(!IsOnFloor())
        {
            velocity.y += GRAVITY;
            if(velocity.y > terminalVelocity.y)
            {
                velocity.y = terminalVelocity.y;
            }
            if(isJumping)
            {
                if(velocity.y < 0)
                {
                    player.Play("jump_start");
                }
                else
                {
                    player.Play("fall");
                }
            }
            MoveAndSlide(velocity * delta * 75, Vector2.Up);
            //GD.Print("falling"); 
        }
        else
        {
            isJumping = false;
            velocity.y = 1;
            //GD.Print("not falling"); 
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        handleMovement(delta);
        handleGravity(delta);
        //GD.Print(isJumping);
        //GD.Print(velocity, "and", fallVelocity);
    }
}

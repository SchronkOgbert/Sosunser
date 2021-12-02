using Godot;
using System;

public class Enemy : KinematicBody2D 
{
    //physics
    private const float GRAVITY = 20f;

    //configurables
    [Export]
    public float patrolLength = 64;
    [Export]
    public float goesRight = 1;
    [Export]
    public float waitTime = 1;
    [Export]
    public float walkSpeed = 100;
    [Export]
    public float sprintSpeed = 300;

    //references
    private Sprite sprite;
    private CollisionShape2D collission;
    private AnimationPlayer player;
    private Timer timer;

    //movement
    private Vector2 velocity;
    private Vector2 terminalVelocity = new Vector2(0, 1000);
    private Vector2 targetDestination;
    private float speed;
    private float maxX;
    private float minX;

    public override void _Draw()
    {
        sprite = (Sprite)GetNode("Sprite");
        collission = (CollisionShape2D)GetNode("CollisionShape2D");
        player = (AnimationPlayer)GetNode("AnimationPlayer");
        timer = (Timer)GetNode("Timer");
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
            if(velocity.y < 0)
            {
                player.Play("jump_start");
            }
            else
            {
                player.Play("fall");
            }
            MoveAndSlide(velocity * delta * 75, Vector2.Up);
            //GD.Print("falling"); 
        }
        else
        {
            velocity.y = 1;
            //GD.Print("not falling"); 
        }
    }

    private void handleAnimation()
    {
        if(velocity.x != 0)
        {
            player.Play("walk", -1, velocity.x / 150);
        }
        else
        {
            player.Play("idle");
        }
    }

    private void move(Vector2 velocity, float delta)
    {
        MoveAndSlide(velocity * delta * 75, Vector2.Up);
    }

    private bool compareFloatsWithError(float x, float y, float error = 0.1f)
    {
        return (Math.Abs(x - y) < error);
    }

    private void computeTargetDestination()
    {
        targetDestination.x = targetDestination.x + (goesRight * patrolLength);
        //GD.Print(targetDestination, Position, "at speed", velocity.x);
    }

    private void turnAround()
    {
        goesRight = -1* goesRight;
        velocity.x = speed * goesRight;
        sprite.FlipH = !sprite.FlipH;
        computeTargetDestination();
    }

    private void handlePatrol()
    {
        if(Position.x > maxX || Position.x < minX)
        {
            Position = targetDestination;
            return;
        }
        if(IsOnWall())
        {
            turnAround();
        }
        if(compareFloatsWithError(Position.x, targetDestination.x, 5))
        {
            Position = new Vector2(targetDestination.x, Position.y);
            //GD.Print("target destination reached");
            if(timer.IsStopped() && velocity.x != 0)
            {
                //GD.Print("timer started");
                timer.Start();
            }
            if(timer.TimeLeft > 0)
            {
                velocity.x = 0;                
                return;
            }
            //GD.Print("timer stopped");
            //GD.Print("changing direction to: ", Math.Abs(Position.x - targetDestination.x));
            turnAround();
        }        
    }

    private void handleMovement(float delta)
    {
        if(goesRight != 0)
        {
            handlePatrol();
        }
        if(delta >= 1) return;
        move(velocity, delta);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        sprite = (Sprite)GetNode("Sprite");
        collission = (CollisionShape2D)GetNode("CollisionShape2D");
        player = (AnimationPlayer)GetNode("AnimationPlayer");
        timer = (Timer)GetNode("Timer");
        if(goesRight == 1)
        {
            maxX = Position.x + patrolLength + 16;
            minX = Position.x - 16;
        }
        else
        {
            maxX = Position.x - patrolLength - 16;
            minX = Position.x + 16;
        }
        timer.WaitTime = waitTime;
        targetDestination = Position;
        speed = walkSpeed;
        computeTargetDestination();
        velocity.x = goesRight * speed;        
        timer.Start();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        handleGravity(delta);
        handleMovement(delta);
        handleAnimation();
        //GD.Print(targetDestination, Position, "at speed", velocity.x);
    }
}

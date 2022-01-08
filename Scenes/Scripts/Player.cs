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

    //items and abilities
    private int coins = 0;
    private int maxHP = 3;
    private int currentHP = 3;

    //references
    private Sprite sprite;
    private AnimationPlayer player;
    private Timer jumpTimer;
    public GameHUD gameHUD;
    private Vector2 snap;
    private Area2D swordCollision;
    private Timer attackTimer;
    private World _world;

    //helpers
    private int attackIndex = 0;
    private bool isAttacking = false;
    private bool nextAttack = false;
    private int experience;
    private int level;

    //properties
    public int MaxHP
    {
        get
        {
            if (maxHP >= 0) return maxHP;
            return maxHP;
        }
        set
        {
            if (value < 0)
            {
                maxHP = 0;
                return;
            }
            maxHP = value;
        }
    }

    public int CurrentHP
    {
        get
        {
            if (currentHP >= 0) return currentHP;
            return maxHP;
        }
        set
        {
            if (value < 0)
            {
                currentHP = 0;
                return;
            }
            if (value > MaxHP)
            {
                CurrentHP = MaxHP;
                return;
            }
            currentHP = value;
        }
    }

    public World World
    {
        get
        {
            if (_world == null)
            {
                _world = (World)GetTree().Root.GetNode("Node2D");
            }
            //GD.Print("World is: ", _world);
            return _world;
        }
    }
    [Export]
    public int AttackIndex { get => attackIndex; set => attackIndex = value; }
    [Export]
    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
    [Export]
    public bool NextAttack { get => nextAttack; set => nextAttack = value; }
    public int Experience { get => experience; private set => experience = value; }
    public int Level { get => level; private set => level = value; }

    public Godot.Collections.Dictionary<string, object> Save()
    {
        return new Godot.Collections.Dictionary<string, object>()
        {
            { "Filename", Filename },
            { "Parent", GetParent().GetPath() },
            { "MaxHealth", MaxHP },
            { "Experience", Experience },
            { "Level", Level },
            { "Coins", coins }
        };
    }

    public void addCoins(int amount = 1, bool playAnimation = true)
    {
        coins += amount;
        //GD.Print("added", amount, "coin");
        if (gameHUD == null)
        {
            //GD.Print("hud is null");
            gameHUD = (GameHUD)GetTree().CurrentScene.GetNode("GameHUD");
        }
        if (gameHUD != null)
        {
            //GD.Print("hud is now valid");
            gameHUD.addCoins(amount, playAnimation);
        }
    }
    private void disableSword()
    {
        // swordCollision.SetCollisionLayerBit(0, false);
        // swordCollision.SetCollisionMaskBit(2, false);
        CollisionShape2D swordActualColl = (CollisionShape2D)swordCollision.GetNode("CollisionShape2D");
        swordActualColl.Disabled = true;
    }

    private void enableSword()
    {
        CollisionShape2D swordActualColl = (CollisionShape2D)swordCollision.GetNode("CollisionShape2D");
        swordActualColl.Disabled = false;
    }

    public void _on_Area2D_body_entered(KinematicBody2D body)
    {
        GD.Print("enemy detected");
        body.Call("takeDamage", this, 10);
    }

    public void takeDamage(int dmg)
    {
        GD.Print("player received damage");
        CurrentHP = CurrentHP - dmg;
        gameHUD.updateHearts(CurrentHP);
        if (CurrentHP == 0)
        {
            Die();
        }
    }

    public void Die()
    {
        GD.Print("me ded");
        try
        {
            GetTree().ChangeScene("res://Scenes/Level1.tscn");
        }
        catch (Exception e)
        {
            GD.Print(e);
        }
    }

    public void adjustAttackPosition()
    {
        if (sprite.FlipH)
        {
            sprite.Position = new Vector2(sprite.Position.x - 16, sprite.Position.y);
            CollisionShape2D swordCollShape = (CollisionShape2D)swordCollision.GetNode("CollisionShape2D");
            swordCollShape.Position = new Vector2(-46, swordCollShape.Position.y);
        }
        else
        {
            sprite.Position = new Vector2(sprite.Position.x + 16, sprite.Position.y);
            CollisionShape2D swordCollShape = (CollisionShape2D)swordCollision.GetNode("CollisionShape2D");
            swordCollShape.Position = new Vector2(46, swordCollShape.Position.y);
        }
    }

    private void startAttack()
    {
        switch (AttackIndex)
        {
            case 0:
                {
                    disableSword();
                    player.Stop();
                    player.Play("attack", -1, 2);
                    IsAttacking = true;
                    GD.Print(IsAttacking);
                    AttackIndex++;
                    break;
                }
            case 1:
                {
                    AttackIndex = 0;
                    break;
                }
            default:
                {
                    AttackIndex = 0;
                    disableSword();
                    break;
                }
        }
    }

    public void getPoints(int amount)
    {
        gameHUD.addScore(amount);
    }

    // Called when the node enters the scene tree for the first time.

    public override void _Draw()
    {
        base._Draw();
        sprite = (Sprite)GetNode("Sprite");
        player = (AnimationPlayer)GetNode("AnimationPlayer");
        gameHUD = (GameHUD)GetTree().CurrentScene.GetNode("GameHUD");
    }

    public override void _Ready()
    {
        //player.Play("idle");
        velocity.y = 1;
        try
        {
            gameHUD = (GameHUD)World.GetNode("GameHUD");
        }
        catch (Exception e)
        {
            GD.PrintErr(e);
        }
        GD.Print(gameHUD);
        if (gameHUD != null)
        {
            gameHUD.hearts = MaxHP;
            gameHUD.coins = coins;
        }
        else
        {
            GD.Print("invalid hud");
        }
        // Camera2D camera = (Camera2D)GetNode("Camera2D");
        // GD.Print(camera.LimitTop);
        jumpTimer = (Timer)GetNode("JumpTimer");
        swordCollision = (Area2D)GetNode("Area2D");
        IsAttacking = false;
    }

    private void Jump()
    {
        if (IsOnFloor())
        {
            isJumping = true;
            velocity.y = JUMP_FORCE;
            //GD.Print("jumping");
            //player.Play("jump_start");
            jumpTimer.Start();
        }
    }

    private void _on_JumpTimer_timeout()
    {
        //GD.Print("long jump start");
        if (!Input.IsActionPressed("jump"))
        {
            velocity.y = JUMP_FORCE / 4;
        }
    }

    private void longJump()
    {
        //GD.Print("checking short jump");
        if (!Input.IsActionPressed("jump"))
        {
            velocity.y = 0;
            GD.Print("short jump");
        }
    }

    private float clamp(float value, float min = 0.5f, float max = 1)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    private void checkMovement()
    {
        if (IsAttacking)
        {
            velocity.x = 0;
            return;
        }
        if (Input.IsKeyPressed(68))
        {
            //right movement
            if (sprite.FlipH)
            {
                sprite.FlipH = false;
            }
            velocity.x = speed;
        }
        else if (Input.IsKeyPressed(65))
        {
            //left movement
            if (!sprite.FlipH)
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
        // if(isJumping)
        // {
        //     velocity.x = velocity.x * clamp((Math.Abs(velocity.y) / -JUMP_FORCE));
        // }
        if (IsOnCeiling())
        {
            velocity.y = 0;
        }
    }

    private void checkSprint()
    {
        if (Input.IsActionJustPressed("sprint"))
        {
            speed = DEFAULT_SPRINT_SPEED;
        }
        if (Input.IsActionJustReleased("sprint"))
        {
            speed = DEFAULT_RUN_SPEED;
        }
    }

    private void checkAttack()
    {
        //GD.Print(attackIndex);
        if (Input.IsActionJustPressed("attack"))
        {
            if (AttackIndex == 0 || AttackIndex == 1 && NextAttack)
            {
                //GD.Print("attacking");
                startAttack();
            }
        }
    }

    private void move(Vector2 velocity, float delta)
    {
        if (IsAttacking) return;
        if (!isJumping)
        {
            if (velocity.x != 0)
            {
                player.Play("run", -1, speed / DEFAULT_RUN_SPEED);
            }
            else
            {
                player.Play("idle");
            }
        }
        MoveAndSlideWithSnap(velocity * delta * 75, snap, Vector2.Up);
    }

    private void handleMovement(float delta)
    {
        if (Input.IsActionJustPressed("jump"))
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
        if (!IsOnFloor())
        {
            velocity.y += GRAVITY;
            if (velocity.y > terminalVelocity.y)
            {
                velocity.y = terminalVelocity.y;
            }
            if (isJumping)
            {
                if (velocity.y < 0)
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
        if (IsOnFloor())
        {
            snap = Vector2.Down;
            checkAttack();
        }
        else
        {
            snap = Vector2.Zero;
        }
        handleMovement(delta);
        handleGravity(delta);
        //GD.Print(isJumping);
        //GD.Print(velocity, "and", fallVelocity);
    }
}




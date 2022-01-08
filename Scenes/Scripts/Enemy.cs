using Godot;
using System;

public enum attackerType
{
	Melee,
	Projectile,
	Both
}

public class Enemy : KinematicBody2D 
{
	//physics
	private const float GRAVITY = 20f;

    //configurables
    private float patrolLength = 64;
    private float goesRight = 1;
    private float waitTime = 1;
    private float walkSpeed = 100;
    private float sprintSpeed = 300;
    private float maxHP = 20;    
    private attackerType type;
    private float currentHP;

	//references
	private Sprite sprite;
	private CollisionShape2D collission;
	private AnimationPlayer animationPlayer;
	private Timer timer;
	private RayCast2D edgeRay;
	private Position2D projectilePosition;
	private World world;
	private Timer playerLostTimer;

	//movement
	private Vector2 velocity;
	private Vector2 terminalVelocity = new Vector2(0, 1000);
	private Vector2 targetDestination;
	private float speed;
	private float maxX;
	private float minX;

    //combat
    private bool hurt = false;
    private Player player;
	private Timer decisionTimer;
	private bool inCombat = false;
	private bool searchingPlayer = false;

	//properties
	public World World
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
	[Export]
	public float PatrolLength { get => patrolLength; set => patrolLength = value; }
	[Export]
	public float GoesRight { get => goesRight; set => goesRight = value; }
	[Export]
	public float WaitTime { get => waitTime; set => waitTime = value; }
	[Export]
	public float WalkSpeed { get => walkSpeed; set => walkSpeed = value; }
	[Export]
	public float SprintSpeed { get => sprintSpeed; set => sprintSpeed = value; }
	[Export]
	public float MaxHP { get => maxHP; set => maxHP = value; }
	[Export]
	public attackerType Type { get => type; set => type = value; }
	[Export]
    public bool Hurt { get => hurt; set => hurt = value; }

    public override void _Draw()
	{
		sprite = (Sprite)GetNode("Sprite");
		collission = (CollisionShape2D)GetNode("CollisionShape2D");
		animationPlayer = (AnimationPlayer)GetNode("AnimationPlayer");
		//timer = (Timer)GetNode("Timer");
	}

	public void playerDetected(KinematicBody2D body)
	{
		//if(body != null && body != _world.player) return;
		//GD.Print("saw player: ", body);
		inCombat = true;
		searchingPlayer = false;
		_startAttack();
	}

	public void playerOutOfSight(KinematicBody2D body)
	{
		//GD.Print("player disappeared");
		searchingPlayer = true;
		inCombat = false;
		playerLostTimer.Start();
	}

	public void playerLost()
	{
		//GD.Print("player lost");
		searchingPlayer = false;
	}

	private void die()
	{
		if(currentHP <= 0)
		{
			animationPlayer.Stop();
			animationPlayer.Play("die");            
			SetCollisionMaskBit(0, false);
			SetCollisionLayerBit(2, false);
		}
		else
		{
			animationPlayer.Play("idle");
			startAttack();
		}
	}

	private void _startAttack()
	{
		if(searchingPlayer || currentHP <= 0) return;
		findPlayer(World.player);
		World.CallDeferred("spawnProjectile",
		projectilePosition.GlobalPosition, 256 * GoesRight,
		2, GetTree().Root.GetNode("Node2D/Player"));
		//GD.Print("started attack");
		startAttack();
	}

	private void findPlayer(KinematicBody2D body)
	{
		if(Math.Sign(body.Position.x - Position.x) != Math.Sign(GoesRight))
		{
			turnAround();
		}
	}

	public void startAttack()
	{
		decisionTimer.WaitTime = new RandomNumberGenerator().Randf() + 1f;
		decisionTimer.Start();
		if(!Hurt)
		{
			animationPlayer.Play("idle");
		}
	}

	public void takeDamage(KinematicBody2D body, int dmg = 1)
	{
		inCombat = true;
		//searchingPlayer = false;
		timer.WaitTime = timer.TimeLeft + 0.4f;
		currentHP -= dmg;
		findPlayer(body);
		Hurt = true;
		animationPlayer.Stop();
		animationPlayer.Play("get_hurt", -1, 0.5f);
	}
	private void death_cleanup()
	{
		World.player.getPoints(20);
		QueueFree();
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
				animationPlayer.Play("jump_start");
			}
			else
			{
				//animationPlayer.Play("fall");
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
		//GD.Print(!hurt && currentHP > 0 && !inCombat && !searchingPlayer);
		if(!Hurt && currentHP > 0 && !inCombat && !searchingPlayer)
		{
			if(velocity.x != 0)
			{
				animationPlayer.Play("walk", -1, velocity.x / 150);
				//GD.Print("playing walk");
			}
			else
			{
				//GD.Print("playing idle");
				animationPlayer.Play("idle");
			}
		}
	}

	private void move(Vector2 velocity, float delta)
	{
		if(Hurt || currentHP <= 0 || inCombat || searchingPlayer) return;
		MoveAndSlide(velocity * delta * 75, Vector2.Up);
	}

	private bool compareFloatsWithError(float x, float y, float error = 0.1f)
	{
		return (Math.Abs(x - y) < error);
	}

	private void computeTargetDestination()
	{
		targetDestination.x = targetDestination.x + (GoesRight * PatrolLength);
		//GD.Print(targetDestination, Position, "at speed", velocity.x);
	}

	private void turnAround()
	{
		GoesRight = -1* GoesRight;
		velocity.x = speed * GoesRight;
		sprite.Scale = new Vector2(sprite.Scale.x * -1, sprite.Scale.y);
		computeTargetDestination();
	}

	private void handlePatrol()
	{
		if(Hurt || currentHP <= 0) return;
		if(Position.x > maxX || Position.x < minX)
		{
			Position = targetDestination;
			return;
		}
		if(IsOnWall() || !edgeRay.IsColliding())
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
		if(GoesRight != 0)
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
		animationPlayer = (AnimationPlayer)GetNode("AnimationPlayer");
		timer = (Timer)GetNode("Timer");
		edgeRay = (RayCast2D)GetNode("Sprite/RayCast2D");
		decisionTimer = (Timer)GetNode("decisionTimer");
		projectilePosition = (Position2D)GetNode("Sprite/Position2D");
		playerLostTimer = (Timer)GetNode("playerLostTimer");
		if(GoesRight != 0)
		{
			GoesRight = Scale.x;
		}
		if(GoesRight == 1)
		{
			maxX = Position.x + PatrolLength + 16;
			minX = Position.x - 16;
		}
		else
		{
			maxX = Position.x - PatrolLength - 16;
			minX = Position.x + 16;
		}
		currentHP = MaxHP;
		timer.WaitTime = WaitTime;
		targetDestination = Position;
		speed = WalkSpeed;
		computeTargetDestination();
		velocity.x = GoesRight * speed;        
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



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
	private int _maxHP = 3;
	private int _currentHP = 3;
	
	//references
	private Sprite sprite;
	private AnimationPlayer player;
	private Timer jumpTimer;
	private GameHUD gameHUD;
	private Vector2 snap;
	private Area2D swordCollision;
	private Timer attackTimer;
	private World _world;

	//helpers
	[Export]
	public int attackIndex = 0;
	[Export]
	public bool isAttacking = false;
	[Export]
	public bool nextAttack = false;

	//properties
	public int maxHP
	{
		get 
		{ 
			if(_maxHP >= 0) return _maxHP;
			return _maxHP;
		}
		set
		{
			if(value < 0)
			{
				 _maxHP = 0;
				 return;
			}
			_maxHP = value;
		}
	}

	public int currentHP
	{
		get 
		{ 
			if(_currentHP >= 0) return _currentHP;
			return _maxHP;
		}
		set
		{
			if(value < 0)
			{
				_currentHP = 0;
				return;
			} 				
			if(value > maxHP) 
			{
				currentHP = maxHP;
				return;
			}
			_currentHP = value;
		}
	}

	public World world
	{
		get
		{
			if(_world == null)
			{
				_world = (World)GetTree().Root.GetNode("Node2D");
			}
			return _world;
		}
	}

	public void addCoins(int amount = 1, bool playAnimation = true)
	{
		coins += amount;
		//GD.Print("added", amount, "coin");
		if(gameHUD == null)
		{
			//GD.Print("hud is null");
			gameHUD = (GameHUD)GetTree().CurrentScene.GetNode("GameHUD");
		}
		if(gameHUD != null)
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
		currentHP = currentHP - dmg;
		gameHUD.updateHearts(currentHP);
		if(currentHP == 0)
		{
			Die();
		}
	}

	public void Die()
	{
		GD.Print("me ded");
		GetTree().ChangeScene("res://Scenes/Level1.tscn");
	}

	public void adjustAttackPosition()
	{
		if(sprite.FlipH)
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
		switch(attackIndex)
		{
			case 0:
			{
				disableSword();
				player.Stop();
				player.Play("attack", -1, 2);
				isAttacking = true;
				GD.Print(isAttacking);
				attackIndex++;
				break;
			}
			case 1:
			{
				attackIndex = 0;
				break;
			}
			default: 
			{
				attackIndex = 0;
				disableSword();
				break;
			}
		}
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
		gameHUD = (GameHUD)world.GetNode("GameHUD");
		GD.Print(gameHUD);
		if(gameHUD != null)
		{
			gameHUD.hearts = maxHP;
			gameHUD.coins = coins;
		}
		else
		{
			GD.Print("invalid hud");
		}
		jumpTimer = (Timer)GetNode("JumpTimer");
		swordCollision = (Area2D)GetNode("Area2D");
		isAttacking = false;
	}    

	private void Jump()
	{
		if(IsOnFloor())
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
		if(!Input.IsActionPressed("jump"))
		{
			velocity.y = JUMP_FORCE / 4;
		}
	}

	private void longJump()
	{
		//GD.Print("checking short jump");
		if(!Input.IsActionPressed("jump"))
		{
			velocity.y = 0;
			GD.Print("short jump");
		}
	}

	private float clamp(float value, float min = 0.5f, float max = 1)
	{
		if(value < min) return min;
		if(value > max) return max;
		return value;
	}

	private void checkMovement()
	{        
		if(isAttacking)
		{
			velocity.x = 0;
			return;
		}
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
		// if(isJumping)
		// {
		//     velocity.x = velocity.x * clamp((Math.Abs(velocity.y) / -JUMP_FORCE));
		// }
		if(IsOnCeiling())
		{
			velocity.y = 0;
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

	private void checkAttack()
	{
		//GD.Print(attackIndex);
		if(Input.IsActionJustPressed("attack"))
		{
			if(attackIndex == 0 || attackIndex == 1 && nextAttack)
			{
				//GD.Print("attacking");
				startAttack();
			}
		}
	}

	private void move(Vector2 velocity, float delta)
	{
		if(isAttacking) return;
		if(!isJumping)
		{
			if(velocity.x != 0)
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
		if(IsOnFloor())
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




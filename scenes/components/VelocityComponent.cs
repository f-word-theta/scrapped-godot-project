using Godot;
using System;

[GlobalClass]
public partial class VelocityComponent : Node
{
	[ExportGroup("Physics Constants")]
	public const float Gravity = 65f;
	public const float Friction = 9f;

	[ExportGroup("Necessary Components")]
	[Export] StaminaComponent staminaComponent { get; set; }
	[Export] HealthComponent healthComponent { get; set; }
	[Export] public Node3D Gimbal { get; set; }

	public bool JustLanded { get; private set; }
	public bool WishJump { get; set; }
	public bool CanDoubleJump { get; private set; }
	
	public CharacterBody3D CharacterBody { get; private set; }

	public Vector3 WishDirection { get; set; }
	public Vector3 Velocity { get => CharacterBody.Velocity; set => CharacterBody.Velocity = value; }
	public Vector3 ZeroYVelocity => new (CharacterBody.Velocity.X, 0, CharacterBody.Velocity.Z);

	public float VelocityY { get => Velocity.Y; set => Velocity = Velocity with { Y = value }; }

	[Export] public float MaxAcceleration = 15f;
	
	public float MaxAirAcceleration;

	[Export] public float JumpImpulse = 20f;
	[Export] public bool Controllable = false;


	public override void _Ready() {
		CharacterBody = GetParent<CharacterBody3D>();

		MaxAirAcceleration = MaxAcceleration * 0.3f;
	}

	public override void _PhysicsProcess(double delta)
	{	
		UpdateWishDirection();
		UpdateFloorSnapLength();

		if (healthComponent is not null && !healthComponent.Alive) return;
		
		UpdateHorizontalMovement((float) delta);
		UpdateVerticalMovement((float) delta);
		
		QueueJump();
		QueueDoubleJump();

		CharacterBody.MoveAndSlide();
	}

	//=====================================================================================================================

	void UpdateHorizontalMovement(float delta)
	{
		if (CharacterBody.IsOnFloor()) MoveGround(Velocity, delta);
		else MoveAir(Velocity, delta);
	}

	void UpdateVerticalMovement(float delta)
	{
		if (CharacterBody.IsOnFloor())
		{
			if (WishJump)
			{
				if (staminaComponent is not null) staminaComponent.DepleteStamina(StaminaComponent.StaminaCosts.Jump);

				VelocityY = JumpImpulse;
			}
			else VelocityY = 0f;
		}
		else VelocityY -= Gravity * delta;
	}

	void QueueDoubleJump()
	{
		if (CharacterBody.IsOnFloor()) CanDoubleJump = true;
		else
		{
			if (CanDoubleJump && WishJump)
			{
				CanDoubleJump = false;
				
				if (staminaComponent is not null) staminaComponent.DepleteStamina(StaminaComponent.StaminaCosts.DoubleJump);

				VelocityY = JumpImpulse / 1.2f;
			}
		}
	}

	void QueueJump()
	{
		if (Controllable)
			WishJump = ((CharacterBody.IsOnFloor() && Input.IsActionPressed("jump")) ||
			            (!CharacterBody.IsOnFloor() && Input.IsActionJustPressed("jump"))) && staminaComponent is not null && staminaComponent.Stamina > 0f;
	}

	void UpdateWishDirection()
	{
		if (Controllable)
		{
			var inputVector = Input.GetVector("left", "right", "forward", "backward");
			
			if (Gimbal is not null) WishDirection = Gimbal.Basis * new Vector3(inputVector.X, 0, inputVector.Y).Normalized();
			else WishDirection = CharacterBody.Basis * new Vector3(inputVector.X, 0, inputVector.Y).Normalized();
		}
	}

	void UpdateFloorSnapLength()
	{
		JustLanded = CharacterBody.IsOnFloor() && CharacterBody.FloorSnapLength == 0f;

		if (WishJump || !CharacterBody.IsOnFloor()) CharacterBody.FloorSnapLength = 0.0f;
		if (JustLanded) CharacterBody.FloorSnapLength = 1.0f;
	}

	void MoveGround(Vector3 inputVelocity, float delta)
	{
		Vector3 nextVelocity = new (inputVelocity.X, VelocityY, inputVelocity.Z);
		nextVelocity = ApplyFriction(nextVelocity, Friction, delta);
		nextVelocity += Accelerate(nextVelocity, WishDirection, MaxAcceleration, delta);

		Velocity = nextVelocity;
	}

	void MoveAir(Vector3 inputVelocity, float delta)
	{
		Vector3 nextVelocity = new (inputVelocity.X, Velocity.Y, inputVelocity.Z);
		nextVelocity += Accelerate(nextVelocity, WishDirection, MaxAirAcceleration, delta);

		Velocity = nextVelocity;
	}
	
	static Vector3 ApplyFriction(Vector3 inputVelocity, float inputFriction, float inputDelta)
	{
		float speed = inputVelocity.Length();
		Vector3 scaledVelocity = new ();

		if (speed != 0f)
		{
			float drop = speed * inputFriction * inputDelta;
			scaledVelocity = inputVelocity * Mathf.Max(speed - drop, 0) / speed;
		}

		if (speed < 0.01f) return Vector3.Zero;

		return scaledVelocity;
	}

	static Vector3 Accelerate(Vector3 inputVelocity, Vector3 inputWishDir, float maxAcceleration, float inputDelta)
	{
		Vector3 hVelocity = new (inputVelocity.X, 0f, inputVelocity.Z);
		float curSpeed = hVelocity.Dot(inputWishDir);
		float curAccel = Mathf.Clamp(maxAcceleration - curSpeed, 0f, 215f * inputDelta);

		return inputWishDir * curAccel;
	}
}

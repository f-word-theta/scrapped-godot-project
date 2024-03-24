using Godot;
using Vector2 = Godot.Vector2;
using Vector3 = Godot.Vector3;

[GlobalClass]
public partial class DashComponent : Node
{
	[ExportGroup("Dash-related Properties")]
	[Export] public float DashAcceleration = 30f;
	[Export] public float DashFov = 95f;
	[Export] public float GroundMultiplier = 3f;

	[ExportGroup("Necessary Components")]
	[Export] PlayerCameraManager playerCameraManager { get; set; }
	[Export] private VelocityComponent velocityComponent { get; set; }
	[Export] StaminaComponent staminaComponent { get; set; }
	[Export] Timer dashTimer { get; set; }
	[Export] AnimationPlayer dashAnimationPlayer { get; set; }
	
	Vector3 horizontalDashVector { get; set; }
	Vector3 verticalDashVector { get; set; }
	private Vector2 inputVector { get; set; }

	public float DashDuration {
		get => (float) dashTimer.WaitTime;
		set => dashTimer.WaitTime = value;
	}

	public bool WishDash { get; set; }
	public bool TimerOnIdle => dashTimer.IsStopped() && dashTimer.TimeLeft == 0f;
	public bool TimerRunning => !dashTimer.IsStopped() && dashTimer.TimeLeft != 0f;
	
	public override void _PhysicsProcess(double delta)
	{
		QueueDash();
		GetInputVector();
		
		UpdateCameraFov((float) delta);

		if (WishDash)
		{
			dashTimer.Start();
			staminaComponent.DepleteStamina(StaminaComponent.StaminaCosts.Dash);
			dashAnimationPlayer.PlayFromBeginning("default");
		}

		if (TimerRunning) Dash(DashAcceleration);
	}

	//=====================================================================================================================

	void UpdateCameraFov(float delta)
	{
		if (playerCameraManager is null) return;
		
		if (TimerOnIdle) playerCameraManager.Cam.Fov = Mathf.Lerp(playerCameraManager.Cam.Fov, playerCameraManager.Fov, delta * 5f);
		else playerCameraManager.Cam.Fov = Mathf.Lerp(playerCameraManager.Cam.Fov, DashFov, 20f * delta);
	}


	void QueueDash()
	{
		WishDash = Input.IsActionJustPressed("dash") && TimerOnIdle && staminaComponent.Stamina >= (int) StaminaComponent.StaminaCosts.Dash;
	}

	void GetInputVector()
	{
		inputVector = Input.GetVector("left", "right", "backward", "forward");
	}

	void Dash(float acceleration)
	{
		Vector3 direction;
		
		Vector3 forward = -velocityComponent.Gimbal.GlobalTransform.Basis.Z;
		Vector3 left = velocityComponent.Gimbal.GlobalTransform.Basis.X;
		
		if ((inputVector.Length() == 0 || inputVector.Y > 0) && inputVector.X == 0)direction = forward;
		else direction = -forward * -Mathf.Sign(inputVector.Y) + left * Mathf.Sign(inputVector.X);

		if (velocityComponent.CharacterBody.IsOnFloor()) direction *= GroundMultiplier;
		
		velocityComponent.Velocity = direction * acceleration;
	}
}

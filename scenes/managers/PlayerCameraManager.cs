using Godot;
using System;

public partial class PlayerCameraManager : Node
{
	[Export] private VelocityComponent velocityComponent { get; set; }
	[Export] private HealthComponent healthComponent { get; set; }

	[Export] public float Sensitivity = 0.05f;
	[Export] public float Fov = 75f;
	
	[Export] public Node3D Gimbal { get; set; }
	[Export] public Node3D ModificationGimbal { get; set; }
	[Export] public Camera3D Cam { get; set; }

	[Export] public float Trauma = 0f;
	[Export] public float TraumaPower = 3f;
	[Export] public float Decay = 0.8f;
	[Export] public float MaxShakeRoll = 0.01f;

	public bool BobbleEnabled = true;
	public bool CameraSwayEnabled = true;
	public bool BreathingEnabled = true;
	public bool RumbleEnabled = true;

	private Vector3 bobble { get; set; }
	private Vector3 rumble { get; set; }
	private Vector3 breathing { get; set; }
	
	Vector2 mouseRelative = Vector2.Zero;

	private float zRot { get; set; }

	public override void _Ready()
	{
		Initialize();
	}

	public override void _Input(InputEvent @event)
	{
		UpdateToggleMouseLook(@event);
		
		if (healthComponent is not null && !healthComponent.Alive) return;
		
		UpdateLook(@event);
	}
	
	public override void _Process(double delta)
	{
		ResetMouseRelative((float) delta);

		if (healthComponent is not null && !healthComponent.Alive) return;
		
		AddCamLookInertia();
		UpdateCamEffectValues();
		
		HandleModificationGimbalTransform((float) delta);
	}

	//=====================================================================================================================

	private void ResetMouseRelative(float delta)
	{
		mouseRelative = mouseRelative.Lerp(Vector2.Zero, 8f * delta);
		mouseRelative.X = Mathf.Clamp(mouseRelative.X, -4f, 4f);
	}

	void UpdateLook(InputEvent @event)
	{
		if (Input.MouseMode != Input.MouseModeEnum.Captured || !Cam.Current) return;
		if (@event is not InputEventMouseMotion motion) return;
		
		mouseRelative += motion.Relative / 40f;

		Gimbal.RotateY(Mathf.DegToRad(-motion.Relative.X * Sensitivity));
		Cam.RotateX(Mathf.DegToRad(-motion.Relative.Y * Sensitivity));

		Cam.Rotation = new Vector3(
			Mathf.Clamp(
				Cam.Rotation.X,
				Mathf.DegToRad(-85f),
				Mathf.DegToRad(85f)),
			Cam.Rotation.Y,
			Cam.Rotation.Z);
	}

	void UpdateToggleMouseLook(InputEvent @event)
	{
		
	}

	void LerpCamTransform(float delta)
	{
		Cam.Transform = Cam.Transform with
		{
			Origin = Cam.Transform.Origin.Lerp((bobble * Convert.ToInt16(IsTargetMoving()) + rumble), 10f * delta),
		};
	}

	void HandleModificationGimbalTransform(float delta)
	{
		float amount = Mathf.Pow(Trauma, TraumaPower);
		Trauma = Mathf.Max(Trauma - Decay * delta, 0f);

		ModificationGimbal.Rotation = ModificationGimbal.Rotation.Lerp(new Vector3(ModificationGimbal.Rotation.X, ModificationGimbal.Rotation.Y, 0f), 10f * delta);
		
		ModificationGimbal.Rotation = new Vector3(breathing.X + (new RandomNumberGenerator().RandfRange(-1f, 1f) * amount * MaxShakeRoll), breathing.Y + (new RandomNumberGenerator().RandfRange(-1f, 1f) * amount * MaxShakeRoll), ModificationGimbal.Rotation.Z + zRot);

		ModificationGimbal.Position = new Vector3(breathing.X , breathing.Y * 5f, ModificationGimbal.Position.Z);

		//ModificationGimbal.Rotation = new Vector3(ModificationGimbal.Rotation.X, ModificationGimbal.Rotation.Y, ModificationGimbal.Rotation.Z + zRot);
	}

	public void AddTrauma(float amount)
	{
		Trauma = Mathf.Min(Trauma + amount, 1.0f);
	}

	void UpdateCamEffectValues()
	{
		bobble = BobbleEnabled
			? new Vector3(
				Mathf.Cos(Tick.CurrentTick * 5f) * 0.6f / 1.5f,
				Mathf.Abs(Mathf.Sin(Tick.CurrentTick * 5f)) * 0.8f - 0.6f,
				0f
			)
			: Vector3.Zero;

		zRot = CameraSwayEnabled ? Mathf.DegToRad(Mathf.Clamp(mouseRelative.X * 0.09f, -10f, 10f)) : 0f;

		breathing = BreathingEnabled
			? new Vector3(Mathf.Cos(Tick.CurrentTick) * 0.01f / 1.5f, Mathf.Sin(Tick.CurrentTick) * 0.015f, 0f)
			: Vector3.Zero;
		
		rumble = RumbleEnabled ? new Vector3(0, Mathf.Clamp(Mathf.Remap(
			velocityComponent.Velocity.Y,
			-VelocityComponent.Gravity / 10,
			VelocityComponent.Gravity / 10,
			-0.3f,
			0.3f), -0.6f, 0.6f), 0f) : Vector3.Zero;
	}
	
	void AddCamLookInertia()
	{
		Gimbal.Rotation = Gimbal.Rotation with
		{
			Y = Gimbal.Rotation.Y - mouseRelative.X * Sensitivity / 30f,
		};

		Cam.Rotation = Cam.Rotation with
		{
			X = Cam.Rotation.X - mouseRelative.Y * Sensitivity / 30f,
		};
	}

	void Initialize()
	{
		if (Cam.Current) Input.MouseMode = Input.MouseModeEnum.Captured;
		Cam.Fov = Fov;

		GetTree().CallGroup("doom_sprites", "SetCamera", this.Cam);
	}

	bool IsTargetMoving()
	{ 
		return Input.GetVector("left", "right", "forward", "backward").Length() > 0f &&
	         velocityComponent.CharacterBody.IsOnFloor();
	}
}

using Godot;

public partial class PlayerWeaponManager : Node
{
    [Export] public Node3D WeaponOrigin { get; set; }
    [Export] public Weapon CurrentWeapon { get; set; } 
    [Export] public Camera3D WeaponCam { get; set; }

    [Export] private PlayerCameraManager playerCameraManager { get; set; }
    [Export] private VelocityComponent velocityComponent { get; set; }
    [Export] HealthComponent healthComponent { get; set; }

    private Vector2 mouseRelative { get; set; }
    private Vector3 originalPosition { get; set; }
    private Vector3 bobble { get; set; }

    public override void _Input(InputEvent @event)
    {
        if (healthComponent is not null && !healthComponent.Alive) return;

        HandleWeaponInput(@event);
        UpdateLook(@event);
    }

    public override void _Ready()
    {
        Initialize();
    }

    public override void _Process(double delta)
    {
        AttachWeaponCamera((float) delta);
        HandleWeaponVisibility();

        ResetMouseRelative((float) delta);

		if (healthComponent is not null && !healthComponent.Alive) return;

        LerpHandTransform((float) delta);
        UpdateHandEffects((float) delta);
    }

    //=====================================================================================================================

    private void Initialize()
    {
        originalPosition = WeaponOrigin.Transform.Origin;
    }

    private void UpdateLook(InputEvent @event)
    {
        if (Input.MouseMode != Input.MouseModeEnum.Captured || !playerCameraManager.Cam.Current) return;
        if (@event is not InputEventMouseMotion motion) return;

        mouseRelative += motion.Relative / new Vector2(100000f, 500000f);
    }

    void HandleWeaponInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed("primary_fire")) CurrentWeapon.PrimaryFire();
        else if (Input.IsActionJustPressed("secondary_fire")) CurrentWeapon.SecondaryFire();
    }

    private void ResetMouseRelative(float delta)
    {
        mouseRelative = mouseRelative.Lerp(Vector2.Zero, 6f * delta);
        mouseRelative = mouseRelative.Clamp(new Vector2(-0.01f, -0.01f), new Vector2(0.01f, 0.01f));
    }

    void AttachWeaponCamera(float delta)
    {
        if (playerCameraManager is null) return;
        WeaponCam.GlobalTransform = playerCameraManager.Cam.GlobalTransform;
    }

    void HandleWeaponVisibility()
    {
        if (CurrentWeapon is null) return;
        foreach (var _weapon in WeaponOrigin.GetChildren())
        {
            if (_weapon is not Weapon weapon) return;

            weapon.Visible = weapon == CurrentWeapon;
        }
    }

    void LerpHandTransform(float delta)
    {
        WeaponOrigin.Transform = WeaponOrigin.Transform with
        {
            Origin = WeaponOrigin.Transform.Origin.Lerp(originalPosition + bobble, 8f * delta) + new Vector3(-mouseRelative.X / 2, 0f, 0f),
        };
    }
    
    bool IsPlayerMoving()
    { 
        return Input.GetVector("left", "right", "forward", "backward").Length() > 0f &&
               velocityComponent.CharacterBody.IsOnFloor();
    }

    void UpdateHandEffects(float delta)
    {
        if (!IsPlayerMoving()) bobble = Vector3.Zero;
        else
            bobble = new Vector3(
                Mathf.Cos(Tick.CurrentTick * 5f) * 0.05f,
                (-Mathf.Abs(Mathf.Sin(Tick.CurrentTick * 5)) * 0.1f) + 0.05f,
                0f
            );
    }
}
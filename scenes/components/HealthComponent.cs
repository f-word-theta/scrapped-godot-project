using Godot;
using System;

[GlobalClass]
public partial class HealthComponent : Node
{
	[Export] public float MaxHealth = 100f;
	[Export] private Label3D healthLabel;

	public float Health { get; private set; }

	public bool Alive { get; private set; }

	[Signal] public delegate void DamageTakenEventHandler(float oldHealth, float newHealth);
	[Signal] public delegate void HealedEventHandler(float oldHealth, float newHealth);
	
	public override void _Input(InputEvent @event)
	{
		if (Input.IsActionJustPressed("take_damage")) TakeDamage(10f);
		if (Input.IsKeyPressed(Key.R)) Respawn();
	}

	public override void _Ready()
	{
		Respawn();
	}

	public override void _Process(double delta)
	{
		if (healthLabel is not null)
			healthLabel.Text = Health.ToString();
	}

	//=====================================================================================================================

	public void Respawn()
	{
		Alive = true;
		Health = MaxHealth;
	}

	public void Heal(float healAmount)
	{
		if (!Alive) return;
		
		EmitSignal(SignalName.Healed, Health, Mathf.Clamp(Health + healAmount, Health, MaxHealth));
		
		if (Health + healAmount > MaxHealth) Health = MaxHealth;
		else Health += healAmount;
	}

	public void TakeDamage(float damageAmount)
	{
		if (!Alive) return;
		
		EmitSignal(SignalName.DamageTaken, Health,  Mathf.Clamp(Health - damageAmount, 0f, Health));

		if (Health - damageAmount < 0f)
		{
			Alive = false;
			Health = 0f;
		} else Health -= damageAmount;
	}
}

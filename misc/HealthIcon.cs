using Godot;
using System;

public partial class HealthIcon : TextureRect
{
	
	[Export] HealthComponent healthComponent;
	[Export] AnimationPlayer effectAnimationPlayer;
	// [Export] AnimationPlayer beatingAnimationPlayer;
	
	public override void _Ready()
	{
		healthComponent.DamageTaken += OnDamageTaken;
	}

	void OnDamageTaken(float oldHealth, float newHealth)
	{
		effectAnimationPlayer.PlayFromBeginning("Damage");
	}

	public override void _Process(double delta)
	{
		if (Material is not ShaderMaterial material) return;
		
		material.SetShaderParameter("progress", healthComponent.Health / healthComponent.MaxHealth);
		// beatingAnimationPlayer.SpeedScale = Mathf.Remap((healthComponent.MaxHealth / healthComponent.Health), 1f, 10f, 1.2f, 5f);
	}
}

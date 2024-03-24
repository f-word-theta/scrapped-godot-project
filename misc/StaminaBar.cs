using Godot;
using System;

public partial class StaminaBar : Panel
{

	[Export] private StaminaComponent staminaComponent;
	[Export] private AnimationPlayer opacityAnimationPlayer;

	public override void _Ready()
	{
		// CallDeferred(MethodName.OnStaminaFilled);
		
		staminaComponent.StaminaFilled += OnStaminaFilled;
		staminaComponent.StaminaUsed += OnStaminaUsed;
	}

	public override void _Process(double delta)
	{
		ShaderMaterial material = Material as ShaderMaterial;
		material.SetShaderParameter("progress", (float) staminaComponent.Stamina / (float) staminaComponent.MaxStamina);
	}

	void OnStaminaFilled()
	{
		opacityAnimationPlayer.Play("Disappear");
	}

	void OnStaminaUsed()
	{
		opacityAnimationPlayer.Play("Appear");
	}
}

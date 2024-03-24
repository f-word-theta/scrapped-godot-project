using Godot;
using System;

[GlobalClass]
public partial class StaminaComponent : Node
{
	public readonly int MaxStamina = 100;

	[Export] private Timer restorationTimer { get; set; }
	
	public float RestorationTime
	{
		get => (float) restorationTimer.WaitTime;
		set => restorationTimer.WaitTime = value;
	}

	public int Stamina { get; private set; }

	public bool TimerOnIdle => restorationTimer.IsStopped() && restorationTimer.TimeLeft == 0f;
	public bool HasStamina => Stamina > 0;
	public bool OutOfStamina => Stamina <= 0;

	public enum StaminaCosts
	{
		Jump = 1,
		DoubleJump = 4,
		Dash = 10,
	}

	[Signal] public delegate void StaminaFilledEventHandler();
	[Signal] public delegate void StaminaUsedEventHandler();

	public override void _Ready()
	{
		Stamina = MaxStamina;
	}

	public override void _Process(double delta)
	{
		RegenerateStaminaUponIdle();
	}

	//=====================================================================================================================

	void RegenerateStaminaUponIdle(int amount = 1)
	{
		if (TimerOnIdle && Stamina == MaxStamina - 1) EmitSignal(SignalName.StaminaFilled);
		if (TimerOnIdle && Stamina < MaxStamina) Stamina += amount;
	}

	public void DepleteStamina(StaminaCosts amount)
	{
		restorationTimer.Start();
		if (Stamina == MaxStamina) EmitSignal(SignalName.StaminaUsed);

		if (Stamina - amount >= 0 && HasStamina) Stamina -= (int) amount;
		else if (Stamina - amount < 0) Stamina = 0;
	}
	
}

using Godot;
using System;

public partial class WanderState : State
{
	[Export] private IdleState idleState;
	[Export] private VelocityComponent velocityComponent;

	public override void OnStateEnter()
	{
		velocityComponent.WishDirection = new Vector3(new RandomNumberGenerator().RandiRange(-1, 1), 0f,
			new RandomNumberGenerator().RandiRange(-1, 1)).Normalized();
	}

	public override void StateProcess(float delta)
	{
		randomTimer -= delta;
		
		if (randomTimer < 0f)
		{
			randomTimer = new RandomNumberGenerator().RandfRange(0f, 0.5f);
			NextState = idleState;
		}
	}

	public override void OnStateExit()
	{
		randomTimer = new RandomNumberGenerator().RandfRange(0f, 0.5f);
	}
}

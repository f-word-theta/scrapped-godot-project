using Godot;
using System;

public partial class IdleState : State
{
	[Export] private DirectionalSpriteComponent directionalSpriteComponent;
	[Export] private VelocityComponent velocityComponent;
	[Export] private DetectionAreaComponent detectionAreaComponent;
	[Export] private WanderState wanderState;
	[Export] private ChaseState chaseState;

	private bool playerFound = false;

	public override void OnStateEnter()
	{
		velocityComponent.WishDirection = Vector3.Zero;
		directionalSpriteComponent.RotationIsVelocityBased = false;
		directionalSpriteComponent.RandomizeRotation();
	}

	public override void StateProcess(float delta)
	{
		if (detectionAreaComponent.PlayerFound) NextState = chaseState;
		else
		{
			randomTimer -= delta;
			
			if (randomTimer < 0f)
			{
				randomTimer = new RandomNumberGenerator().RandfRange(0f, 2f);
				NextState = wanderState;
			}
		}
		
		
	}

	public override void OnStateExit()
	{
		randomTimer = new RandomNumberGenerator().RandfRange(0f, 2f);
		directionalSpriteComponent.RotationIsVelocityBased = true;
	}
	
}

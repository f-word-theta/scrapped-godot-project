using System;
using Godot;

public partial class ChaseState : State
{
    [Export] private DirectionalSpriteComponent directionalSpriteComponent;
    [Export] private DetectionAreaComponent detectionAreaComponent;
    [Export] private VelocityComponent velocityComponent;

    [Export] private IdleState idleState;

    private float timer = 1.0f;

    public override void StatePhysicsProcess(float delta)
    {
        if (!detectionAreaComponent.PlayerFound) NextState = idleState;
        else
        {
            timer -= delta;
            
            if (timer < 0f)
            {
                timer = 1.0f;
                velocityComponent.WishJump = Convert.ToBoolean(new RandomNumberGenerator().RandiRange(0, 1));
            }
            
            velocityComponent.WishDirection = (detectionAreaComponent.PlayerEntity.GlobalTransform.Origin - velocityComponent.CharacterBody.GlobalTransform.Origin).Normalized();
        }
    }

    public override void OnStateExit()
    {
        velocityComponent.WishJump = false;
    }

    public void Test() 
    {
    }
}
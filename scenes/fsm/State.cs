using Godot;
using System;

public partial class State : Node
{
	protected StateMachine stateMachine;
	protected float randomTimer = 1.0f;
	
	public State NextState = null;

	public override void _Ready()
	{
		stateMachine = GetParent<StateMachine>();
	}

	public virtual void StatePhysicsProcess(float delta)
	{
		
	}

	public virtual void StateProcess(float delta)
	{
		
	}

	public virtual void OnStateEnter()
	{
		
	}

	public virtual void OnStateExit()
	{
		
	}
}

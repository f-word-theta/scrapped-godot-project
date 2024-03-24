using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class StateMachine : Node
{
	[Export] private Label3D stateLabel;
	
	public List<State> States = new();
	[Export] public State CurrentState;
	public State PreviousState;

	[Signal] public delegate void StateChangedEventHandler();

	public override void _Ready()
	{
		foreach (Node node in GetChildren())
		{
			State state = node as State;
			States.Add(state);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		CurrentState.StatePhysicsProcess((float) delta);
	}

	public override void _Process(double delta)
	{
		stateLabel.Text = "State: " + CurrentState.Name;
		
		if (CurrentState.NextState is not null)
			SwitchStates(CurrentState.NextState);

		CurrentState.StateProcess((float) delta);
	}

	public void SwitchStates(State nextState)
	{
		if (CurrentState is not null)
		{
			CurrentState.OnStateExit();
			CurrentState.NextState = null;
		}

		CurrentState = nextState;
		CurrentState.OnStateEnter();
	}
}

using Godot;
using System;

public partial class Tick : Node
{
	public static float CurrentTick { get; set; }

	public override void _Process(double delta)
	{
		CurrentTick += (float) delta;
	}
}

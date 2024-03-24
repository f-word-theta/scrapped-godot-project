using Godot;
using System;

[GlobalClass]
public partial class DetectionAreaComponent : Area3D
{
	public PlayerEntity PlayerEntity;
	public bool PlayerFound = false;

	void OnBodyEntered(Node3D node3D)
	{
		if (node3D is PlayerEntity playerEntity)
		{
			PlayerFound = true;
			PlayerEntity = playerEntity;
		}
	}

	void OnBodyExited(Node3D node3D)
	{
		if (node3D is PlayerEntity playerEntity)
		{
			PlayerFound = false;
			PlayerEntity = null;
		}
	}
}

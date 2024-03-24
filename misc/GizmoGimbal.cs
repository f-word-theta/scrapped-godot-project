using Godot;
using System;

public partial class GizmoGimbal : Node3D
{
	[Export] private PlayerCameraManager _playerCameraManager;
	
	public override void _Process(double delta)
	{
		Rotation = Rotation with
		{
			X = Mathf.Lerp(Rotation.X, -_playerCameraManager.Cam.Rotation.X, 10f * (float) delta),
			Y = -_playerCameraManager.Gimbal.Rotation.Y,
		};
	}
}

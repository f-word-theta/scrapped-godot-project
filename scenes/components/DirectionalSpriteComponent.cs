using Godot;
using System;

[GlobalClass]
public partial class DirectionalSpriteComponent : AnimatedSprite3D
{
	[Export] public bool RotationIsVelocityBased = true;
	private Camera3D currentCamera;
	
	public CharacterBody3D CharacterBody;

	public override void _Ready()
	{
		CharacterBody = GetParent<CharacterBody3D>();
	}

	public override void _PhysicsProcess(double delta)
	{
		UpdateRotation();
	}
	
	public override void _Process(double delta)
	{
		UpdateAnimation();
	}

	//=====================================================================================================================

	private void UpdateAnimation()
	{
		var dotProduct = GlobalTransform.Basis.Z.Dot(-currentCamera.GlobalTransform.Basis.Z);
		var leftDotProduct = GlobalTransform.Basis.X.Dot(-currentCamera.GlobalTransform.Basis.Z);

		if (dotProduct >= 0.75f) Animation = "front";
		else if (dotProduct is < 0.75f and >= 0.25f) Animation = "diagonal_front";
		else if (dotProduct is < 0.25f and >= -0.25f) Animation = "side";
		else if (dotProduct is < -0.25f and >= -0.75f) Animation = "diagonal_back";
		else if (dotProduct < -0.75f) Animation = "back";

		FlipH = leftDotProduct < 0f;
		
		//GD.Print("GlobalTransform.Basis.Z  " + GlobalTransform.Basis.Z + "\n");
		//GD.Print("-currentCamera.GlobalTransform.Basis.Z " + -currentCamera.GlobalTransform.Basis.Z);
	}

	void UpdateRotation()
	{
		if (RotationIsVelocityBased)
			Rotation = Rotation with
			{
				Y = Mathf.Atan2(-CharacterBody.Velocity.X, -CharacterBody.Velocity.Z),
			};
	}

	public void RandomizeRotation()
	{
		if (!RotationIsVelocityBased)
			RotationDegrees = RotationDegrees with
			{
				Y = new RandomNumberGenerator().RandiRange(-360, 360),
			};
	}

	public void SetCamera(Camera3D cam)
	{
		currentCamera = cam;
	}
}

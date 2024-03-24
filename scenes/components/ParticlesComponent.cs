using Godot;
using System;

[GlobalClass]
public partial class ParticlesComponent : Node
{
	[Export] VelocityComponent velocityComponent { get; set; }
	[Export] public GpuParticles3D RunParticles { get; set; }
	[Export] public GpuParticles3D LandParticles { get; set; }
	
	
	public override void _Process(double delta)
	{
		RunParticles.Emitting = velocityComponent.ZeroYVelocity.Length() >= 0.5f && velocityComponent.CharacterBody.IsOnFloor();

		if (velocityComponent.JustLanded)
		{
			LandParticles.Restart();
			LandParticles.Emitting = true;
		}
	}
}
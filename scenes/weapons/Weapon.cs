using Godot;
using System;

[GlobalClass]
public partial class Weapon : Node3D
{
	[Export] WeaponStats weaponStats { get; set; }
	
	public virtual void PrimaryFire()
	{
		
	}

	public virtual void SecondaryFire()
	{
		
	}
}

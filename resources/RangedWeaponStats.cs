using Godot;

[GlobalClass]
public partial class RangedWeaponStats : WeaponStats
{
    [Export] public int MaxAmmo { get; set; }
    [Export] public float Range { get; set; }
    [Export] public float Damage { get; set; }

    // Empty constructor method is required for scripting resources in C#
    public RangedWeaponStats() : this(0, 0f, 0f) {}

    public RangedWeaponStats(int maxAmmo, float range, float damage)
    {
        MaxAmmo = maxAmmo;
        Range = range;
        Damage = damage;
    }
}

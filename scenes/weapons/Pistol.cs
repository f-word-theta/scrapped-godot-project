using Godot;

[GlobalClass]
public partial class Pistol : Weapon
{
	[Export] PlayerCameraManager playerCameraManager;
	[Export] AnimationPlayer animationPlayer;
	[Export] Sprite3D boomSprite;

    public override void _Ready()
    {
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public override void PrimaryFire()
    {
		animationPlayer.PlayFromBeginning("primary_fire");
    }

	public void Shake()
	{
		playerCameraManager.AddTrauma(0.7f);
	}

	public void RandomizeBoomSprite()
	{
		GD.Randomize();
		float randomSize = new RandomNumberGenerator().RandiRange(2, 4);

		if (new RandomNumberGenerator().RandiRange(0,1) == 1) boomSprite.FlipV = true;
		else boomSprite.FlipV = false;

		if (new RandomNumberGenerator().RandiRange(0,1) == 1) boomSprite.FlipH = true;
		else boomSprite.FlipH = false;

		boomSprite.Scale = new Vector3(randomSize, randomSize, randomSize);
	}
}
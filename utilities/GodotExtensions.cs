using Godot;

public static class GodotExtensions
{
    public static void PlayFromBeginning(this AnimationPlayer animationPlayer, string name, double customBlend = -1D, float customSpeed = 1F, bool fromEnd = false)
    {
        animationPlayer.Stop();
        animationPlayer.Play(name, customBlend, customSpeed, fromEnd);
    }

    public static void PlayFromBeginning(this AnimatedSprite3D animatedSprite, string name, float customSpeed = 1, bool fromEnd = false)
    {
        animatedSprite.Stop();
        animatedSprite.Play(name, customSpeed, fromEnd);
    }
}
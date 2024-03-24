using Godot;
using System;

public partial class UISubViewportContainer : SubViewportContainer
{
    Vector2 mouseRelative;

    public override void _Input(InputEvent @event)
    {
        if (Input.MouseMode != Input.MouseModeEnum.Captured || @event is not InputEventMouseMotion motion) return;
        mouseRelative += motion.Relative / new Vector2(150f, 400f);
    }

    public override void _Process(double delta)
    {
        mouseRelative = mouseRelative.Lerp(Vector2.Zero, 10f * (float) delta);
        Position = mouseRelative;
    }
}

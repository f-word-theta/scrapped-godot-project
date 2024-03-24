using Godot;
using System;

public partial class GizmoTextureRect : TextureRect
{
	[Export] private SubViewport subViewport;

	public override void _Ready()
	{
		Texture = subViewport.GetTexture();
	}
}

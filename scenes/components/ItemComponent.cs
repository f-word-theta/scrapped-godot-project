using Godot;
using System;

[GlobalClass]
public partial class ItemComponent : Node
{
    [Export] public MeshInstance3D Mesh { get; set; }
    [Export] public Material SelectedMaterial { get; set; }
    [Export] public Material DefaultMaterial { get; set; }

    public override void _Ready()
    {
        base._Ready();

        SetDefault();
    }

    //=====================================================================================================================

    public void SetSelected()
    {
        Mesh.MaterialOverlay = SelectedMaterial;
    }

    public void SetDefault()
    {
        Mesh.MaterialOverlay = DefaultMaterial;
    }
}

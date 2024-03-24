using Godot;

public partial class PauseScreenUI : CanvasLayer
{
	[Export] AnimationPlayer animationPlayer;
	[Export] ColorRect SelectionGraphic;
	[Export] VBoxContainer buttonList;

	public override void _UnhandledInput(InputEvent @event)
	{
		if (Input.IsActionJustPressed("escape"))
		{
			GetTree().Paused = !GetTree().Paused;

			if (Input.MouseMode == Input.MouseModeEnum.Visible) Input.MouseMode = Input.MouseModeEnum.Captured;
			else if (Input.MouseMode == Input.MouseModeEnum.Captured) Input.MouseMode = Input.MouseModeEnum.Visible;
		}
	}

	public override void _Ready()
	{
		foreach (Button button in buttonList.GetChildren())
		{
			button.MouseEntered += Handle;
		}
	}

	void Handle()
	{
		
	}

	
	public override void _Process(double delta)
	{
		
	}
}

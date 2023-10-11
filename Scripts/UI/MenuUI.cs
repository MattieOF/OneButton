using Godot;
using System;

public partial class MenuUI : Control
{
	[Export] public Label[] buttons = new Label[3];
	[Export] public Label pointer;
	[Export] public Path2D path;
	[Export] public PackedScene gameScene;

	private double _selection = 0;
	
	public override void _Process(double delta)
	{
		_selection += delta / 8;
		_selection %= 1;
		pointer.GlobalPosition = path.GlobalPosition + path.Curve.Samplef((float) (_selection * 2)) - new Vector2(0, 35);
		
		int selected = ((int) Math.Floor(_selection * 6));
		if (selected >= 3)
			selected = 6 - selected - 1;
		
		for (int i = 0; i < 3; i++)
		{
			Color currentColor = buttons[i].GetThemeColor("font_color");
			currentColor.V += (float) Math.Min(delta, (i == selected ? (1 - currentColor.V) : (0.6 - currentColor.V)));
			buttons[i].AddThemeColorOverride("font_color", currentColor);
		}

		if (Input.IsActionJustPressed("the_input"))
		{
			switch (selected)
			{
				case 0:
					GetTree().ChangeSceneToPacked(gameScene);
					break;
				case 1:
					break;
				case 2:
					GetTree().Quit();
					break;
			}
		}
	}
}

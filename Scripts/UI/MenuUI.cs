using Godot;
using System;

public partial class MenuUI : Control
{
	[Export] public Label[] buttons = new Label[3];
	[Export] public Label pointer;
	[Export] public Path2D path;
	[Export] public PackedScene gameScene;
	[Export] public RichTextLabel helpText;
	[Export] public Sprite2D cat;
	[Export] public Vector2 catStart, catEnd;

	private double _selection = 0;
	private bool _helpOpen = false;
	
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
			currentColor.V = (float) Mathf.MoveToward(currentColor.V, i == selected && !_helpOpen ? 0 : 0.6f, delta);
			buttons[i].AddThemeColorOverride("font_color", currentColor);
		}

		if (Input.IsActionJustPressed("the_input") && !Console.Instance.Open)
		{
			if (_helpOpen)
			{
				var tween = helpText.CreateTween();
				tween.TweenProperty(helpText, new NodePath(Control.PropertyName.Position), Variant.From(new Vector2(1200, helpText.Position.Y)), .5f).SetEase(Tween.EaseType.Out);
				tween.TweenProperty(cat, new NodePath(Sprite2D.PropertyName.Position), Variant.From(catStart), .5f).SetEase(Tween.EaseType.Out);
				_helpOpen = false;
				pointer.Visible = true;
			}
			else
			{
				switch (selected)
				{
					case 0:
						GetTree().ChangeSceneToPacked(gameScene);
						break;
					case 1:
						_helpOpen = true;
						var tween = helpText.CreateTween();
						tween.TweenProperty(cat, new NodePath(Sprite2D.PropertyName.Position), Variant.From(catEnd), .5f).SetEase(Tween.EaseType.Out);
						tween.TweenProperty(helpText, new NodePath(Control.PropertyName.Position), Variant.From(new Vector2(700, helpText.Position.Y)), .5f).SetEase(Tween.EaseType.Out);
						pointer.Visible = false;
						break;
					case 2:
						GetTree().Quit();
						break;
				}
			}
		}
	}
}

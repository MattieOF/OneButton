using Godot;
using System;

public partial class HUD : CanvasLayer
{
	[Export] public Label scoreText;
	
	public override void _Process(double delta)
	{
		scoreText.Text = (Convert.ToDouble(scoreText.Text) + delta).ToString("F2");
	}
}

using Godot;
using System;

public partial class HUD : CanvasLayer
{
	[Export] public Label scoreText, streakText;

	private Tween _streakTween;
	
	public override void _Process(double delta)
	{
		scoreText.Text = (Convert.ToDouble(scoreText.Text) + delta).ToString("F2");
	}

	public void SetStreak(int newStreak)
	{
		streakText.Text = $"x{newStreak}";
		if (_streakTween is not null && _streakTween.IsRunning())
			_streakTween.Stop();
		_streakTween = streakText.CreateTween();
		if (newStreak == 0)
		{
			_streakTween.TweenMethod(Callable.From<Color>(col => streakText.AddThemeColorOverride("font_color", col)), Variant.From(Colors.Red), Variant.From(Colors.White), 0.4f);
			_streakTween.TweenMethod(Callable.From<int>(size => streakText.AddThemeFontSizeOverride("font_size", size)), Variant.From(40), Variant.From(25), 0.4f);
			// _streakTween.TweenMethod(Callable.From<int>(size => streakText.AddThemeFontSizeOverride("font_size", size)), Variant.From(40), Variant.From(25), 0.2f).SetDelay(0.2f);
			// tween.TweenProperty(streakText, new NodePath(Control.PropertyName.Scale), Variant.From(Vector2.One), 0.4f);
		}
		else
		{
			_streakTween.TweenMethod(Callable.From<int>(size => streakText.AddThemeFontSizeOverride("font_size", size)), Variant.From(25), Variant.From(40), 0.1f);
			_streakTween.TweenMethod(Callable.From<int>(size => streakText.AddThemeFontSizeOverride("font_size", size)), Variant.From(40), Variant.From(25), 0.1f).SetDelay(0.1f);
			// tween.TweenProperty(streakText, new NodePath(Control.PropertyName.Scale), Variant.From(Vector2.One * 1.5f), 0.1f);
			// tween.TweenProperty(streakText, new NodePath(Control.PropertyName.Scale), Variant.From(Vector2.One), 0.1f).SetDelay(0.1f);
		}
	}
}

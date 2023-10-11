using Godot;

public partial class HUD : CanvasLayer
{
	[Export] public Label scoreText, addedScoreText, streakText;

	private Tween _streakTween, _addedScoreTween;
	private int _addedScore;
	
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
			streakText.AddThemeColorOverride("font_color", Colors.White);
			_streakTween.TweenMethod(Callable.From<int>(size => streakText.AddThemeFontSizeOverride("font_size", size)), Variant.From(25), Variant.From(40), 0.1f);
			_streakTween.TweenMethod(Callable.From<int>(size => streakText.AddThemeFontSizeOverride("font_size", size)), Variant.From(40), Variant.From(25), 0.1f).SetDelay(0.1f);
			// tween.TweenProperty(streakText, new NodePath(Control.PropertyName.Scale), Variant.From(Vector2.One * 1.5f), 0.1f);
			// tween.TweenProperty(streakText, new NodePath(Control.PropertyName.Scale), Variant.From(Vector2.One), 0.1f).SetDelay(0.1f);
		}
	}

	public void AddScore(int newScore, int finalScore)
	{
		scoreText.Text = finalScore.ToString();
		_addedScore += newScore;
		
		if (_addedScoreTween is not null && _addedScoreTween.IsRunning())
			_addedScoreTween.Stop();

		addedScoreText.Text = $"+{_addedScore}";
		addedScoreText.AddThemeColorOverride("font_color", Colors.White);
		_addedScoreTween = addedScoreText.CreateTween();
		_addedScoreTween
			.TweenMethod(
				Callable.From<float>(alpha => addedScoreText.AddThemeColorOverride("font_color", new Color(1, 1, 1, alpha))),
				Variant.From(1.0f), Variant.From(0.0f), 1).SetDelay(1f);
		_addedScoreTween.TweenCallback(Callable.From(() =>
		{
			_addedScore = 0;
		})).SetDelay(2f);
	}
}

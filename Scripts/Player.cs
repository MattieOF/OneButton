using Godot;

public partial class Player : Node2D
{
	[Export] public AnimatedSprite2D sprite;

	private Tween _flipTween;
	private bool _flipped = false;
	private Vector2 _defaultScale;

	public override void _Ready()
	{
		_defaultScale = sprite.Scale;
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("the_input"))
			Flip();
	}

	public void Flip()
	{
		if (_flipTween is not null)
			_flipTween.Stop();
		_flipTween = sprite.CreateTween();
		_flipTween.TweenProperty(sprite, new NodePath(Node2D.PropertyName.Scale),
			Variant.From(new Vector2(_flipped ? _defaultScale.X : -_defaultScale.X, _defaultScale.Y)), 0.15f);
		_flipped = !_flipped;
	}
}

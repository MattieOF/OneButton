using Godot;

public partial class DamageNumber : Label
{
	public Vector2 velocity;
	public float angularVelocity;
	public Color color = Colors.White;

	public override void _Ready()
	{
		var tween = CreateTween();
		tween.TweenMethod(Callable.From<float>(alpha =>
		{
			color.A = alpha;
			AddThemeColorOverride("font_color", color);
		}), Variant.From(1.0f), Variant.From(0.0f), 1.5f);
		tween.TweenCallback(Callable.From(QueueFree)).SetDelay(1.5f);
	}

	public override void _Process(double delta)
	{
		GlobalPosition += (velocity * (float) delta);
		RotationDegrees += angularVelocity * (float) delta;
		velocity -= velocity * (float) delta * 0.5f;
		angularVelocity -= angularVelocity * (float) delta * 0.5f;
	}
}

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
		
		Commands.Instance.AddCommand("draw_raycasts", args =>
		{
			// TODO: Doesn't seem to work?
			Line2D line = new();
			line.AddPoint(Transform.Origin + Vector2.Left * 1000);
			line.AddPoint(Transform.Origin);
			line.AddPoint(Transform.Origin + Vector2.Right * 1000);
			line.DefaultColor = Colors.Red;
			line.Width = 20;
			AddChild(line);
			line.CreateTween().TweenCallback(Callable.From(line.QueueFree)).SetDelay(3);
			return true;
		});
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("the_input"))
			Attack();
	}

	public void Attack()
	{
		// Cast rays to the left and to the right of the player to check for any enemies
		var spaceState = GetWorld2D().DirectSpaceState;
		var query = PhysicsRayQueryParameters2D.Create(Transform.Origin, Transform.Origin + Vector2.Left * 1000);
		query.HitFromInside = false; // Should prevent hitting the player itself
		query.CollideWithAreas = true;
		var left = spaceState.IntersectRay(query);
		query = PhysicsRayQueryParameters2D.Create(Transform.Origin, Transform.Origin + Vector2.Right * 1000);
		query.CollideWithAreas = true;
		query.HitFromInside = false;
		var right = spaceState.IntersectRay(query);

		float leftDist = float.MaxValue, rightDist = float.MaxValue;
		if (left.ContainsKey("position"))
			leftDist = Transform.Origin.DistanceSquaredTo(left["position"].AsVector2());
		if (right.ContainsKey("position"))
			rightDist = Transform.Origin.DistanceSquaredTo(right["position"].AsVector2());
		Console.Instance.WriteLine($"Left: {leftDist}, right: {rightDist}");

		// Tolerance is irrelevant here because we're just testing to see if both missed,
		// in which case, both will equal float.MaxValue.
		// ReSharper disable once CompareOfFloatsByEqualityOperator
		if (leftDist == rightDist)
			Flip();
		else if (leftDist < rightDist)
		{
			if (_flipped)
				Flip();	
		}
		else if (leftDist > rightDist)
		{
			if (!_flipped)
				Flip();	
		}
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

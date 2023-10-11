using Godot;

// ReSharper disable CompareOfFloatsByEqualityOperator

public partial class Player : Node2D
{
	[Export] public AnimatedSprite2D sprite;
	[Export] public StaticBody2D body;
	[Export] public CollisionShape2D shape;

	[Export] public float attackRange = 200;

	private Tween _flipTween;
	private bool _flipped = false;
	private Vector2 _defaultScale;
	private AudioStreamPlayer2D _flipSoundPlayer;

	public override void _Ready()
	{
		_defaultScale = sprite.Scale;

		_flipSoundPlayer = GetNode<AudioStreamPlayer2D>("FlipSound");
		
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
		var query = PhysicsRayQueryParameters2D.Create(Transform.Origin, Transform.Origin + Vector2.Left * attackRange);
		query.HitFromInside = true;
		var exclusions = query.Exclude;
		exclusions.Add(body.GetRid()); // Avoid hitting the player itself
		query.Exclude = exclusions;
		var left = spaceState.IntersectRay(query);
		query.To = Transform.Origin + Vector2.Right * attackRange;
		var right = spaceState.IntersectRay(query);

		// Calculate the distance to the nearest enemy either side of the player
		// Distance will equal float.MaxValue if there is no enemy in range
		float leftDist = float.MaxValue, rightDist = float.MaxValue;
		if (left.ContainsKey("position"))
			leftDist = GlobalPosition.DistanceSquaredTo(((StaticBody2D)left["collider"].AsGodotObject()).GlobalPosition);
		if (right.ContainsKey("position"))
			rightDist = GlobalPosition.DistanceSquaredTo(((StaticBody2D)right["collider"].AsGodotObject()).GlobalPosition);
		Console.Instance.WriteLine($"Left: {leftDist}, right: {rightDist}");

		StaticBody2D closest = null;
		if (leftDist == rightDist && left.ContainsKey("position"))
			closest = ((StaticBody2D) left["collider"].AsGodotObject());
		else if (leftDist < rightDist)
			closest = ((StaticBody2D) left["collider"].AsGodotObject());
		else if (rightDist < leftDist)
			closest = ((StaticBody2D) right["collider"].AsGodotObject());
		
		// Tolerance is irrelevant here because we're just testing to see if both missed,
		// in which case, both will equal float.MaxValue.
		if (leftDist == float.MaxValue && rightDist == float.MaxValue)
			Flip();
		else if (closest.GlobalPosition.X < GlobalPosition.X)
		{
			if (_flipped)
				Flip();
		}
		else if (closest.GlobalPosition.X > GlobalPosition.X)
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
		_flipSoundPlayer.Play();
		_flipped = !_flipped;
	}
}

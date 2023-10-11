using System.Globalization;
using Godot;

// ReSharper disable CompareOfFloatsByEqualityOperator

public partial class Player : Node2D
{
	[Export] public AnimatedSprite2D sprite;
	[Export] public StaticBody2D body;
	[Export] public CollisionShape2D shape;
	[Export] public ProgressBar powerBar, healthBar;
	[Export] public GameManager gameManager;

	[Export] public AudioStream[] punchSounds = new AudioStream[3];
	[Export] public PackedScene damageNumber;

	[Export] public float attackRange = 200;
	[Export] public float maxHp = 100;

	private Tween _flipTween;
	private bool _flipped = false;
	private Vector2 _defaultScale;
	private AudioStreamPlayer2D _flipSoundPlayer;
	private float _power = 1;
	private float _hp;
	private float _baseDmg = 50;

	public override void _Ready()
	{
		_defaultScale = sprite.Scale;
		_hp = maxHp;

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
	
	public void Damage(float dmg)
	{
		_hp -= dmg;
		if (_hp <= 0)
		{
			// Player died!
			gameManager.PlayerDied();
		}
		
		var dmgNumber = damageNumber.Instantiate() as DamageNumber;
		dmgNumber!.velocity = new Vector2(Utility.RNG.RandfRange(-60, 60), Utility.RNG.RandfRange(-60, -20));
		dmgNumber.angularVelocity = Utility.RNG.RandfRange(-80, 80);
		dmgNumber.color = Colors.Red;
		dmgNumber.Text = dmg.ToString("F1");
		AddChild(dmgNumber);
		dmgNumber.GlobalPosition = GlobalPosition;

		var hpPercentage = _hp / maxHp;
		healthBar.Value = hpPercentage * 100;
		var stylebox = healthBar.GetThemeStylebox("fill") as StyleBoxFlat;
		stylebox!.BgColor = Colors.Red.Lerp(Colors.Green, hpPercentage);
		healthBar.AddThemeStyleboxOverride("fill", stylebox);
	}

	public override void _Process(double delta)
	{
		if (_power < 1)
		{
			_power += (float) delta * 3;
			_power = Mathf.Min(1, _power);
			
			// Update UI
			powerBar.Value = _power * 100;
			var stylebox = powerBar.GetThemeStylebox("fill") as StyleBoxFlat;
			stylebox!.BgColor = Colors.Red.Lerp(Colors.Green, _power);
			powerBar.AddThemeStyleboxOverride("fill", stylebox);
		}
		
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
		// Console.Instance.WriteLine($"Left: {leftDist}, right: {rightDist}");

		StaticBody2D closest = null;
		if (leftDist == rightDist && left.ContainsKey("position"))
			closest = ((StaticBody2D) left["collider"].AsGodotObject());
		else if (leftDist < rightDist)
			closest = ((StaticBody2D) left["collider"].AsGodotObject());
		else if (rightDist < leftDist)
			closest = ((StaticBody2D) right["collider"].AsGodotObject());
		
		// Next, see if we need to flip over to face the enemy
		// Tolerance is irrelevant here because we're just testing to see if both missed,
		// in which case, both will equal float.MaxValue.
		if (leftDist == float.MaxValue && rightDist == float.MaxValue)
			Flip();
		else if (closest!.GlobalPosition.X < GlobalPosition.X)
		{
			if (_flipped)
				Flip();
		}
		else if (closest.GlobalPosition.X > GlobalPosition.X)
		{
			if (!_flipped)
				Flip();	
		}

		// Actually attack the nearest enemy
		if (closest is not null)
		{
			if (_power >= 0.95f)
				gameManager.IncrementStreak();
			else
				gameManager.ResetStreak();
			
			var enemy = closest.GetNode("../..") as Enemy;
			var dmg = Utility.EaseInExpo(_power, 15) * (_baseDmg + Utility.RNG.RandfRange(-10, 10));
			enemy!.Damage(dmg);
			
			var dmgNumber = damageNumber.Instantiate() as DamageNumber;
			dmgNumber!.velocity = new Vector2(Utility.RNG.RandfRange(-60, 60), Utility.RNG.RandfRange(-60, -20));
			dmgNumber.angularVelocity = Utility.RNG.RandfRange(-80, 80);
			dmgNumber.Text = dmg.ToString("F1");
			AddChild(dmgNumber);
			dmgNumber.GlobalPosition = ((enemy.GlobalPosition - GlobalPosition) / 2) + GlobalPosition;
			
			this.PlaySound2D(Utility.ChooseRandom(punchSounds));
			_power = 0;
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

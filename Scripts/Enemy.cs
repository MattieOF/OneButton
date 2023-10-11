using Godot;

public partial class Enemy : Node2D
{
	[Export] public ProgressBar healthBar;
	[Export] public RayCast2D attackRay;
	
	[Export] public AudioStream[] punchSounds = new AudioStream[3]; 
	
	[Export] public Vector2 speedRange = new(150, 250);
	[Export] public Vector2 hpRange = new(150, 250);
	[Export] public Vector2 attackRange = new(70, 130);
	[Export] public Vector2 attackCooldown = new(0.8f, 1.2f);

	private Vector2 _dir = Vector2.Right;
	private float _speed;
	private float _attackRange;
	private float _hp, _maxHp;
	private float _attackCooldown;

	public float MaxHP => _maxHp;
	
	public override void _Ready()
	{
		base._Ready();
		var rng = new RandomNumberGenerator();
		_speed = rng.RandfRange(speedRange.X, speedRange.Y);
		_attackRange = rng.RandfRange(attackRange.X, attackRange.Y);
		_maxHp = rng.RandfRange(hpRange.X, hpRange.Y);
		_hp = _maxHp;
		_attackCooldown = attackCooldown.Y;

		var hpBarStylebox = new StyleBoxFlat();
		hpBarStylebox.BgColor = Colors.Green;
		hpBarStylebox.CornerRadiusBottomLeft = 5;
		hpBarStylebox.CornerRadiusBottomRight = 5;
		hpBarStylebox.CornerRadiusTopLeft = 5;
		hpBarStylebox.CornerRadiusTopRight = 5;
		healthBar.AddThemeStyleboxOverride("fill", hpBarStylebox);
	}

	public void SetHPRange(Vector2 newHpRange, bool resetHp = true)
	{
		hpRange = newHpRange;
		if (!resetHp) return;
		_maxHp = Utility.RNG.RandfRange(hpRange.X, hpRange.Y);
		_hp = _maxHp;
	}

	public override void _Process(double delta)
	{
		if (!attackRay.IsColliding())
		{
			var transform = Transform;
			transform.Origin += _dir * _speed * (float) delta;
			Transform = transform;
		}

		if (attackRay.IsColliding())
		{
			_attackCooldown = (float) Mathf.Max(0, _attackCooldown - delta);
			if (_attackCooldown <= 0)
			{
				var player = (attackRay.GetCollider() as StaticBody2D)!.FindParent("Player") as Player;
				this.PlaySound2D(Utility.ChooseRandom(punchSounds));
				player!.Damage(10);
				_attackCooldown = Utility.RNG.RandfRange(attackCooldown.X, attackCooldown.Y);
			}
		}
	}

	public void SetDir(Vector2 newDir)
	{
		_dir = newDir;
		attackRay.TargetPosition = newDir * _attackRange;
	}

	public bool Damage(float dmg)
	{
		_hp -= dmg;

		var hpPercentage = _hp / _maxHp;
		healthBar.Value = hpPercentage * 100;
		var stylebox = healthBar.GetThemeStylebox("fill") as StyleBoxFlat;
		stylebox!.BgColor = Colors.Red.Lerp(Colors.Green, hpPercentage);
		healthBar.AddThemeStyleboxOverride("fill", stylebox);

		if (_hp <= 0)
		{
			QueueFree();
			return true;
		}
		else return false;
	}
}

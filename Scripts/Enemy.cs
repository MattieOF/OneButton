using Godot;

public partial class Enemy : Node2D
{
	[Export] public ProgressBar healthBar;
	
	[Export] public Vector2 speedRange = new(150, 250);
	[Export] public Vector2 hpRange = new(150, 250);

	public Vector2 dir = Vector2.Right;
	
	private float _speed;
	private float _hp, _maxHp;
	
	public override void _Ready()
	{
		base._Ready();
		var rng = new RandomNumberGenerator();
		_speed = rng.RandfRange(speedRange.X, speedRange.Y);
		_maxHp = rng.RandfRange(hpRange.X, hpRange.Y);
		_hp = _maxHp;
	}

	public override void _Process(double delta)
	{
		var transform = Transform;
		transform.Origin += dir * _speed * (float) delta;
		Transform = transform;
	}

	public void Damage(float dmg)
	{
		_hp -= dmg;
		if (_hp <= 0)
			QueueFree();

		var hpPercentage = _hp / _maxHp;
		healthBar.Value = hpPercentage * 100;
		var stylebox = healthBar.GetThemeStylebox("fill") as StyleBoxFlat;
		stylebox!.BgColor = Colors.Red.Lerp(Colors.Green, hpPercentage);
		healthBar.AddThemeStyleboxOverride("fill", stylebox);
	}
}

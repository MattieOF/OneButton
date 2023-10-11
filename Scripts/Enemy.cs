using Godot;

public partial class Enemy : Node2D
{
	[Export] public Vector2 SpeedRange = new(150, 250);

	private float _speed;
	private Vector2 _dir = Vector2.Right;
	
	public override void _Ready()
	{
		base._Ready();
		var rng = new RandomNumberGenerator();
		_speed = rng.RandfRange(SpeedRange.X, SpeedRange.Y);
	}

	public override void _Process(double delta)
	{
		var transform = Transform;
		transform.Origin += _dir * _speed * (float) delta;
		Transform = transform;
	}
}

using Godot;
using Godot.Collections;

public partial class GameManager : Node2D
{
	[Export] public Node2D spawnPoints;
	[Export] public Array<PackedScene> enemies;
	[Export] public HUD hud;

	private int _level, _streak;
	private Timer _spawnTimer = new();
	
	public override void _Ready()
	{
		AddChild(_spawnTimer);
		_spawnTimer.OneShot = false;
		_spawnTimer.WaitTime = 6;
		_spawnTimer.Timeout += Spawn;
		_spawnTimer.Start();
	}

	public void Spawn()
	{
		var enemy = enemies.PickRandom();
		var instantiatedEnemy = enemy.Instantiate() as Enemy;
		AddChild(instantiatedEnemy);
		var spawnPoint = (Node2D) spawnPoints.GetChildren().PickRandom();
		instantiatedEnemy!.GlobalPosition = spawnPoint.GlobalPosition;
		instantiatedEnemy.SetDir(spawnPoint.Transform.BasisXform(Vector2.Right));
	}

	public void PlayerDied()
	{
		
	}

	public void IncrementStreak()
	{
		_streak++;
		hud.SetStreak(_streak);
	}

	public void ResetStreak()
	{
		_streak = 0;
		hud.SetStreak(_streak);
	}
}

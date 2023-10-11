using Godot;
using Godot.Collections;

public partial class GameManager : Node2D
{
	[Export] public Node2D spawnPoints;
	[Export] public Array<PackedScene> enemies;
	[Export] public PackedScene timingChallenge, gameOverScene;
	[Export] public HUD hud;
	[Export] public Vector2 hpRange = new(150, 200);
	[Export] public float hpIncreasePerSpawn = 10;
	[Export] public Player player;
	[Export] public AudioStream successSound, failureSound;

	private int _level, _streak, _score;
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
		instantiatedEnemy!.SetHPRange(hpRange, true);
		AddChild(instantiatedEnemy);
		var spawnPoint = (Node2D) spawnPoints.GetChildren().PickRandom();
		instantiatedEnemy.GlobalPosition = spawnPoint.GlobalPosition;
		instantiatedEnemy.SetDir(spawnPoint.Transform.BasisXform(Vector2.Right));
		hpRange.X += hpIncreasePerSpawn;
		hpRange.Y += hpIncreasePerSpawn;
	}

	public void PlayerDied()
	{
		var globals = GetNode<Globals>("/root/Globals");
		globals.StopMusic();
		globals.newScore = _score;
		GetTree().ChangeSceneToPacked(gameOverScene);
	}

	public void IncrementStreak()
	{
		_streak++;
		hud.SetStreak(_streak);

		if (_streak != 0 && _streak % 15 == 0)
		{
			var challenge = timingChallenge.Instantiate() as TimingChallenge;
			AddChild(challenge);
			challenge!.Completed += success =>
			{
				if (success)
				{
					player.AddBaseDamage(30);
					player.Damage(-10); // Heal player for 10
				}
				this.PlaySound2D(success ? successSound : failureSound);
			};
		}
	}

	public void ResetStreak()
	{
		_streak = 0;
		hud.SetStreak(_streak);
	}

	public int GetStreak() => _streak;

	public void AddScore(int score)
	{
		_score += score;
		hud.AddScore(score, _score);
	}
}

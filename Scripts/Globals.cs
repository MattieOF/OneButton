using Godot;

public partial class Globals : Node
{
	[Export] public LoopingAudioStreamPlayer music;
	
	public int newScore = 0;

	private Tween _musicTween;

	public void StopMusic()
	{
		if (_musicTween is not null && _musicTween.IsRunning())
			_musicTween.Stop();

		_musicTween = music.CreateTween();
		_musicTween.TweenProperty(music, new NodePath(AudioStreamPlayer.PropertyName.PitchScale), 0.1, 0.5f);
	}

	public void StartMusic()
	{
		if (_musicTween is not null && _musicTween.IsRunning())
			_musicTween.Stop();

		_musicTween = music.CreateTween();
		_musicTween.TweenProperty(music, new NodePath(AudioStreamPlayer.PropertyName.PitchScale), 1, 0.5f);
	}
}

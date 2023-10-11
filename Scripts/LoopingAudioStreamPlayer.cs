using Godot;

public partial class LoopingAudioStreamPlayer : AudioStreamPlayer
{
	public override void _Ready()
	{
		Finished += () => Play();
	}
}

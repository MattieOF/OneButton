using Godot;

public static class Utility
{
    private static readonly RandomNumberGenerator RNG = new();
    
    public static AudioStream ChooseRandom(AudioStream[] streams) => streams[RNG.RandiRange(0, streams.Length - 1)];

    public static void PlaySound2D(this Node2D node, AudioStream stream)
    {
        AudioStreamPlayer2D punchPlayer = new AudioStreamPlayer2D();
        node.AddChild(punchPlayer);
        punchPlayer.Stream = stream;
        punchPlayer.Play();
        punchPlayer.Finished += () => punchPlayer.QueueFree();
    }
}

using Godot;

public class Utility
{
    private static RandomNumberGenerator _rng = new();
    
    public static AudioStream ChooseRandom(AudioStream[] streams) => streams[_rng.RandiRange(0, streams.Length - 1)];
}

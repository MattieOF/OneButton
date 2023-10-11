using Godot;

public partial class TimingChallenge : CanvasLayer
{
	[Export] public ColorRect theMark;
	[Export] public ColorRect cursor;
	[Export] public float markWidth = 50, cursorWidth = 7, fullWidth = 500;
	
	private float _cursorPos = 0, _markPos;

	[Signal]
	public delegate void CompletedEventHandler(bool success);
	
	public override void _Ready()
	{
		RandomiseMark();
	}

	public void RandomiseMark()
	{
		_markPos = Utility.RNG.Randf();
		var markPos = theMark.Position;
		markPos.X = (fullWidth - markWidth) * _markPos;
		theMark.Position = markPos;
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("the_input"))
		{
			if (cursor.Position.X > theMark.Position.X && cursor.Position.X < theMark.Position.X + markWidth)
				EmitSignal(SignalName.Completed, true);
			else
				EmitSignal(SignalName.Completed, false);
			QueueFree();
		}
		
		_cursorPos += (float) delta / 2.5f;
		_cursorPos %= 1;
		
		var pos = _cursorPos * 2;
		var cursorPos = cursor.Position;
		cursorPos.X = pos < 1
			? (fullWidth - cursorWidth) * pos
			: (fullWidth - cursorWidth) - ((fullWidth - cursorWidth) * (pos - 1));
		cursor.Position = cursorPos;
	}
}

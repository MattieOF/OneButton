using Godot;

public partial class Console : Window
{
	[Export] public RichTextLabel output;
	[Export] public LineEdit inputBox;
	
	private bool _open = false;
	
	public override void _Ready()
	{
		CloseRequested += () => SetOpen(false);
		inputBox.TextSubmitted += CommandEntered;
		Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("toggle_console"))
			SetOpen(!_open);
	}

	public void SetOpen(bool open)
	{
		if (!open)
			inputBox.ReleaseFocus();
		
		_open = open;
		Visible = _open;
		
		if (open)
			inputBox.GrabFocus();
	}

	public void WriteLine(string line)
	{
		output.Text += $"{line}\n";
		GD.Print($"Current: {output.GetVScrollBar().Value}, max: {output.GetVScrollBar().MaxValue}, min: {output.GetVScrollBar().MinValue}, page: {output.GetVScrollBar().Page}");
		if ((output.GetVScrollBar().MaxValue - output.GetVScrollBar().Value) - 10 <= output.GetVScrollBar().Page)
		{
			output.GetVScrollBar().Value = output.GetVScrollBar().MaxValue;
			GD.Print("Scroll!");
		}
	}

	public void CommandEntered(string command)
	{
		command = command.Trim();
		switch (command)
		{
			case "exit":
				GetTree().Quit();
				break;
			case "clear":
				output.Text = "";
				output.Clear();
				break;
			default:
				WriteLine($"Unknown command: \"{command}\"");
				break;
		}

		inputBox.Clear();
	}
}

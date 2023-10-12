using Godot;
using System;

public partial class GameOverUI : CanvasLayer
{
    [Export] public Label[] buttons = new Label[2];
    [Export] public Label pointer;
    [Export] public Label scoreText;
    [Export] public Path2D path;
    [Export] public string gameScene, menuScene;

    private double _selection = 0;

    public override void _Ready()
    {
        var highscore = FileAccess.Open("user://highscore.save",
            FileAccess.FileExists("user://highscore.save")
                ? FileAccess.ModeFlags.Read
                : FileAccess.ModeFlags.WriteRead);

        var globals = GetNode<Globals>("/root/Globals");
        
        var contents = highscore.GetAsText();
        if (string.IsNullOrWhiteSpace(contents))
            contents = "0";
        bool isNewHighscore = false;
        if (int.TryParse(contents, out int old))
            isNewHighscore = globals.newScore > old;
        
        scoreText.Text = $"Highscore: {contents}\nFinal score: {globals.newScore}{(isNewHighscore ? " (New highscore!)" : "")}";
        
        highscore.Close();
        highscore = FileAccess.Open("user://highscore.save", FileAccess.ModeFlags.Write);
        highscore.StoreString(globals.newScore.ToString());
        highscore.Close();
    }

    public override void _Process(double delta)
    {
        _selection += delta / 8;
        _selection %= 1;
        pointer.GlobalPosition = path.GlobalPosition + path.Curve.Samplef((float) (_selection * 2)) - new Vector2(0, 35);
		
        int selected = ((int) Math.Floor(_selection * 4));
        if (selected >= 2)
            selected = 4 - selected - 1;
		
        for (int i = 0; i < 2; i++)
        {
            Color currentColor = buttons[i].GetThemeColor("font_color");
            currentColor.V += (float) Math.Min(delta, (i == selected ? (1 - currentColor.V) : (0.6 - currentColor.V)));
            buttons[i].AddThemeColorOverride("font_color", currentColor);
        }

        if (Input.IsActionJustPressed("the_input"))
        {
            var globals = GetNode<Globals>("/root/Globals");
            globals.StartMusic();
            switch (selected)
            {
                case 0:
                    GetTree().ChangeSceneToFile(gameScene);
                    break;
                case 1:
                    GetTree().ChangeSceneToFile(menuScene);
                    break;
            }
        }
    }
}
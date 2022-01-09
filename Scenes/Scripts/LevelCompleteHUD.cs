using Godot;
using System;

public class LevelCompleteHUD : CanvasLayer
{
    private World world;

    public World World
    {
        get
        {
            if (world == null)
            {
                world = (World)GetTree().Root.GetNode("Node2D");
            }
            return world;
        }
    }

    public void setScoreText(int score)
    {
        Label scoreLabel = (Label)GetNode("Score/Label");
        scoreLabel.Text = "Score:" + score.ToString();
    }
    public void loadNextLevel()
    {
        try
        {
            GetTree().ChangeScene(World.NextLevelResource);
            GetTree().Paused = false;
        }
        catch (Exception ex)
        {
            GD.Print("Cannot go to next level because:\n\n", ex, "\n\n");
            loadMainMenu();
        }
    }

    public void loadMainMenu()
    {
           GetTree().ChangeScene("res://Scenes/MainMenu.tscn");
    }

    public void resetGame()
    {

    }

//     // Called when the node enters the scene tree for the first time.
//     public override void _Ready()
//     {
        
//     }

// //  // Called every frame. 'delta' is the elapsed time since the previous frame.
// //  public override void _Process(float delta)
// //  {
// //      
// //  }
}

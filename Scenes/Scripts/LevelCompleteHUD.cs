using Godot;
using System;

public class LevelCompleteHUD : CanvasLayer
{
    public void setScoreText(int score)
    {
        Label scoreLabel = (Label)GetNode("Score/Label");
        scoreLabel.Text = "Score:" + score.ToString();
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

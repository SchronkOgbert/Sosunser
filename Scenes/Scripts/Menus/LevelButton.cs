using Godot;
using System;

public class LevelButton : Godot.Button
{
    private Label textLabel;
    private AnimationPlayer animationPlayer;
    private string _levelPath = "";

    private void getLabelReference()
    {
        textLabel = (Label)GetNode("Label");
    }

    private void getAnimationPlayerReference()
    {
        animationPlayer = (AnimationPlayer)GetNode("AnimationPlayer");
    }

    [Export]
    public string labelText
    {
        get 
        {
            try
            {
                return textLabel.Text;
            }
            catch (NullReferenceException e)
            {
                GD.Print(e);
                getLabelReference();
                return textLabel.Text;
            }
        }
        set
        {
            try
            {
                textLabel.Text = value;
            }
            catch (NullReferenceException e)
            {
                GD.Print(e);
                getLabelReference();
                textLabel.Text = value;
            }
        }
    }

    [Export]
    public string levelPath
    {
        get
        {
            return _levelPath;
        }
        set
        {
            _levelPath = value;
        }
    }

    public void loadLevel()
    {
        try
        {
            GD.Print(_levelPath);
            GetTree().ChangeScene(levelPath);
            GD.Print("loaded level");
        }
        catch (Exception e)
        {
            GD.Print(e);
        }
    }

    public void _on_LevelButton_focus_entered()
    {
        try
        {
            animationPlayer.Play("focused", -1, 2);
        }
        catch(NullReferenceException e)
        {
            GD.Print(e);
            getAnimationPlayerReference();
            animationPlayer.Play("focused");
        }
    }

    public void _on_LevelButton_focus_exited()
    {
        animationPlayer.Stop();
        animationPlayer.Play("reset");
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        try
        {
            getLabelReference();
            getAnimationPlayerReference();
            labelText = labelText;
        }
        catch(Exception e)
        {
            GD.Print(e);
        }
    }

    // public override void _Draw()
    // {
    //     //getLabelReference();
    // }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

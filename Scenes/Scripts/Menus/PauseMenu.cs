using Godot;
using System;
using System.Collections.Generic;

public class PauseMenu : CanvasLayer
{
    private List<Godot.Button> _buttons = new List<Godot.Button>();

    private World world;

    public void pauseGame(bool paused)
    {
        GetTree().Paused = paused;
        ((Panel)GetNode("Panel")).Visible = paused;
    }

    public void exitToMenu()
    {
        world.SaveGame();
        GetTree().ChangeScene("res://Scenes/MainMenu.tscn");
    }

    public void quit()
    {
        world.SaveGame();
        GetTree().Quit();
    }

    private void highlightButton(int index)
    {
        _buttons[index].RectScale = _buttons[index].RectScale * 0.8f;
    }

    private void highlightButtons(int index)
    {
        for (int i = 0; i < 3; i++)
        {
            try
            {
                if (index == i)
                {
                    _buttons[i].RectScale = new Vector2(0.8f, 0.8f);
                }
                else
                {
                    _buttons[i].RectScale = Vector2.One;
                }
            }
            catch (Exception e)
            {
                GD.Print(e);
            }
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        try
        {
            world = (World)GetTree().Root.GetNode("Node2D");
            _buttons.Add((Godot.Button)GetNode("Panel/ItemList/ResumeButton"));
            _buttons.Add((Godot.Button)GetNode("Panel/ItemList/MainMenuButton"));
            _buttons.Add((Godot.Button)GetNode("Panel/ItemList/QuitButton"));
        }
        catch (Exception ex)
        {
            GD.Print(ex);
        }
    }

    private void checkInput()
    {
        if(Input.IsActionJustPressed("ui_cancel"))
        {
            pauseGame(!GetTree().Paused);
        }
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        checkInput();
    }
}

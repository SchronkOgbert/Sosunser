using Godot;
using System;
using System.Collections.Generic;

public class MainMenu : Control
{
    private Control buttonsWidget;
    private int _optionNumber = 0;
    private List<Godot.Button> mainButtons = new List<Godot.Button>();
    private Control optionsMenu;
    private bool inMenu = false;
    public int optionNumber
    {
        get { return _optionNumber; }
        set
        {
            _optionNumber = value % 3;
        }
    }

    public void showOptions()
    {
        GD.Print("showing options");
        buttonsWidget.Visible = false;
        optionsMenu.Visible = true;
        inMenu = true;
    }
    public void play()
    {
        GD.Print("playing");
        GetTree().ChangeScene("res://Scenes/Level1.tscn");
    }
    public void quit()
    {
        GetTree().Quit();
    }
    private void highlightButtons(int index)
    {
        for(int i = 0; i < 3; i++)
        {
            try
            {
                if(index == i)
                {
                    mainButtons[i].RectScale = new Vector2(0.8f, 0.8f);
                }
                else
                {
                    mainButtons[i].RectScale = Vector2.One;
                }                 
            }
            catch (Exception e)
            {
                GD.Print(e);
            }
        }
    }

    private void handleOptions()
    {
        if(Input.IsActionJustPressed("ui_accept"))
        {
            if(!inMenu)
            {
                mainButtons[optionNumber].EmitSignal("pressed");                
            }
        }
        if(Input.IsActionJustPressed("ui_cancel"))
        {
            if(inMenu)
            {
                optionsMenu.Visible = false;
                buttonsWidget.Visible = true;
                inMenu = false;
            }
        }
        if(Input.IsActionJustPressed("ui_down"))
        {
            GD.Print("changed option down");
            optionNumber++;
        }
        if(Input.IsActionJustPressed("ui_up"))
        {
            GD.Print("changed option up");
            optionNumber--;
        }
        highlightButtons(optionNumber);
    }
    public override void _Ready()
    {
        Godot.Button buffer;
        buttonsWidget = (Control)GetNode("Buttons");
        GD.Print(buttonsWidget.GetChild(0).GetType());
        buffer = (Godot.Button)buttonsWidget.GetNode("Button");
        mainButtons.Add(buffer);
        buffer = (Godot.Button)buttonsWidget.GetNode("Button2");
        mainButtons.Add(buffer);
        buffer = (Godot.Button)buttonsWidget.GetNode("Button3");
        mainButtons.Add(buffer);
        optionsMenu = (Control)GetNode("OptionsMenu");


        highlightButtons(optionNumber);
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        handleOptions();
    }
}

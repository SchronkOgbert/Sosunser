using Godot;
using System;
using System.Collections.Generic;

public class MainMenu : Control
{
    public enum state
    {
        MAIN,
        OPTIONS,
        SELECT
    }
    private Control buttonsWidget;
    private int _optionNumber = 0;
    private List<Godot.Button> mainButtons = new List<Godot.Button>();
    private Control optionsMenu;
    private bool inMenu = false;
    private state nextState;
    public int optionNumber
    {
        get { return _optionNumber; }
        set
        {
            _optionNumber = value % 3;
        }
    }

    private state _menuState = state.MAIN;

    public state menuState
    {
        get { return _menuState; }
        set 
        { 
            _menuState = value; 
        }
    }

    public void requestChangeState(state newState)
    {
        nextState = newState;
        Timer timer = (Timer)GetNode("ChangeStateTimer");
        timer.Start();
    }

    public void changeState()
    {
        menuState = nextState;
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
        // GD.Print("playing");
        // GetTree().ChangeScene("res://Scenes/Level1.tscn");
        try
        {
            ((LevelSelect)GetNode("LevelSelect")).Visible = true;
            requestChangeState(state.SELECT);
        }
        catch (Exception e)
        {
            GD.Print(e);
        }
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
        if(menuState == state.SELECT) return;
        if(Input.IsActionJustPressed("ui_accept"))
        {
            mainButtons[optionNumber].EmitSignal("pressed");
            if(optionNumber == 1)
            {
                requestChangeState(state.OPTIONS);
            }
        }
        if(Input.IsActionJustPressed("ui_cancel"))
        {
            if(menuState == state.OPTIONS)
            {
                optionsMenu.Visible = false;
                buttonsWidget.Visible = true;
                inMenu = false;
                requestChangeState(state.MAIN);
            }
        }
        if(menuState != state.OPTIONS)
        {
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
        ((LevelSelect)GetNode("LevelSelect")).menuRef = this;


        highlightButtons(optionNumber);
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        handleOptions();
    }
}

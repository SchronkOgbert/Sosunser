using Godot;
using System;
using System.Collections.Generic;

public class LevelSelect : Control
{
    private List<Godot.Button> _levelButtons = new List<Godot.Button>();
    private int _index = 0;
    private MainMenu _menuRef;

    private int index
    {
        get { return _index; }
        set
        {
            _index = value % 3;
        }
    }

    public MainMenu menuRef
    {
        get { return _menuRef; }
        set 
        {
            _menuRef = value;
        }
    }

    private void handleOptions()
    {
        if(menuRef.menuState != MainMenu.state.SELECT) return;
        //GD.Print("level select pressing option");
        highlightButtons(index);
        if(Input.IsActionJustPressed("ui_accept"))
        {
            _levelButtons[index].EmitSignal("pressed");
        }
        if(Input.IsActionJustPressed("ui_cancel"))
        {
            this.Visible = false;
            //buttonsWidget.Visible = true;
            menuRef.menuState = MainMenu.state.MAIN;
        }
        if(Input.IsActionJustPressed("ui_down"))
        {
            GD.Print("level select changed option down");
            index++;
        }
        if(Input.IsActionJustPressed("ui_up"))
        {
            GD.Print("level select changed option up");
            index--;
        }
        highlightButtons(index);
    }

    private void highlightButtons(int newIndex)
    {
        //GD.Print(newIndex);
        for(int i = 0; i < 3; i++)
        {
            try
            {
                if(newIndex == i)
                {
                    _levelButtons[i].EmitSignal("focus_entered");
                }
                else
                {
                    _levelButtons[i].EmitSignal("focus_exited");
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
            ItemList buttonList = (ItemList)GetNode("ItemList");
            GD.Print("Kist is: ", GetNode("ItemList").GetNode("LevelButton").GetType());
            Godot.Button buffer = (Godot.Button)buttonList.GetNode("LevelButton");
            GD.Print(buffer);
            _levelButtons.Add(buffer);
            buffer = (Godot.Button)buttonList.GetNode("LevelButton2");
            _levelButtons.Add(buffer);
            buffer = (Godot.Button)buttonList.GetNode("LevelButton3");
            _levelButtons.Add(buffer);
        }
        catch (NullReferenceException e)
        {
            GD.Print("Level select ready function throws:\n", e);
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        try
        {
            handleOptions();
        }
        catch(Exception e)
        {
            //GD.Print("Level select says:\n\n", e);
        }
    }
}

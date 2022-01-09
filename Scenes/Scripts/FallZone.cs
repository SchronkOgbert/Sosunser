using Godot;
using System;

public class FallZone : Area2D
{
    [Export]
    public string currentSceneName;
    public void _on_FallZone_body_entered(KinematicBody player)
    {
        try
        {
            GD.Print("Scene name", GetTree().EditedSceneRoot.Filename);
        }
        catch (Exception e)
        {
            GD.Print("Fall zone says:\n\n", e);
        }
        GetTree().ChangeScene(currentSceneName);
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

}

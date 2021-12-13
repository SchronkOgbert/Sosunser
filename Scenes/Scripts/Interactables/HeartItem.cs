using Godot;
using System;

public class HeartItem : Area2D
{

    public void addLife(KinematicBody2D body)
    {
        try
        {
            body.Call("takeDamage", -1);             
        }
        catch (Exception e)
        {
            GD.Print(e);
        }
        QueueFree();
    }
}

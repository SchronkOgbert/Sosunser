using Godot;
using System;

public class GameHUD : CanvasLayer
{
    private int coins;
    private int hearts;

    //references
    private Label coinsText;
    private AnimationPlayer player;

    private void setCoinsText()
    {
        coinsText.Text = coins.ToString();
    }
    private void playCollectedAnimation()
    {
        player.Stop();
        player.Play("add_coin");
    }

    public void addCoins(int amount = 1, bool playAnimation = false)
    {
        //GD.Print("adding coins to hud");
        coins += amount;
        setCoinsText();
        if(playAnimation)
        {
            playCollectedAnimation();
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        coinsText = (Label)GetNode("Coins").GetNode("Label");
        player = (AnimationPlayer)GetNode("AnimationPlayer");
    }
}

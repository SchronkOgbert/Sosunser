using Godot;
using System;
using System.Collections.Generic;

public class GameHUD : CanvasLayer
{
    //constants
    private Vector2 heartSpriteSize = new Vector2(3, 3);
    //logic
    private int _coins;
    private int _hearts;
    private int _score;
    List<Heart> heartsList = new List<Heart>();

    //references
    private Label coinsText;
    private AnimationPlayer player;
    private Control heartsContainer;
    private Label scorePointsLabel;

    public int coins
    {
        get
        {
            return _coins;
        }
        set
        {
            _coins = value;
        }
    }

    public int hearts
    {
        get
        {
            return _hearts;
        }
        set
        {
            if(value < 0) 
            {
                _hearts = 0;
                return;
            }
            _hearts = value;
        }
    }
    public int score
    {
        get { return _score; }
        set { _score = value; }
    }

    private void setCoinsText()
    {
        try
        {
            coinsText.Text = _coins.ToString();             
        }
        catch (Exception e)
        {
            GD.Print("setCoinsText():\n", e);
        }
    }
    private void playCollectedAnimation()
    {
        player.Stop();
        player.Play("add_coin");
    }

    private void setScoreWidget()
    {
        scorePointsLabel.Text = score.ToString();
    }

    public void addScore(int amount)
    {
        score += amount;
        setScoreWidget();
    }

    private void makeHearts(int count)
    {
        PackedScene package = (PackedScene)ResourceLoader.Load("res://Scenes/Heart.tscn");
        GD.Print("making hearts");
        for(int i = 0; i < count; i++)
        {
            GD.Print("making heart ", i);
            try
            {
                Heart buffer = (Heart)package.Instance();
                heartsContainer.AddChild(buffer);
                heartsList.Add(buffer);
                buffer.Position = new Vector2(24 + 48 * i, 24);                 
            }
            catch (Exception e)
            {
                GD.Print("makeHearts(int count):\n", e);
            }
        }
    }

    public void updateHearts(int count)
    {
        foreach(Heart heart in heartsList)
        {
            if(count > 0)
            {
                count--;
                heart.Texture = (Texture)GD.Load("res://Textures/platform_metroidvania asset pack v1.01/hud elements/hearts_hud.png");
                continue;
            }
            heart.Texture = (Texture)GD.Load("res://Textures/platform_metroidvania asset pack v1.01/hud elements/no_hearts_hud.png");
        }
    }

    public void addCoins(int amount = 1, bool playAnimation = false)
    {
        //GD.Print("adding _coins to hud");
        _coins += amount;
        setCoinsText();
        if(playAnimation)
        {
            playCollectedAnimation();
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        coinsText = (Label)GetNode("Coins/Label");
        GD.Print(coinsText);
        player = (AnimationPlayer)GetNode("AnimationPlayer");
        heartsContainer = (Control)GetNode("Hearts");
        scorePointsLabel = (Label)GetNode("Score/Points");
        addCoins(coins);
        makeHearts(hearts);
        updateHearts(hearts);
    }
}

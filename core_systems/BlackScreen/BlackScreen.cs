using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class BlackScreen : Control
{
    AnimationPlayer player;
    bool isActive = false;

    public override void _Ready()
    {
        base._Ready();

        player = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public void SetActive(bool newActive, bool newAnim = true)
    {
        isActive = newActive;

        if (newAnim)
        {
            if(Visible == false)
                Visible = true;

            if (newActive)
                player.Play("PlayUnvisible");
            else
                player.PlayBackwards("PlayUnvisible");
        }
        else
        {
            Visible = newActive;
        }
    }
}

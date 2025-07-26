using Godot;
using System;

public partial class Radio : Area3D
{
    Timer timer = null;
    AudioStreamPlayer3D radio_on = null;
    AudioStreamPlayer3D music = null;

    public override void _Ready()
    {
        timer = GetNode<Timer>("%Timer");
        radio_on = GetNode<AudioStreamPlayer3D>("%RadioOn");
        music = GetNode<AudioStreamPlayer3D>("%Music");

        radio_on.Play();
        timer.Timeout += PlayAnnouncement;
    }

    private void TurnMusicOnOff()
    {
        if (music.Playing)
        {
            music.Stop();
            return;
        }
        
        radio_on.Stop();
        music.Play();
    }

    private void PlayAnnouncement()
    {

    }

    public void PlayerInteract()
    {
        TurnMusicOnOff();
    }
}

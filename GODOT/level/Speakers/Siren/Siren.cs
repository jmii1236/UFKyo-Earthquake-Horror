using Godot;
using System;

public partial class Siren : Area3D
{
    [Export]
    double start_time_siren = 5;
    [Export]
    double interval_between = 5;
    Timer timer = null;
    AudioStreamPlayer3D noise = null;

    public override void _Ready()
    {
        timer = GetNode<Timer>("%Timer");
        noise = GetNode<AudioStreamPlayer3D>("%Noise");

        Timer start_timer = new Timer();
        start_timer.Timeout += () => timer.Start(interval_between);
        start_timer.Autostart = true;

        timer.Timeout += PlaySiren;
    }

    private void PlaySiren()
    {
        noise.Play();
    }
}

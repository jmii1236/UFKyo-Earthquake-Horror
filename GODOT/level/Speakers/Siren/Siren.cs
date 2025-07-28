using Godot;
using System;

public partial class Siren : Area3D
{
    [Export]
    double interval_between = 10;
    AudioStreamPlayer3D noise = null;
    Globals globals = null;
    Timer timer = null;

    public override void _Ready()
    {
        globals = Globals.Instance;
        globals.EarthquakeOccurs += OnEarthquakeOccurs;

        noise = GetNode<AudioStreamPlayer3D>("%Noise");
        timer = GetNode<Timer>("Timer");
        timer.Timeout += PlaySiren;
    }

    private void PlaySiren()
    {
        noise.Play();
    }

    private void OnEarthquakeOccurs()
    {
        GD.Print("Earthquake!");
        timer.Start(interval_between);
    }
}

using Godot;
using System;

public partial class Earthquake : Node3D
{
    private Timer _timer = null;
    private Globals globals = null;

    public override void _Ready()
    {
        globals = GetNode("/root/Globals") as Globals;

        _timer = GetNode<Timer>("Timer");
        _timer.Timeout += () => globals.EmitSignal(Globals.SignalName.EarthquakeOccurs);
    }
}

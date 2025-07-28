using Godot;
using System;

public partial class Earthquake : Node3D
{
    private Timer _timer = null;
    private TraumaCauser trauma_causer = null;

    public override void _Ready()
    {
        trauma_causer = GetNode<TraumaCauser>("TraumaCauser");

        _timer = GetNode<Timer>("Timer");
        _timer.Timeout += () => trauma_causer.CauseTrauma();
    }
}

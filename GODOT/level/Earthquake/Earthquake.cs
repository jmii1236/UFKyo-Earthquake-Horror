using Godot;
using System;

public partial class Earthquake : Node3D
{
    private Area3D trigger_zone = null;
    private TraumaCauser trauma_causer = null;
    private bool occurred = false;
    Globals globals = null;

    public override void _Ready()
    {
        globals = Globals.Instance;

        trauma_causer = GetNode<TraumaCauser>("TraumaCauser") as TraumaCauser;
        trigger_zone = GetNode<Area3D>("%TriggerZone") as Area3D;

        trigger_zone.BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (body.IsInGroup("Player") && !occurred)
        {
            trauma_causer.CauseTrauma();
            globals.EmitSignal("EarthquakeOccurs");

            occurred = true;
        }
    }
}

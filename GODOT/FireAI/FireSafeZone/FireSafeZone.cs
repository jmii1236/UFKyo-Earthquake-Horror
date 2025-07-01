using Godot;
using System;

public partial class FireSafeZone : Area3D
{
    private Globals globals = null;
    public override void _Ready()
    {
        BodyEntered += _On_Body_Entered;
        globals = GetNode("/root/Globals") as Globals;
    }

    private void _On_Body_Entered(Node3D body)
    {
        if (body.IsInGroup("Player"))
        {
            globals.EmitSignal(Globals.SignalName.DisableFire);
        }
    }
}
using Godot;
using System;

[GlobalClass]
public partial class Items : RigidBody3D
{
    private Globals globals = null;

    public override void _Ready()
    {
        globals = GetNode("/root/Globals") as Globals;
        
        BodyEntered += _On_Body_Entered;
    }


    private void _On_Body_Entered(Node body)
    {
        globals.EmitSignal(Globals.SignalName.ItemInteraction);
    }
}

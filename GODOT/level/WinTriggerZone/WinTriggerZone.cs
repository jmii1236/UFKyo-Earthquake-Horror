using Godot;
using System;

public partial class WinTriggerZone : Area3D
{
    [Export]
    private bool debug = false;
    private MeshInstance3D mesh = null;
    private Globals globals = null;
    public override void _Ready()
    {
        mesh = GetNode("%Mesh") as MeshInstance3D;
        if (!debug)
        {
            mesh.QueueFree();
        }

        BodyEntered += _On_Body_Entered;
        globals = Globals.Instance;
    }

    private void _On_Body_Entered(Node3D body)
    {
        if (body.IsInGroup("Player"))
        {
            globals.EmitSignal(Globals.SignalName.WinGame);
        }
    }
}

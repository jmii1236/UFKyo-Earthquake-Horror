using Godot;
using System;

public partial class SpawnTriggerZone : Area3D
{
    [Export]
    private Node3D thing_to_spawn = null;
    public override void _Ready()
    {
        BodyEntered += _On_Body_Entered;
    }


    public void _On_Body_Entered(Node3D body)
    {
        if (thing_to_spawn == null && body.IsInGroup("Player")) return;
        AddChild(thing_to_spawn);
    }
}
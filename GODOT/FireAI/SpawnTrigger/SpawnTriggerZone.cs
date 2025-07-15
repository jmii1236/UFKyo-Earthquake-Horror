using Godot;
using System;

public partial class SpawnTriggerZone : Area3D
{
    [Export]
    private PackedScene thing_to_spawn = null;
    private Node thing_spawned = null;
    private bool spawned = false;
    public override void _Ready()
    {
        BodyEntered += _On_Body_Entered;
    }


    public void _On_Body_Entered(Node body)
    {
        if (spawned) return;
        thing_spawned = thing_to_spawn.Instantiate();
        AddChild(thing_spawned);
        spawned = true;
    }
}
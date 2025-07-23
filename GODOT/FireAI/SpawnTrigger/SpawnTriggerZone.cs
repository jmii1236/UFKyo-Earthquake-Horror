using Godot;
using System;
using System.Diagnostics;

public partial class SpawnTriggerZone : Area3D
{
    [Export]
    private PackedScene thing_to_spawn = null;
    [Export]
    private CharacterBody3D PlayerDad = null;
    [Export]
    private bool debug = false;
    private Node3D thing_spawned = null;
    private bool spawned = false;
    private MeshInstance3D mesh = null;
    private Globals globals = null;
    public override void _Ready()
    {
        BodyEntered += _On_Body_Entered;

        mesh = GetNode("%Mesh") as MeshInstance3D;
        if (!debug)
        {
            mesh.QueueFree();
        }

        globals = GetNode("/root/Globals") as Globals;
    }

    public void _On_Body_Entered(Node body)
    {
        if (!body.IsInGroup("Player")) return;

        if (spawned) return;

        thing_spawned = thing_to_spawn.Instantiate() as FireAI;
        AddChild(thing_spawned);
        spawned = true;
        thing_spawned.Translate(Vector3.One);
        globals.EmitSignal(Globals.SignalName.SetPlayer, PlayerDad);
    }
}
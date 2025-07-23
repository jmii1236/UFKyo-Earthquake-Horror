using Godot;
using System;
using System.Diagnostics;

public partial class SpawnTriggerZone : Area3D
{
    [Export]
    private Node3D thing_to_spawn = null;
    // PlayerDad playerDad;
    public override void _Ready()
    {
        BodyEntered += _on_body_entered;
        // playerDad = GetNode<PlayerDad>("../PlayerDad");
    }


    public void _on_body_entered(Node3D body)
    {
        if (thing_to_spawn == null && body.IsInGroup("Player")) return;
        AddChild(thing_to_spawn);
        //GD.Print("Spawned fire at " + thing_to_spawn.GlobalPosition);
    }

    // public void _on_body_entered(CharacterBody3D body)
    // {
    //     if (body == playerDad)
    //     {
    //         playerDad.TakeDamage(35);
    //         GD.Print("PlayerDad has entered the spawn trigger zone and took damage.");
    //     }

        
    // }

}
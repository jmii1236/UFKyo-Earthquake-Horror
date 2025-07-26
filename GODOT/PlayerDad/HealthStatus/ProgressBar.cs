using Godot;
using System;

public partial class ProgressBar : Godot.ProgressBar
{
  PlayerDad PlayerDad { get; set; }

  public override void _Ready()
  {
    PlayerDad = GetTree().Root.GetNode("Node3D/PlayerDad") as PlayerDad;
    if (PlayerDad == null)
    {
      GD.PrintErr("ProgressBar: Could not find PlayerDad node.");
      return;
    }

    PlayerDad.Connect(nameof(PlayerDad.HealthChange), new Callable(this, nameof(HealthChanged)));
  }
  private void HealthChanged(int health)
    {
        Value = health;
        //GD.Print($"ProgressBar updated: {Value}");
    }
}

using Godot;
using System;

public partial class ProgressBar : Godot.ProgressBar
{
  PlayerDad PlayerDad { get; set; }

  public override void _Ready()
  {
    PlayerDad = GetNode<PlayerDad>("../../PlayerDad");
    if (PlayerDad == null)
    {
      GD.PrintErr("Could not find PlayerDad node.");
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

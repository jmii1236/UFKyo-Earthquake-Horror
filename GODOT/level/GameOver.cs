using Godot;
using System;

public partial class GameOver : Control
{
    private Globals globals = null;
    public override void _Ready()
    {
        base._Ready();

        globals = GetNode("/root/Globals") as Globals;
        globals.EarthquakeOccurs += _On_Earthquake_Occurs;
    }

    private void _On_Earthquake_Occurs()
    {
        Visible = true;
    }
}

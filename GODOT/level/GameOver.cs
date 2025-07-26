using Godot;
using System;

public partial class GameOver : Control
{
    private Globals globals = null;
    public override void _Ready()
    {
        globals = Globals.Instance;
        globals.EarthquakeOccurs += _On_Earthquake_Occurs;
    }

    private void _On_Earthquake_Occurs()
    {
        Visible = true;
    }
}

using Godot;
using System;
using System.Collections.Generic;

public partial class TraumaCauser : Area3D
{
    [Export] float trauma_amount = 0.1f;

    public void CauseTrauma()
    {
        var trauma_areas = GetOverlappingAreas();
        foreach (Area3D area in trauma_areas)
        {
            if (area.HasMethod("AddTrauma"))
            {
                area.Call("AddTrauma", trauma_amount);
            }
        }
    }
}

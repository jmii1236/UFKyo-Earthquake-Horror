using Godot;
using System;

public partial class Medkit : Items
{
    public override void _Ready()
    {
        BodyEntered += _On_Body_Entered;
    }


    private void _On_Body_Entered(Node body)
    {
        
    }
}

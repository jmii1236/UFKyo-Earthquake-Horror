using Godot;
using System;

public partial class Level : Node3D
{
    private Globals globals = null;

    public override void _Ready()
    {
        globals = GetNode("/root/Globals") as Globals;

        globals.WinCondition += on_win_condition;
    }

    private void on_win_condition()
    {
        GetTree().ChangeSceneToFile("res://Menus/WinMenu/win_menu.tscn");
    }
}

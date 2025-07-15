using Godot;
using System;

public partial class MainMenu : Control
{
    Button play_button, settings_button, quit_button;
    public override void _Ready()
    {
        play_button = GetNode("%Play") as Button;
        settings_button = GetNode("%Settings") as Button;
        quit_button = GetNode("%Quit") as Button;

        play_button.Pressed += on_play_button_pressed;
        settings_button.Pressed += on_settings_button_pressed;
        quit_button.Pressed += on_quit_button_pressed;
    }

    private void on_play_button_pressed()
    {
        GD.Print("Ready Pressed");
    }

    private void on_settings_button_pressed()
    {
        GD.Print("Settings Pressed");
    }
    
    private void on_quit_button_pressed()
    {
        GD.Print("Quit Pressed");
    }
}

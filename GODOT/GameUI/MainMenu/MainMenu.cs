using Godot;
using System;

public partial class MainMenu : Node2D
{
  public void _on_start_pressed()
  {
    // Logic to start the game
    GD.Print("Start button pressed. Starting the game...");
    // Here you would typically change the scene or start the game logic
    GetTree().ChangeSceneToFile("res://level/Level.tscn");
  }
  public void _on_quit_pressed()
  {
    // Logic to quit the game
    GD.Print("Quit button pressed. Exiting the game...");
    GetTree().Quit();    
  }
}

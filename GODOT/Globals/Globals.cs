using Godot;
using System;

public partial class Globals : Node
{
	public static Globals Instance { get; private set; }

	[Signal]
	public delegate void EarthquakeOccursEventHandler();
	[Signal]
	public delegate void DeathEventHandler();
	[Signal]
	public delegate void GameOverEventHandler();
	[Signal]
	public delegate void DisableFireEventHandler();
	[Signal]
	public delegate void WinGameEventHandler();
	[Signal]
	public delegate void SetPlayerEventHandler(CharacterBody3D Player);

    public override void _Ready()
    {
		Instance = this;
    }

}

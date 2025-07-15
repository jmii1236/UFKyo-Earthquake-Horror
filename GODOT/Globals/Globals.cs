using Godot;
using System;

public partial class Globals : Node
{
	[Signal]
	public delegate void EarthquakeOccursEventHandler();
	[Signal]
	public delegate void WinConditionEventHandler();
}

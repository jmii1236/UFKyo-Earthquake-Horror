using Godot;
using System;

public partial class SplotData : Resource
{
  const int MAX_STACK = 99;
  [Export] ItemData itemdata; 
  [Export] public int stackSizeMin = 1;
  [Export] public int stackSizeMax = MAX_STACK;

}

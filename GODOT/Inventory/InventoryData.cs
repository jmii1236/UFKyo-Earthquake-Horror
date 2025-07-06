using Godot;
using System;

public partial class InventoryData : Resource
{
  [Export]
  public Godot.Collections.Array<SplotData> SlotDatas { get; set; } = new();
} 

using Godot;
using System;

[GlobalClass]
public partial class InventoryData : Resource
{
  [Signal] public delegate void InventoryInteractEventHandler(InventoryData inventoryData, int index, int button);
  [Signal] public delegate void InventoryUpdatedEventHandler(InventoryData inventoryData);
  [Export] public Godot.Collections.Array<SlotData> slotdatas { get; set; } = new();

  public void OnSlotClickedEventHandler(int index, int button)
  {
    GD.Print($"InventoryData.OnSlotClickedEventHandler called - Index: {index}, Button: {button}");
    GD.Print($"Connected signals count: {GetSignalConnectionList(SignalName.InventoryInteract).Count}");
    EmitSignal(SignalName.InventoryInteract, this, index, button);
  }

  public SlotData DropSlotData(SlotData grabbedSlotData, int index)
  {
    GD.Print($"InventoryData.DropSlotData called - Index: {index}, DroppedItem: {grabbedSlotData?.ItemData?.Name ?? "null"}");
    var slotData = slotdatas[index];
    if (index >= 0 && index < slotdatas.Count)
    {
      slotdatas[index] = grabbedSlotData;
      EmitSignal(SignalName.InventoryUpdated, this);
    }
    return slotData;
  }

  public bool PickUpSlotData(SlotData slotData)
  {
    GD.Print($"InventoryData.PickUpSlotData called - Item: {slotData?.ItemData?.Name ?? "null"}");
    for (int index = 0; index < slotdatas.Count; index++)
    {
        if (slotdatas[index] == null)
        {
            slotdatas[index] = slotData;
            EmitSignal(SignalName.InventoryUpdated, this);
            GD.Print($"Item placed at index {index}");
            return true;
        }
    }
    GD.Print("No empty slots found");
    return false;
  }

  public SlotData GrabSlotData(int index)
  {
    GD.Print($"InventoryData.GrabSlotData called - Index: {index}");
    if (index >= 0 && index < slotdatas.Count)
    {
      var slotData = slotdatas[index];
      if (slotData != null)
      {
        GD.Print($"Grabbing item: {slotData.ItemData?.Name ?? "null"}");
        slotdatas[index] = null;
        EmitSignal(SignalName.InventoryUpdated, this);
        return slotData;
      }
    }
    GD.Print("Nothing to grab");
    return null;
  }
}

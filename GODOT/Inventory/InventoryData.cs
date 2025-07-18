using Godot;
using System;

[GlobalClass]
public partial class InventoryData : Resource
{
  [Signal] public delegate void InventoryInteractEventHandler(InventoryData inventoryData, int index, int button);
  [Signal] public delegate void InventoryUpdatedEventHandler(InventoryData inventoryData);
  [Export]
  public Godot.Collections.Array<SlotData> slotdatas { get; set; } = new();

  public void OnSlotClickedEventHandler(int index, int button)
  {
    EmitSignal(SignalName.InventoryInteract, this, index, button);
  }

  public SlotData DropSlotData(SlotData grabbedSlotData, int index)
  {
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
    for (int index = 0; index < slotdatas.Count; index++)
    {
        if (slotdatas[index] == null)
        {
            slotdatas[index] = slotData;
            EmitSignal(SignalName.InventoryUpdated, this);
            return true;
        }
    }
    return false;
}

  public SlotData GrabSlotData(int index)
  {
    if (index >= 0 && index < slotdatas.Count)
    {
      var slotData = slotdatas[index];
      if (slotData != null)
      {
        slotdatas[index] = null; // This removes it from the inventory
        EmitSignal(SignalName.InventoryUpdated, this); // Notify that the inventory has been updated
        return slotData;
      }
    }
    return null;
  }
} 

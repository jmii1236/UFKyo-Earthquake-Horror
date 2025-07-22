using Godot;
using System;

public partial class Inventory : PanelContainer
{

  
  private static readonly PackedScene SlotScene = GD.Load<PackedScene>("res://Inventory/slot.tscn");

  [Export] GridContainer itemGrid;

  private InventoryData inventoryData;

  public void SetInventoryData(InventoryData inventoryData)
  {
    this.inventoryData = inventoryData;
    inventoryData.Connect(InventoryData.SignalName.InventoryUpdated, new Callable(this, nameof(PopulateItemGrid)));
    PopulateItemGrid(inventoryData);
  }
  public void ClearInventoryData(InventoryData inventoryData)
  {
    this.inventoryData = inventoryData;
    inventoryData.Disconnect(InventoryData.SignalName.InventoryUpdated, new Callable(this, nameof(PopulateItemGrid)));
  }
  private void PopulateItemGrid(InventoryData inventoryData)
  {
    foreach (Node child in itemGrid.GetChildren())
    {
      child.QueueFree();
    }

    for (int i = 0; i < inventoryData.slotdatas.Count; i++)
    {
      var slotdata = inventoryData.slotdatas[i];
      var slot = (Slot)SlotScene.Instantiate();
      itemGrid.AddChild(slot);

      slot.Connect(Slot.SignalName.SlotClicked, new Callable(this, nameof(OnSlotClicked)));

      // Pass the index and let the slot handle null data
      slot.SetSlotData(slotdata);
    }
  }
  public void OnSlotClicked(int index, int button)
{
    GD.Print($"Slot clicked: Index {index}, Button {button}");
    this.inventoryData?.OnSlotClickedEventHandler(index, button);
}

}

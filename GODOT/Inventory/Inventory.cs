using Godot;
using System;

public partial class Inventory : PanelContainer
{
  private static readonly PackedScene SlotScene = GD.Load<PackedScene>("res://Inventory/slot.tscn");
  [Export] GridContainer itemGrid;
  private InventoryData inventoryData;

  public void SetInventoryData(InventoryData inventoryData)
  {
    GD.Print($"Inventory.SetInventoryData called for inventory: {inventoryData}");
    this.inventoryData = inventoryData;
    
    if (inventoryData.IsConnected("InventoryUpdated", new Callable(this, nameof(PopulateItemGrid))))
    {
        inventoryData.Disconnect("InventoryUpdated", new Callable(this, nameof(PopulateItemGrid)));
    }
    inventoryData.Connect("InventoryUpdated", new Callable(this, nameof(PopulateItemGrid)));

    PopulateItemGrid(inventoryData);
  }

  public void ClearInventoryData(InventoryData inventoryData)
  {
    this.inventoryData = inventoryData;
    inventoryData.Disconnect(InventoryData.SignalName.InventoryUpdated, new Callable(this, nameof(PopulateItemGrid)));
  }

  private void PopulateItemGrid(InventoryData inventoryData)
  {
    GD.Print($"Inventory.PopulateItemGrid called - Slot count: {inventoryData.slotdatas.Count}");
    
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
        slot.SetSlotData(slotdata);
        slot.SetIndex(i); 
    }
  }

  public void OnSlotClicked(int index, int button)
  {
    GD.Print($"Inventory.OnSlotClicked - Index {index}, Button {button}");
    GD.Print($"InventoryData is null: {this.inventoryData == null}");
    this.inventoryData?.OnSlotClickedEventHandler(index, button);
  }
}
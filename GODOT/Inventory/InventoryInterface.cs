using Godot;
using System;

public partial class InventoryInterface : Control
{
    private SlotData grabbedSlotData;
    [Export] private PanelContainer playerInventory;
    private InventoryData externalInventoryOwner;

    private PanelContainer externalInventory;

    [Export] private PanelContainer grabbedSlot;

    public override void _Ready()
    {
        playerInventory = GetNode<PanelContainer>("PlayerInventory");
        grabbedSlot = GetNode<PanelContainer>("GrabbedSlot");
        externalInventory = GetNode<PanelContainer>("ExternalInventory");
    }
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (grabbedSlot.Visible)
        {
            grabbedSlot.GlobalPosition = GetGlobalMousePosition() + new Vector2(2, 2); // Adjust position as needed
        }
  }


    public void SetPlayerInventoryData(InventoryData inventoryData)
    {
        if (inventoryData == null)
    {
        GD.PrintErr("InventoryData is null in SetPlayerInventoryData!");
        return;
    }

    if (playerInventory == null)
    {
        GD.PrintErr("PlayerInventory UI is not assigned!");
        return;
    }
        inventoryData.InventoryInteract += OnInventoryInteract;
        var inventory = playerInventory as Inventory;
        inventory?.SetInventoryData(inventoryData);
    }
    public void SetExternalInventory(InventoryData _externalInventory)
    {
        //GD.Print("Setting external inventory data: " + _externalInventory.ToString());
        if (_externalInventory == null)
    {
        GD.PrintErr("External InventoryData is null in SetPlayerInventoryData!");
        return;
    }


        externalInventoryOwner = _externalInventory;
        InventoryData inventoryData = externalInventoryOwner;
        inventoryData.InventoryInteract += OnInventoryInteract;
        var inventory = externalInventory as Inventory;
        inventory?.SetInventoryData(inventoryData);

        externalInventory.Visible = true;
    }
    public void ClearExternalInventory(InventoryData _externalInventory)
    {
        if (externalInventoryOwner != null)
        {
            InventoryData inventoryData = externalInventoryOwner;
            inventoryData.InventoryInteract += OnInventoryInteract;
            var inventory = externalInventory as Inventory;
            inventory?.SetInventoryData(inventoryData);

            externalInventory.Visible = false;
            externalInventoryOwner = null;
        }
    }

    private void OnInventoryInteract(InventoryData inventoryData, int index, int button)
    {
        switch (grabbedSlotData, button)
        {
            case (null, (int)MouseButton.Left):
                grabbedSlotData = inventoryData.GrabSlotData(index);
                break;
            case (_, (int)MouseButton.Left):
                grabbedSlotData = inventoryData.DropSlotData(grabbedSlotData, index);
                break;
        }

        updateGrabbedSlot();
    }

    public void updateGrabbedSlot()
    {
        if (grabbedSlotData != null)
        {
            grabbedSlot.Show();
            var slot = grabbedSlot as Slot;
            slot?.SetSlotData(grabbedSlotData);
        }
        else
        {
            grabbedSlot.Hide();
        }
    }
}
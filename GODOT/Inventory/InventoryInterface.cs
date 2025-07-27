using Godot;
using System;

public partial class InventoryInterface : Control
{
    private SlotData grabbedSlotData;
    [Export] private PanelContainer playerInventory;
    private InventoryData externalInventoryOwner;
    private InventoryData playerInventoryData;
    
    private PanelContainer externalInventory;
    [Export] private PanelContainer grabbedSlot;
    public Backpack backpack;

    public override void _Ready()
    {
        playerInventory = GetNode<PanelContainer>("PlayerInventory");
        grabbedSlot = GetNode<PanelContainer>("GrabbedSlot");
        externalInventory = GetNode<PanelContainer>("ExternalInventory");
        backpack = GetNode<Backpack>("Backpack");
        if (backpack == null)
        {
            GD.PrintErr("Backpack node not found in InventoryInterface _Ready!");
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (grabbedSlot.Visible)
        {
            grabbedSlot.GlobalPosition = GetGlobalMousePosition() + new Vector2(2, 2);
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

        // Disconnect old signal if exists
        if (playerInventoryData != null)
        {
            playerInventoryData.InventoryInteract -= OnInventoryInteract;
        }

        playerInventoryData = inventoryData;
        inventoryData.InventoryInteract += OnInventoryInteract;
        
        var inventory = playerInventory as Inventory;
        inventory?.SetInventoryData(inventoryData);
    }

    public void SetExternalInventory(InventoryData _externalInventory)
    {
        if (_externalInventory == null)
        {
            GD.PrintErr("External InventoryData is null in SetExternalInventory!");
            return;
        }

        // Disconnect old external inventory if exists
        if (externalInventoryOwner != null)
        {
            externalInventoryOwner.InventoryInteract -= OnInventoryInteract;
        }

        externalInventoryOwner = _externalInventory;
        externalInventoryOwner.InventoryInteract += OnInventoryInteract;

        var inventory = externalInventory as Inventory;
        inventory?.SetInventoryData(externalInventoryOwner);

        externalInventory.Visible = true;
        GD.Print("SetExternalInventory CALLED");
        if (backpack != null && !backpack.Visible)
        {
            externalInventory.Visible = true;
            GD.Print("Backpack picked up, apart of player inventory");
        }
    }

    public void ClearExternalInventory()
    {
        if (externalInventoryOwner != null)
        {
            externalInventoryOwner.InventoryInteract -= OnInventoryInteract;
            externalInventoryOwner = null;
        }
        
        externalInventory.Visible = false;
        
        // If player was holding something from external inventory, drop it back
        if (grabbedSlotData != null)
        {
            // Try to put it back in player inventory
            if (playerInventoryData != null && !playerInventoryData.PickUpSlotData(grabbedSlotData))
            {
                GD.Print("Warning: Couldn't return grabbed item to player inventory");
            }
            grabbedSlotData = null;
            updateGrabbedSlot();
        }
    }

    private void OnInventoryInteract(InventoryData inventoryData, int index, int button)
    {
        GD.Print($"OnInventoryInteract - Index: {index}, Button: {button}, HasGrabbedItem: {grabbedSlotData != null}");
        
        switch (grabbedSlotData, button)
        {
            case (null, (int)MouseButton.Left):
                // Try to grab item
                grabbedSlotData = inventoryData.GrabSlotData(index);
                if (grabbedSlotData != null)
                {
                    GD.Print($"Grabbed: {grabbedSlotData.ItemData?.Name}");
                }
                break;
                
            case (_, (int)MouseButton.Left):
                // Try to drop item
                var returnedSlotData = inventoryData.DropSlotData(grabbedSlotData, index);
                grabbedSlotData = returnedSlotData; // This will be null if drop was successful, or the swapped item
                if (grabbedSlotData != null)
                {
                    GD.Print($"Swapped, now holding: {grabbedSlotData.ItemData?.Name}");
                }
                else
                {
                    GD.Print("Item dropped successfully");
                }
                break;
                
            case (_, (int)MouseButton.Right):
                // Right click - cancel grab or split stack
                if (grabbedSlotData != null)
                {
                    // Try to return to original inventory (you might need to track source)
                    if (!inventoryData.PickUpSlotData(grabbedSlotData))
                    {
                        // If that fails, try player inventory
                        if (playerInventoryData != null)
                        {
                            playerInventoryData.PickUpSlotData(grabbedSlotData);
                        }
                    }
                    grabbedSlotData = null;
                    GD.Print("Cancelled grab with right-click");
                }
                break;
        }

        updateGrabbedSlot();
    }

    public void updateGrabbedSlot()
    {
        if (grabbedSlotData != null)
        {
            grabbedSlot.Show();
            
            // Try to get the Slot component from grabbedSlot
            // This assumes grabbedSlot contains a Slot as a child
            var slot = grabbedSlot.GetNode<Slot>("Slot"); // Adjust path as needed
            if (slot == null)
            {
                // If grabbedSlot IS the slot
                slot = grabbedSlot as Slot;
            }
            
            if (slot != null)
            {
                slot.SetSlotData(grabbedSlotData);
            }
            else
            {
                GD.PrintErr("Could not find Slot component in grabbedSlot!");
            }
        }
        else
        {
            grabbedSlot.Hide();
        }
    }
}
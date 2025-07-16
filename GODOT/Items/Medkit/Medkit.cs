using Godot;
using System;

public partial class Medkit : Item
{
    [Export] public InventoryData inventoryData;

    [Signal] public delegate void InventoryToggleEventHandler(InventoryData ExternalInventory);
    
    public override void _Ready()
    {
        base._Ready(); // Call parent _Ready if Item has one
        
        // Add to the ExternalInventory group so it behaves like a chest
        AddToGroup("ExternalInventory");
        
        // Remove from Item group so it won't be picked up
        RemoveFromGroup("Item");
    }
    
    public void PlayerInteract()
    {
        GD.Print("Player interacting with medkit chest");
        EmitSignal(SignalName.InventoryToggle, inventoryData);
    }
}
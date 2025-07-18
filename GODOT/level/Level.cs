using Godot;

public partial class Level : Node3D
{
    [Export] private CharacterBody3D player;
    [Export] private Control inventoryInterface;

    public override void _Ready()
    {
        inventoryInterface = GetNode<Control>("UI/InventoryInterface");
        player.Connect(nameof(PlayerDad.ToggleInventory), new Callable(this, nameof(ToggleInventoryInterface)));
        
        // Cast player to PlayerDad to access InventoryData
        ((InventoryInterface)inventoryInterface).SetPlayerInventoryData(((PlayerDad)player).InventoryData);

        // Connect to all external inventory objects
        foreach (Node inGroup in GetTree().GetNodesInGroup("ExternalInventory"))
        {
            if (inGroup is Medkit medkit)
            {
                medkit.Connect(nameof(Medkit.InventoryToggle), new Callable(this, nameof(ToggleInventoryInterface)));
                GD.Print("Connected to medkit chest: " + medkit.Name);
            }
            else if (inGroup.HasSignal("InventoryToggle"))
            {
                inGroup.Connect("InventoryToggle", new Callable(this, nameof(ToggleInventoryInterface)));
                GD.Print("Connected to external inventory: " + inGroup.Name);
            }
        }
    }
    
    // Handle player inventory toggle (no external inventory)
    private void ToggleInventoryInterface()
    {
        inventoryInterface.Visible = !inventoryInterface.Visible;

        if (inventoryInterface.Visible)
            Input.MouseMode = Input.MouseModeEnum.Visible;
        else
            Input.MouseMode = Input.MouseModeEnum.Captured;
    }

  // Handle external inventory toggle (medkit chest, actual chests, etc.)
  private void ToggleInventoryInterface(InventoryData externalInventory)
  {
    inventoryInterface.Visible = !inventoryInterface.Visible;

    if (inventoryInterface.Visible)
    {
      Input.MouseMode = Input.MouseModeEnum.Visible;
      GD.Print("Opening external inventory: " + externalInventory.ToString());

      // Set the external inventory when opening
      ((InventoryInterface)inventoryInterface).SetExternalInventory(externalInventory);
    }
    else
    {
      Input.MouseMode = Input.MouseModeEnum.Captured;
      GD.Print("Closing external inventory");

      // Clear/hide the external inventory when closing
      // You might need to add a method to hide it, or pass null
      ((InventoryInterface)inventoryInterface).ClearExternalInventory(externalInventory);
    }
    if (externalInventory != null && inventoryInterface.Visible)
    {
      GD.Print("External inventory toggled: " + externalInventory.ToString());
      ((InventoryInterface)inventoryInterface).SetExternalInventory(externalInventory);
    }
    else
    {
      GD.Print("No external inventory to toggle");
      ((InventoryInterface)inventoryInterface).ClearExternalInventory(externalInventory);
    }
}
}
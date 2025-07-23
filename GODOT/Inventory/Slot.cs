using Godot;

public partial class Slot : PanelContainer
{
    [Signal] public delegate void SlotClickedEventHandler(int index, int button);
    [Export] private TextureRect textureRect;
    [Export] private Label quantityLabel;
    private int index;
    public override void _Ready()
    {
        // Get the nodes if they're not assigned in the editor
        if (textureRect == null)
            textureRect = GetNode<TextureRect>("MarginContainer/TextureRect");
        if (quantityLabel == null)
            quantityLabel = GetNode<Label>("QuantityLabel");
    }


    public void SetSlotData(SlotData slotData)
    {
        // Handle null SlotData (empty slot)
        if (slotData == null)
        {
            ClearSlot();
            return;
        }

        // Handle SlotData with null ItemData (also empty slot)
        if (slotData.ItemData == null)
        {
            ClearSlot();
            return;
        }

        // Check if UI elements exist
        if (textureRect == null)
        {
            GD.PrintErr("TextureRect is null!");
            return;
        }

        if (quantityLabel == null)
        {
            GD.PrintErr("QuantityLabel is null!");
            return;
        }

        // Set the slot data
        var itemData = slotData.ItemData;
        textureRect.Texture = itemData.Texture;
        TooltipText = $"{itemData.Name}\n{itemData.Description}";

        //GD.Print($"Setting slot data: {itemData.Name}, Quantity: {slotData.Quantity}");

        if (slotData.Quantity > 1)
        {
            quantityLabel.Text = $"x{slotData.Quantity}";
            quantityLabel.Show();
        }
        else
        {
            quantityLabel.Hide();
        }
    }

    private void ClearSlot()
    {
        if (textureRect != null)
            textureRect.Texture = null;

        if (quantityLabel != null)
            quantityLabel.Hide();

        TooltipText = "";
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButtonEvent)
        {
            if (mouseButtonEvent.ButtonIndex == MouseButton.Left && mouseButtonEvent.Pressed ||
                mouseButtonEvent.ButtonIndex == MouseButton.Right && mouseButtonEvent.Pressed)
            {
                // Handle right click logic here
                EmitSignal(SignalName.SlotClicked, GetIndex(), (int)mouseButtonEvent.ButtonIndex);
            }
        }
    }
    public void SetIndex(int index)
{
    this.index = index;
}
}
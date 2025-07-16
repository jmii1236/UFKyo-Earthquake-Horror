using Godot;
using System;

[GlobalClass]
public partial class SlotData : Resource
{
    const int MAX_STACK_SIZE = 99;
    [Export] public ItemData ItemData { get; set; }
    private int _quantity = 1;
    [Export]
    public int Quantity
    {
        get => _quantity;
        set => SetQuantity(value);
    }
    [Export] public int stackSizeMin { get; set; } = 1;
    [Export] public int stackSizeMax = MAX_STACK_SIZE;

    public void SetQuantity(int quantity)
    {
        // Check if ItemData exists before accessing its properties
        if (ItemData != null && quantity > 1 && !ItemData.Stackable)
        {
            _quantity = 1;
            GD.PrintErr("Item is not stackable, setting quantity to 1.");
        }
        else
        {
            _quantity = quantity;
        }
    }
    

}

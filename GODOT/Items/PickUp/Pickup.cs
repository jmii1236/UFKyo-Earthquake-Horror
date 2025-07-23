using Godot;

public partial class Pickup : RigidBody3D
{
    [Export] public SlotData SlotData { get; set; }
    
    private Sprite3D sprite3d;

  public override void _Ready()
{
    sprite3d = GetNode<Sprite3D>("Sprite3D");
    sprite3d.Texture = SlotData.ItemData.Texture;
    AddToGroup("ExternalInventory");
    
    var area3d = GetNode<Area3D>("Area3D");
    area3d.BodyEntered += _on_area_3d_body_entered;
    
    var collisionShape = area3d.GetNode<CollisionShape3D>("CollisionShape3D2");
}
    
    public override void _PhysicsProcess(double delta)
    {
        sprite3d.Rotate(Vector3.Up, (float)delta);
    }

  private void _on_area_3d_body_entered(Node3D body)
{
    
    PlayerDad player = body as PlayerDad;
    if (player == null)
    {
        GD.Print("Body is not PlayerDad");
        return;
    }
    
    GD.Print("Player detected!");
    
    if (player.InventoryData == null)
    {
        GD.Print("Player has no InventoryData");
        return;
    }
    
    
    if (player.InventoryData.PickUpSlotData(SlotData))
    {
        GD.Print("Item picked up successfully!");
        QueueFree();
    }
    else
    {
        GD.Print("Failed to pick up item - inventory might be full");
    }
}
    
}
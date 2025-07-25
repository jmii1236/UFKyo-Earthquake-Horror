using Godot;
using Microsoft.VisualBasic;
using System;
using System.Formats.Asn1;

public partial class PlayerDad : CharacterBody3D
{
	[Signal] public delegate void ToggleInventoryEventHandler();

	[Signal] public delegate void HealthChangeEventHandler(int newHealth); 
	[Export] public InventoryData InventoryData { get; set; }


	private float MouseSensitivity = 0.001f;
	private float TwistInput = 0.0f;
	private float PitchInput = 0.0f;
	private float Speed = 5.0f;
	private float JumpVelocity = 4.5f;
	private float Gravity = 9.8f;
	private bool Crawling = false;
	private Camera3D Camera;
	private Node3D TwistPivot;
	private Node3D PitchPivot;
	private Globals globals = null;
	private RayCast3D raycast;
	private bool is_holding_item = false;
	[Export] public int MaxHealth = 100; // Set a default maximum health value;
	public int CurrentHealth;

	private float PitchAngle = 0.0f; // to track the current pitch for clamping

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		TwistPivot = GetNode<Node3D>("TwistPivot");
		PitchPivot = TwistPivot.GetNode<Node3D>("PitchPivot");
		Camera = PitchPivot.GetNode<Camera3D>("Camera3D");

		globals = Globals.Instance;
		raycast = GetNode("%ItemChecker") as RayCast3D;
		PlayerDad player = GetNode<PlayerDad>("../PlayerDad");
		if (player == null)
		{
			GD.PrintErr("Could not find PlayerDad node. (PlayerDad.cs)");
			return;
		}
		CurrentHealth = MaxHealth;
	}

	public override void _UnhandledKeyInput(InputEvent @event)
	{
		if (Input.IsActionJustPressed("inventory"))
		{
			EmitSignal(SignalName.ToggleInventory);
			Interaction();
		}
	}


	public override void _PhysicsProcess(double delta)
	{
		Vector3 input = Vector3.Zero;
		input.X = Input.GetAxis("move_left", "move_right");
		input.Z = Input.GetAxis("move_forward", "move_backward");

		// Rotate input relative to player facing direction (TwistPivot)
		Vector3 direction = (TwistPivot.Basis * input).Normalized();
		Vector3 V = Velocity;
		V.X = direction.X * Speed;
		V.Z = direction.Z * Speed;

		// Gravity
		if (!IsOnFloor())
		{
			V.Y -= Gravity * (float)delta;
		}

		// Jump, crawl mechanics
		if (IsOnFloor())
		{
			if (Input.IsActionJustPressed("jump"))
			{
				V.Y = JumpVelocity;
			}
			else if (Input.IsActionJustPressed("crawl"))
			{
				Crawling = !Crawling;
				if (Crawling)
				{
					Camera.Position = new Vector3(0, 0.1f, 0);
				}
				else
				{
					Camera.Position = new Vector3(0, 0.5f, 0);
				}
			}
		}

		if (Input.IsActionJustPressed("ui_cancel"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}

		if (raycast.IsColliding())
		{
			Node3D Collider = raycast.GetCollider() as Node3D;

			if (Input.IsActionJustPressed("interact"))
			{
				// Handle regular Item interactions (picking up items) - legacy system
				if (Collider.IsInGroup("Item"))
				{
					Item item = Collider as Item;
					_On_Item_Interaction(item);
				}
				// Handle External Inventory interactions (medkit chest, actual chests, etc.)
				else if (Collider.IsInGroup("ExternalInventory"))
				{
					// Check if it's a medkit acting as a chest
					if (Collider is Medkit medkit)
					{
						medkit.PlayerInteract();
					}
					// Handle other external inventory objects
					else if (Collider.HasMethod("PlayerInteract"))
					{
						Collider.Call("PlayerInteract");
					}
				}
			}
		}

		// Apply horizontal rotation
		TwistPivot.RotateY(TwistInput);

		// Apply vertical pitch with clamping
		PitchAngle = Mathf.Clamp(PitchAngle + PitchInput, Mathf.DegToRad(-30), Mathf.DegToRad(30));
		PitchPivot.Rotation = new Vector3(PitchAngle, 0, 0);

		Velocity = V;
		MoveAndSlide();

		// Reset inputs each frame
		TwistInput = 0.0f;
		PitchInput = 0.0f;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotion)
		{
			TwistInput = -mouseMotion.Relative.X * MouseSensitivity;
			PitchInput = -mouseMotion.Relative.Y * MouseSensitivity;
		}
	}

	private void _On_Item_Interaction(Node3D item)
	{
		if (is_holding_item) return;

		item.Reparent(this);
		Vector3 NewPosition = new Vector3(0, 0, 0);

		item.Position = NewPosition;

		is_holding_item = true;
	}

	public void Interaction()
	{
		if (raycast.IsColliding())
		{
			Node3D collider = raycast.GetCollider() as Node3D;

			if (collider is Pickup)
			{
				GD.Print("- This is a Pickup item");
			}
			else if (collider.IsInGroup("Item"))
			{
				GD.Print("- This is a pickupable Item");
			}
			else if (collider.IsInGroup("ExternalInventory"))
			{
				if (collider is Medkit)
				{
					GD.Print("- This is a Medkit chest");
				}
				else
				{
					GD.Print("- This is an External Inventory container");
				}
			}
		}
		else
		{
			GD.Print("Raycast is not colliding with anything");
		}
	}

	public void TakeDamage(int damage)
	{
		if (CurrentHealth <= 0)
		{
			GD.Print("PlayerDad is already dead. Cannot take more damage.");
			return;
		}

		CurrentHealth -= damage;
		if (CurrentHealth < 0) CurrentHealth = 0;

		//GD.Print($"PlayerDad took {damage} damage. Current Health: {CurrentHealth}");
		EmitSignal(SignalName.HealthChange, CurrentHealth);

		// Emit signal for health change

		if (CurrentHealth <= 0)
		{
			GD.Print("PlayerDad has died.");
		}
	}
	public void _on_area_3d_area_entered(Node3D body)
	{
		//GD.Print("Current health: " + CurrentHealth);
			GD.Print("Entered by non-PlayerDad: " + body.Name);
			TakeDamage(5); // Example damage value
}
}
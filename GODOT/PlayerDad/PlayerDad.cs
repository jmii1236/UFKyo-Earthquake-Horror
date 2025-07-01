using Godot;
using System;

public partial class PlayerDad : CharacterBody3D
{
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
	private float PitchAngle = 0.0f; // to track the current pitch for clamping
	private Area3D FireDetector = null;

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		TwistPivot = GetNode<Node3D>("TwistPivot");
		PitchPivot = TwistPivot.GetNode<Node3D>("PitchPivot");
		Camera = PitchPivot.GetNode<Camera3D>("Camera3D");

		globals = GetNode("/root/Globals") as Globals;
		raycast = GetNode("%ItemChecker") as RayCast3D;
		FireDetector = GetNode("FireDetector") as Area3D;

		FireDetector.BodyEntered += _On_Body_Entered;
		globals.Death += _On_Death;
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

		// Jump, crawl mechanics
		//if (GetContactCount() > 0)
		//{
		//if (Input.IsActionJustPressed("jump"))
		//{
		//ApplyCentralImpulse(new Vector3(0, JumpVelocity, 0));
		//}
		//else if (Input.IsActionJustPressed("crawl"))
		//{
		//Crawling = !Crawling;
		//if (Crawling)
		//{
		//Camera.Position = new Vector3(0, 0.1f, 0);
		//}
		//else
		//{
		//Camera.Position = new Vector3(0, 0.5f, 0);
		//}
		//}
		//}

		if (Input.IsActionJustPressed("ui_cancel"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}

		if (raycast.IsColliding())
		{
			Node3D Collider = raycast.GetCollider() as Node3D;
			if (Collider.IsInGroup("Item") && Input.IsActionJustPressed("interact"))
			{
				Item item = Collider as Item;
				_On_Item_Interaction(item);
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

	private void _On_Body_Entered(Node3D body)
	{
		if (body.IsInGroup("Fire"))
		{
			_On_Death();
		}
	}

	private void _On_Death()
	{
		EmitSignal("GameOver");
		GD.Print("Game Over, Died");
	}
}

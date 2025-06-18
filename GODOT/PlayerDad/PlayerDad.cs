using Godot;
using System;

public partial class PlayerDad : RigidBody3D
{
	private float MouseSensitivity = 0.001f;
	private float TwistInput = 0.0f;
	private float PitchInput = 0.0f;
	private float JumpVelocity = 4.5f;
	private float Gravity = 9.8f;
	private bool Crawling = false;
	private Camera3D Camera;
	private Node3D TwistPivot;
	private Node3D PitchPivot;

	private float PitchAngle = 0.0f; // to track the current pitch for clamping

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		TwistPivot = GetNode<Node3D>("TwistPivot");
		PitchPivot = TwistPivot.GetNode<Node3D>("PitchPivot");
		Camera = PitchPivot.GetNode<Camera3D>("Camera3D");
	}

	public override void _Process(double delta)
	{
		Vector3 input = Vector3.Zero;
		input.X = Input.GetAxis("move_left", "move_right");
		input.Z = Input.GetAxis("move_forward", "move_backward"); 

		// Rotate input relative to player facing direction (TwistPivot)
		Vector3 direction = TwistPivot.Basis * input;
		ApplyCentralForce(direction * 1200.0f * (float)delta);
		
		// Jump, crawl mechanics
		if (GetContactCount() > 0) {
			if (Input.IsActionJustPressed("jump")) {
				ApplyCentralImpulse(new Vector3(0, JumpVelocity, 0));
			}
			else if (Input.IsActionJustPressed("crawl")) {
				Crawling = !Crawling;
				if (Crawling) {
					Camera.Position = new Vector3(0, 0.1f, 0);
				}
				else {
					Camera.Position = new Vector3(0, 0.5f, 0);
				}
			}
		}

		if (Input.IsActionJustPressed("ui_cancel"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		
		
		// Apply horizontal rotation
		TwistPivot.RotateY(TwistInput);

		// Apply vertical pitch with clamping
		PitchAngle = Mathf.Clamp(PitchAngle + PitchInput, Mathf.DegToRad(-30), Mathf.DegToRad(30));
		PitchPivot.Rotation = new Vector3(PitchAngle, 0, 0);

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
}

using Godot;

public partial class FireAI : CharacterBody3D
{
	private float speed = 2.0f;
	private float accel = 10.0f;

	private NavigationAgent3D nav;
	private RigidBody3D playerDad;

	private PackedScene fireTrailScene;
	private float trailTimer = 0.0f;
	private float trailInterval = 0.2f;

	public override void _Ready()
	{
		nav = GetNode<NavigationAgent3D>("NavigationAgent3D");

		// Find PlayerDad directly
		playerDad = GetNode<RigidBody3D>("../PlayerDad");

		fireTrailScene = GD.Load<PackedScene>("res://FireAI/fire_trail.tscn");

		if (fireTrailScene == null)
			GD.PushError("FireTrail.tscn could not be loaded. Check the path.");

	}


	public override void _PhysicsProcess(double delta)
	{
		// Make sure we have a valid target
		if (playerDad == null)
			return;


		Vector3 direction = Vector3.Zero;

		nav.TargetPosition = playerDad.GlobalPosition; // Offset slightly to avoid collision;

		// Check if we have a valid path
		if (nav.IsNavigationFinished())
			return;

		direction = nav.GetNextPathPosition() - GlobalPosition;
		direction = direction.Normalized();

		Velocity = Velocity.Lerp(direction * speed, (float)(accel * delta));

		MoveAndSlide();

		trailTimer -= (float)delta;
		if (trailTimer <= 0.0f)
		{
			SpawnFireTrail();
			trailTimer = trailInterval;
		}
	}
	private void SpawnFireTrail()
	{
		Node3D trail = fireTrailScene.Instantiate<Node3D>();
		trail.GlobalPosition = GlobalPosition;

		// Add it to the root of the scene, NOT under FireAI
		GetTree().CurrentScene.AddChild(trail);
		GD.Print("Spawned fire trail at: " + GlobalPosition);
 
	}
}

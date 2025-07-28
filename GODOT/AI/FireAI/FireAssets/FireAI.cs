using Godot;

public partial class FireAI : CharacterBody3D
{
	private float speed = 2.0f;
	private float accel = 10.0f;

	private NavigationAgent3D nav;
	private CharacterBody3D playerDad;

	private PackedScene fireTrailScene;
	private float trailTimer = 0.0f;
	private float trailInterval = 0.2f;
	Globals globals = null;

	public override void _Ready()
	{
		nav = GetNode<NavigationAgent3D>("NavigationAgent3D");

		// Find PlayerDad directly
		//playerDad = GetNode<CharacterBody3D>("../PlayerDad");

		fireTrailScene = GD.Load<PackedScene>("res://AI/FireAI/FireAssets/fire_trail.tscn");

		if (fireTrailScene == null)
			GD.PushError("FireTrail.tscn could not be loaded. Check the path.");

		globals = Globals.Instance;
		globals.SetPlayer += SetPlayer;
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
		if (fireTrailScene == null)
		{
			GD.PushError("fireTrailScene is null! Cannot spawn fire trail.");
			return;
		}

		Node3D trail = fireTrailScene.Instantiate<Node3D>();

		if (trail == null)
		{
			GD.PushError("Failed to instance fireTrailScene.");
			return;
		}

		GetTree().CurrentScene.AddChild(trail); // add to scene first
		trail.GlobalPosition = GlobalPosition;  // then set position
		// GD.Print("Spawned fire trail at: " + GlobalPosition);
	}

	private void SetPlayer(CharacterBody3D player)
	{
		playerDad = player;
	}
}

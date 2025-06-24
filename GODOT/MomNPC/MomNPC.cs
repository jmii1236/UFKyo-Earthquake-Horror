using Godot;

public partial class MomNPC : CharacterBody3D
{
	private float speed = 2.0f;
	private float accel = 10.0f;
	
	private NavigationAgent3D nav;
	private RigidBody3D playerDad;
	
	public override void _Ready()
	{
		nav = GetNode<NavigationAgent3D>("NavigationAgent3D");
		
		// Find PlayerDad directly
		playerDad = GetNode<RigidBody3D>("../PlayerDad");
	}
	
	public override void _PhysicsProcess(double delta)
	{
		// Make sure we have a valid target
		if (playerDad == null)
			return;


		Vector3 direction = Vector3.Zero;

		nav.TargetPosition = playerDad.GlobalPosition - Vector3.One; // Offset slightly to avoid collision;

		// Check if we have a valid path
		if (nav.IsNavigationFinished())
			return;

		direction = nav.GetNextPathPosition() - GlobalPosition;
		direction = direction.Normalized();

		Velocity = Velocity.Lerp(direction * speed, (float)(accel * delta));
    
		direction = Vector3.Zero;
		
		nav.TargetPosition = playerDad.GlobalPosition;
		
		// Check if we have a valid path
		if (nav.IsNavigationFinished())
			return;
		
		direction = nav.GetNextPathPosition() - GlobalPosition;
		direction = direction.Normalized();
		
		Velocity = Velocity.Lerp(direction * speed, (float)(accel * delta));
		
	
		MoveAndSlide();
	}
}

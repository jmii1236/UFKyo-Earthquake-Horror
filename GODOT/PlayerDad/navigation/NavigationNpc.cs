using Godot;

public partial class NavigationNpc : CharacterBody3D
{
	private NavigationAgent3D _navigationAgent;
	private float _speed = 5.0f;
	private float _gravity = 9.8f;
	
	public override void _Ready()
	{
		_navigationAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
		
		// Configure the navigation agent
		_navigationAgent.PathDesiredDistance = 0.5f;
		_navigationAgent.TargetDesiredDistance = 0.5f;
		
		// Wait for the navigation map to be ready
		CallDeferred(MethodName.ActorSetup);
	}
	
	private async void ActorSetup()
	{
		// Wait for the first physics frame so the NavigationServer can sync
		await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
		
		// Set initial target to current position
		_navigationAgent.TargetPosition = GlobalPosition;
	}
	
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_accept"))
		{
			// Generate random position on the ground plane (Y = 0)
			var randomPosition = new Vector3(
				(float)GD.RandRange(-5.0, 5.0),
				0.0f, // Keep Y at ground level
				(float)GD.RandRange(-5.0, 5.0)
			);
			_navigationAgent.TargetPosition = randomPosition;
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		// Apply gravity
		if (!IsOnFloor())
		{
			Velocity = new Vector3(Velocity.X, Velocity.Y - _gravity * (float)delta, Velocity.Z);
		}
		
		// Handle navigation
		if (_navigationAgent.IsNavigationFinished())
		{
			// Stop moving when destination is reached
			Velocity = new Vector3(0, Velocity.Y, 0);
		}
		else
		{
			// Get the next position in the path
			var currentAgentPosition = GlobalTransform.Origin;
			var nextPathPosition = _navigationAgent.GetNextPathPosition();
			
			// Calculate direction (only X and Z, preserve Y for gravity)
			var direction = (nextPathPosition - currentAgentPosition);
			direction.Y = 0; // Don't move vertically for navigation
			direction = direction.Normalized();
			
			// Apply movement velocity
			Velocity = new Vector3(
				direction.X * _speed,
				Velocity.Y, // Preserve Y velocity for gravity/jumping
				direction.Z * _speed
			);
		}
		
		MoveAndSlide();
	}
}

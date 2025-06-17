using Godot;

public partial class NavigationNpc : CharacterBody3D
{
	private NavigationAgent3D _navigationAgent;

	public override void _Ready()
	{
		_navigationAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
	}
	
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_accept"))
		{
			var randomPosition = Vector3.Zero;
			randomPosition.X = (float)GD.RandRange(-5.0, 5.0);
			randomPosition.Z = (float)GD.RandRange(-5.0, 5.0);
			_navigationAgent.TargetPosition = randomPosition;
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		var destination = _navigationAgent.GetNextPathPosition();
		var direction = (destination - GlobalPosition).Normalized();

		Velocity = direction * 5.0f;
		MoveAndSlide();
	}
}

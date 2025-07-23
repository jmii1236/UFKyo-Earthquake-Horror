using Godot;

public partial class ChildNpc : CharacterBody3D
{
	private float speed = 1.3f;
	private float accel = 7.0f;

	private NavigationAgent3D nav;
	[Export] private CharacterBody3D playerDad;
	
	// Animation settings - exportable for easy configuration
	[Export] private string SlowRunAnimationName = "BoyAnimations/SlowRun";
	[Export] private string IdleAnimationName = "BoyAnimations/Idle";
	[Export] private string AnimationPlayerPath = "AnimationPlayer";
	[Export] private float MovementThreshold = 0.1f; // Speed threshold to switch between idle and run
	
	// Animation reference
	private AnimationPlayer animationPlayer;
	private string currentAnimation = "";
	private bool isMoving = false;

	public override void _Ready()
	{
		// Initialize NavigationAgent3D first
		nav = GetNode<NavigationAgent3D>("NavigationAgent3D");
		
		if (nav == null)
		{
			GD.PrintErr("NavigationAgent3D node not found! Make sure it exists as a child node.");
		}
		
		// Check if playerDad is assigned
		if (playerDad == null)
		{
			GD.PrintErr("playerDad is not assigned! Please assign it in the editor.");
		}
		
		// Find and initialize AnimationPlayer
		FindAnimationPlayer();
		
		// Start with idle animation
		PlayAnimation(IdleAnimationName);
	}

	private void FindAnimationPlayer()
	{
		// Try the configured path first
		if (!string.IsNullOrEmpty(AnimationPlayerPath))
		{
			animationPlayer = GetNodeOrNull<AnimationPlayer>(AnimationPlayerPath);
		}

		// If not found, try common paths
		if (animationPlayer == null)
		{
			string[] commonPaths = {
				"AnimationPlayer",
				"MeshInstance3D/AnimationPlayer",
				"Body/AnimationPlayer",
				"Model/AnimationPlayer",
				"Character/AnimationPlayer",
				"BoyModel/AnimationPlayer",
				"Boy/AnimationPlayer"
			};

			foreach (string path in commonPaths)
			{
				animationPlayer = GetNodeOrNull<AnimationPlayer>(path);
				if (animationPlayer != null)
				{
					GD.Print($"Found AnimationPlayer at: {path}");
					break;
				}
			}
		}

		// Last resort: recursive search
		if (animationPlayer == null)
		{
			animationPlayer = FindAnimationPlayerRecursive(this);
		}

		if (animationPlayer != null)
		{
			// Debug: Print available animations
			var animations = animationPlayer.GetAnimationList();
			GD.Print("Available animations:");
			foreach (string anim in animations)
			{
				GD.Print($"  - {anim}");
			}
		}
		else
		{
			GD.PrintErr("AnimationPlayer not found! Please check the scene structure.");
		}
	}

	private AnimationPlayer FindAnimationPlayerRecursive(Node node)
	{
		if (node is AnimationPlayer player)
		{
			return player;
		}
		
		foreach (Node child in node.GetChildren())
		{
			var result = FindAnimationPlayerRecursive(child);
			if (result != null)
			{
				return result;
			}
		}
		
		return null;
	}

	public override void _PhysicsProcess(double delta)
	{
		// Make sure we have valid references
		if (playerDad == null)
		{
			GD.PrintErr("playerDad is null! Please assign it in the editor or via code.");
			return;
		}
		
		if (nav == null)
		{
			GD.PrintErr("NavigationAgent3D is null! Make sure it exists in the scene.");
			return;
		}

		Vector3 direction = Vector3.Zero;

		nav.TargetPosition = playerDad.GlobalPosition;

		// Check if we have a valid path
		if (nav.IsNavigationFinished())
		{
			// No movement needed, should be idle
			isMoving = false;
		}
		else
		{
			direction = nav.GetNextPathPosition() - GlobalPosition;
			direction = direction.Normalized();

			Velocity = Velocity.Lerp(direction * speed, (float)(accel * delta));
			
			// Check if we're actually moving
			isMoving = Velocity.Length() > MovementThreshold;
		}

		MoveAndSlide();
		
		// Update animations based on movement
		UpdateAnimationState();
	}
	
	private void UpdateAnimationState()
	{
		if (animationPlayer == null) return;
		
		// Determine which animation should be playing
		string targetAnimation = isMoving ? SlowRunAnimationName : IdleAnimationName;
		
		// Switch animations if needed
		if (targetAnimation != currentAnimation)
		{
			PlayAnimation(targetAnimation);
		}
		
		// Adjust animation speed based on movement speed if running
		if (isMoving && currentAnimation == SlowRunAnimationName)
		{
			float currentSpeed = Velocity.Length();
			float speedMultiplier = Mathf.Clamp(currentSpeed / speed, 0.5f, 1.5f);
			animationPlayer.SpeedScale = speedMultiplier;
		}
		else
		{
			// Normal speed for idle animation
			animationPlayer.SpeedScale = 1.0f;
		}
	}
	
	private void PlayAnimation(string animationName)
	{
		if (animationPlayer == null || string.IsNullOrEmpty(animationName)) return;
		
		// Don't restart the same animation
		if (currentAnimation == animationName && animationPlayer.IsPlaying()) return;
		
		if (animationPlayer.HasAnimation(animationName))
		{
			animationPlayer.Play(animationName);
			currentAnimation = animationName;
			GD.Print($"Playing animation: {animationName}");
		}
		else
		{
			GD.PrintErr($"Animation '{animationName}' not found! Available animations:");
			foreach (string anim in animationPlayer.GetAnimationList())
			{
				GD.Print($"  - {anim}");
			}
		}
	}
	
	// Public method to get current animation state (useful for debugging)
	public string GetCurrentAnimation()
	{
		return currentAnimation;
	}
	
	public bool IsMoving()
	{
		return isMoving;
	}
}
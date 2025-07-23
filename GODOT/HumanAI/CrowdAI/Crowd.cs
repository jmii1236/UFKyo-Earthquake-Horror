using Godot;
using System;

public partial class Crowd : Node3D
{
  // Adjusted weights for panicked behavior - more directional
  [Export] private float AlignmentWeight = 0.8f;     
  [Export] private float CohesionWeight = 0.05f;     
  [Export] private float SeparationWeight = 0.6f;    
  [Export] private float PanicWeight = 0.3f;         
  [Export] private float NeighborRadius = 4.0f;      
  
  // Speed settings for running/panicked movement - now with variation
  [Export] private float MaxSpeed = 4.0f;           
  [Export] private float MinSpeed = 2.0f;            
  [Export] private float SpeedVariation = 0.3f;      
  [Export] private float MaxAcceleration = 1.0f;     
  [Export] private float RotationSpeed = 4.0f;       
  
  // Randomness settings
  [Export] private float MovementNoiseStrength = 1.0f;  
  [Export] private float DirectionChangeFrequency = 2.0f; 
  [Export] private float PersonalityVariation = 0.4f;   
  [Export] private float StumbleChance = 0.02f;        
  
  // Panic-specific settings
  [Export] private float PanicRadius = 6.0f;         
  [Export] private float RandomMovementStrength = 2.0f; 
  [Export] private float DirectionalBias = 2.0f;     
  [Export] private float GlobalDirectionWeight = 1.5f; 
  [Export] private Vector3 EscapeDirection = Vector3.Forward; 

  // Animation settings - All exportable for easy configuration in Godot
  //[Export] private string RunAnimationName = "ANIMATION/Idle";  
  [Export] private string FastRunAnimationName = "OhMyGodPlease/Armature|mixamo_com|Layer0_001";  
  [Export] private string AnimationPlayerPath = "AnimationPlayer";  
  [Export] private float FastRunThreshold = 0.6f;

  private Vector3 alignment = Vector3.Zero;
  private Vector3 cohesion = Vector3.Zero;
  private Vector3 separation = Vector3.Zero;
  private Vector3 panicForce = Vector3.Zero;
  private int count = 0;
  private Godot.Collections.Array<Crowd> unitArray = new Godot.Collections.Array<Crowd>();
  private Vector3 velocity = Vector3.Zero;
  private RandomNumberGenerator rng = new RandomNumberGenerator();
  
  // Animation reference
  private AnimationPlayer animationPlayer;
  
  // Panic state variables
  private float panicLevel = 1.0f;
  private float panicDecayRate = 0.5f;
  
  // Individual personality traits (randomized per boid)
  private float personalMaxSpeed;
  private float personalMinSpeed;
  private float personalPanicSensitivity;
  private float personalDirectionalBias;
  private Vector3 personalMovementOffset = Vector3.Zero;
  private float directionChangeTimer = 0.0f;
  private float stumbleTimer = 0.0f;
  private bool isStumbling = false;
  
  // Animation state tracking
  private string currentAnimation = "";
  private bool isInFastRun = false;

  public override void _Ready()
  {
    // Initialize random number generator
    rng.Randomize();

    // Find AnimationPlayer
    FindAnimationPlayer();

    // Randomize individual personality traits
    personalMaxSpeed = MaxSpeed * (1.0f + rng.RandfRange(-SpeedVariation, SpeedVariation));
    personalMinSpeed = MinSpeed * (1.0f + rng.RandfRange(-SpeedVariation, SpeedVariation));
    personalPanicSensitivity = 1.0f + rng.RandfRange(-PersonalityVariation, PersonalityVariation);
    personalDirectionalBias = DirectionalBias * (1.0f + rng.RandfRange(-PersonalityVariation, PersonalityVariation));

    // Initialize direction change timer
    directionChangeTimer = rng.RandfRange(0.0f, 1.0f / DirectionChangeFrequency);

    // Start with some initial panic velocity (with personal variation)
    velocity = new Vector3(
        rng.RandfRange(-1f, 1f),
        0,
        rng.RandfRange(-1f, 1f)
    ).Normalized() * personalMinSpeed;

    // Find all other Crowd nodes in the scene
    GetTree().CallGroup("boids", "RegisterBoid", this);

    // Start with the normal running animation
    //PlayAnimation(RunAnimationName);
    PlayAnimation(FastRunAnimationName);
    
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
        "MANPOSE/AnimationPlayer",
        "running/AnimationPlayer",
        "MeshInstance3D/AnimationPlayer",
        "Body/AnimationPlayer"
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
      if (animationPlayer != null)
      {
        
      }
    }

    if (animationPlayer != null)
    {
      // Debug: Print available animations
      var animations = animationPlayer.GetAnimationList();
      
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

  public void RegisterBoid(Crowd boid)
  {
    if (boid != this)
    {
      unitArray.Add(boid);
    }
  }

  public override void _PhysicsProcess(double delta)
  {
    // Reset values
    alignment = Vector3.Zero;
    cohesion = Vector3.Zero;
    separation = Vector3.Zero;
    panicForce = Vector3.Zero;
    count = 0;
    
    int nearbyCount = 0;

    // Calculate boid behaviors
    foreach (var unit in unitArray)
    {
      if (unit == null || unit == this) continue;

      var distance = GlobalPosition.DistanceTo(unit.GlobalPosition);
      
      // Regular boid behaviors
      if (distance < NeighborRadius && distance > 0)
      {
        alignment += unit.velocity;
        cohesion += unit.GlobalPosition;
        separation += (GlobalPosition - unit.GlobalPosition) / distance;
        count++;
      }
      
      // Panic behavior - count nearby units
      if (distance < PanicRadius)
      {
        nearbyCount++;
      }
    }

    // Calculate panic level based on crowding (with personal sensitivity)
    float targetPanic = Mathf.Clamp((nearbyCount / 8.0f) * personalPanicSensitivity, 0.0f, 1.0f);
    panicLevel = Mathf.Lerp(panicLevel, targetPanic, (float)delta * 2.0f);

    if (count > 0)
    {
      CalcAlignment();
      CalcCohesion();
      CalcSeparation();
    }
    
    // Add panic behaviors
    CalcPanicForce();
    
    // Update movement noise and direction changes
    UpdateMovementNoise(delta);
    UpdateStumbling(delta);

    // Apply forces with panic modifiers
    var totalForce = (alignment * AlignmentWeight + 
                     cohesion * CohesionWeight + 
                     separation * SeparationWeight +
                     panicForce * PanicWeight) * (float)delta;

    velocity += totalForce;

    // Add strong directional bias (escape direction) with personal variation
    Vector3 globalDirection = EscapeDirection.Normalized() * GlobalDirectionWeight * (float)delta;
    velocity += globalDirection;
    
    // Additional personal directional bias
    velocity += EscapeDirection.Normalized() * personalDirectionalBias * (float)delta;
    
    // Add continuous movement noise
    velocity += personalMovementOffset * MovementNoiseStrength * (float)delta;

    // Add erratic movement when panicked
    if (panicLevel > 0.3f)
    {
      Vector3 randomForce = new Vector3(
          rng.RandfRange(-1f, 1f),
          0,
          rng.RandfRange(-1f, 1f)
      ).Normalized() * RandomMovementStrength * panicLevel * (float)delta;
      
      velocity += randomForce;
    }

    // Clamp velocity to personal speed limits (adjusted for panic and stumbling)
    float currentMaxSpeed = personalMaxSpeed * (1.0f + panicLevel * 0.5f);
    float currentMinSpeed = personalMinSpeed;
    
    // Reduce speed if stumbling
    if (isStumbling)
    {
      currentMaxSpeed *= 0.3f;
      currentMinSpeed *= 0.3f;
    }
    
    if (velocity.Length() > currentMaxSpeed)
    {
      velocity = velocity.Normalized() * currentMaxSpeed;
    }
    else if (velocity.Length() < currentMinSpeed)
    {
      // Maintain minimum running speed
      velocity = velocity.Normalized() * currentMinSpeed;
    }

    // Apply movement
    GlobalPosition += velocity * (float)delta;

    // Update animation based on panic level and speed
    UpdateAnimationState();

    // Rotate to face movement direction (more erratic when panicked)
    Vector3 direction = velocity.Normalized();
    if (direction != Vector3.Zero)
    {
      var targetTransform = Transform3D.Identity;
      targetTransform.Origin = GlobalPosition;
      
      // Add some rotation wobble when panicked
      if (panicLevel > 0.5f)
      {
        var wobble = new Vector3(
            rng.RandfRange(-0.2f, 0.2f) * panicLevel,
            0,
            rng.RandfRange(-0.2f, 0.2f) * panicLevel
        );
        direction += wobble;
        direction = direction.Normalized();
      }
      
      targetTransform = targetTransform.LookingAt(GlobalPosition + direction, Vector3.Up);

      float rotSpeed = RotationSpeed * (1.0f + panicLevel);
      GlobalTransform = GlobalTransform.InterpolateWith(targetTransform, rotSpeed * (float)delta);
    }
  }

  private void CalcAlignment()
  {
    alignment /= count;
    alignment = alignment.Normalized() * MaxSpeed - velocity;
    if (alignment.Length() > MaxAcceleration)
    {
      alignment = alignment.Normalized() * MaxAcceleration;
    }
  }

  private void CalcCohesion()
  {
    cohesion = (cohesion / count) - GlobalPosition;
    cohesion = cohesion.Normalized() * MaxSpeed - velocity;
    if (cohesion.Length() > MaxAcceleration)
    {
      cohesion = cohesion.Normalized() * MaxAcceleration;
    }
  }

  private void CalcSeparation()
  {
    separation /= count;
    separation = separation.Normalized() * MaxSpeed - velocity;
    if (separation.Length() > MaxAcceleration)
    {
      separation = separation.Normalized() * MaxAcceleration;
    }
  }
  
  private void CalcPanicForce()
  {
    // Create erratic movement patterns with some consistency
    panicForce = personalMovementOffset * panicLevel * 0.5f;
    
    // Add momentum preservation (panicked people don't change direction easily)
    panicForce += velocity.Normalized() * 0.5f;
  }
  
  private void UpdateMovementNoise(double delta)
  {
    // Update direction change timer
    directionChangeTimer -= (float)delta;
    
    if (directionChangeTimer <= 0.0f)
    {
      // Generate new movement offset
      personalMovementOffset = new Vector3(
          rng.RandfRange(-1f, 1f),
          0,
          rng.RandfRange(-1f, 1f)
      ).Normalized();
      
      // Reset timer with some randomness
      directionChangeTimer = (1.0f / DirectionChangeFrequency) * rng.RandfRange(0.5f, 1.5f);
    }
  }
  
  private void UpdateStumbling(double delta)
  {
    if (isStumbling)
    {
      stumbleTimer -= (float)delta;
      if (stumbleTimer <= 0.0f)
      {
        isStumbling = false;
      }
    }
    else
    {
      // Check for stumbling
      if (rng.Randf() < StumbleChance * (float)delta * 60.0f) // Adjust for framerate
      {
        isStumbling = true;
        stumbleTimer = rng.RandfRange(0.2f, 0.8f); // Stumble for 0.2-0.8 seconds
      }
    }
  }
  
  private void UpdateAnimationState()
  {
    if (animationPlayer == null) return;
    
    // Determine which animation should be playing based on panic level
    bool shouldUseFastRun = panicLevel >= FastRunThreshold;
    
    // Switch animations if needed
    if (shouldUseFastRun && !isInFastRun)
    {
      // Switch to fast run
      PlayAnimation(FastRunAnimationName);
      isInFastRun = true;
    }
    else if (!shouldUseFastRun && isInFastRun)
    {
      // Switch back to normal run
      //PlayAnimation(RunAnimationName);
      PlayAnimation(FastRunAnimationName);
      isInFastRun = false;
    }
    
    // Update animation speed based on movement speed
    float currentSpeed = velocity.Length();
    float speedMultiplier = Mathf.Clamp(currentSpeed / MaxSpeed, 0.5f, 2.0f);
    
    // Increase speed multiplier when in fast run mode
    if (isInFastRun)
    {
      speedMultiplier *= 1.3f; // Make fast run even faster
    }
    
    animationPlayer.SpeedScale = speedMultiplier;
    
    // Add some randomness to animation speed for variety
    animationPlayer.SpeedScale *= rng.RandfRange(0.95f, 1.05f);
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
    }
    else
    {

      foreach (string anim in animationPlayer.GetAnimationList())
      {
        GD.Print($"  - {anim}");
      }
    }
  }

  public Vector3 GetVelocity()
  {
    return velocity;
  }
  
  public float GetPanicLevel()
  {
    return panicLevel;
  }

  private void _on_area_3d_body_exited(Node3D body)
  {
      if (body is Crowd crowd)
      {
          unitArray.Remove(crowd);
      }
  }

  public void _on_area_3d_body_entered(Area3D area)
  {
      var parent = area.GetParent();
      if (parent is Crowd crowd && crowd != this)
      {
          if (!unitArray.Contains(crowd))
          {
              unitArray.Add(crowd);
          }
      }
  }

  public void _on_area_3d_body_exited(Area3D area)
  {
      var parent = area.GetParent();
      if (parent is Crowd crowd)
      {
          unitArray.Remove(crowd);
      }
  }
}
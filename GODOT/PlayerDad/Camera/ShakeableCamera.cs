using Godot;
using System;
using System.ComponentModel.DataAnnotations;

public partial class ShakeableCamera : Area3D
{
    [Export] float trauma_reduction_rate = 1.0f;

    [Export] float max_x = 10.0f;
    [Export] float max_y = 10.0f;
    [Export] float max_z = 5.0f;

    [Export] FastNoiseLite noise = null;
    [Export] float noise_speed = 50.0f;

    Camera3D camera = null;
    Vector3 initial_rotation = Vector3.Zero;
    float trauma = 0.0f;
    float time = 0.0f;

    public override void _Ready()
    {
        camera = GetNode<Camera3D>("Camera3D");
        initial_rotation = camera.RotationDegrees;
    }

    public override void _Process(double delta)
    {
        time += (float)delta;
        trauma = Math.Max(trauma - (float)delta * trauma_reduction_rate, 0.0f);

        Vector3 rotation_degrees = Vector3.Zero;
        rotation_degrees.X = initial_rotation.X + max_x * ShakeIntensity() * GetNoiseFromSeed(0);
        rotation_degrees.Y = initial_rotation.Y + max_y * ShakeIntensity() * GetNoiseFromSeed(1);
        rotation_degrees.Z = initial_rotation.Z + max_z * ShakeIntensity() * GetNoiseFromSeed(2);
        camera.SetRotationDegrees(rotation_degrees);
    }

    public void AddTrauma(float trauma_amount)
    {
        trauma = Math.Clamp(trauma + trauma_amount, 0.0f, 1.0f);
    }

    public float ShakeIntensity()
    {
        return trauma * trauma;
    }

    public float GetNoiseFromSeed(int _seed)
    {
        noise.Seed = _seed;
        return noise.GetNoise1D(time * noise_speed);
    }
}

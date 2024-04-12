using Godot;
using System;
using System.Linq;

public partial class Enemy : RigidBody3D
{
	[Export]
	public float speed = 10.0f; // The speed of the enemy
	private Vector3 velocity = new Vector3(); // The current velocity of the enemy
	
	public override void _Ready()
	{
			// Connect the Area3D's body_entered signal to the OnBodyEntered method
			Callable callable = new Callable(this, nameof(OnBodyEntered));
			GetNode<Area3D>("BulletDetector").Connect("body_entered", callable);
	}
		
	public override void _Process(double delta)
	{
		// Move the enemy
		LinearVelocity = velocity;
	}
	
	private void OnBodyEntered(Node body)
	{
		GD.Print("called");
		// If the body is a Bullet
		if (body is bullet bull)
		{
			// Calculate the direction from the bullet to the enemy
			Vector3 direction = GlobalTransform.Origin - bull.GlobalTransform.Origin;

		// Normalize the direction and multiply it by the speed to get the velocity
			velocity = direction.Normalized() * speed;
		}
	}
}


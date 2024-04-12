using Godot;
using System;
using System.Linq;

public partial class Enemy : RigidBody3D
{
    private Vector3 velocity;
    private float speed = 10f;
	Vector3 escapeDirection = new Vector3();
	int bulletCount = 0;
	[Export]
    public Area3D BulletDetector;
	

    public override void _Process(double delta){
		var bullets = BulletDetector.GetOverlappingBodies();
		foreach (bullet Bullet in bullets.OfType<bullet>())
		{
    		// Determine escape direction (perpendicular to bullet's trajectory)
    		escapeDirection += new Vector3(-Bullet.LinearVelocity.Z, 0, Bullet.LinearVelocity.X).Normalized();
    		bulletCount++;
		}
		GD.Print(bulletCount);

		if (bulletCount > 0)
		{
    	// Average the escape directions
    		escapeDirection /= bulletCount;

 	   	// Move the enemy
	    	velocity = escapeDirection * speed;
		}

        // Apply the velocity
        LinearVelocity = velocity;
	}
}

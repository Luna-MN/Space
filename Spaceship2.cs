using System.ComponentModel;
using System.Diagnostics;
using Godot;


public partial class Spaceship2 : RigidBody3D
{
	// Game objects
	[Export]
	public MeshInstance3D sprite;
	[Export]
	public RigidBody3D BG;
	[Export]
	public PackedScene Bullet;
	
	// Game state
	private Timer timer;
	private PhysicsDirectSpaceState3D spaceState;
	private Vector2 mousePos;
	private Vector3 pos;
	private bool Dash;
	private int speed = 1;
	
	// Camera
	private Camera3D cam;
	
	// Raycast
	private Godot.Collections.Dictionary rayA;
	
	// Weapons
	private enum Weps { Standard, Fast, Slow, Burst }
	private Weps Equipped = Weps.Standard;
	private int BurstFire = 0;
	private enum Elements { Fire, Water, Earth, Air };
	private Elements Element = Elements.Fire;
	public bool DoT = false;
	public bool Slow = false;
	public bool Earth = false;
	public bool slow = false;
	private Color color = new Color(1, 0, 0);

	public override void _Ready()
	{
		// Timer for weapon cooldown
        timer = new Timer
        {
            Autostart = true,
			OneShot = true,
            WaitTime = 0.1
        };
		AddChild(timer);
    }

	public override void _Process(double delta)
	{
		// Look at mouse position
		pos = ScreenPointToRay();
		pos.Y = 0;

		sprite.LookAt(pos, Vector3.Up, true);

		var mid = Position.Lerp(pos, 0.5f);
		cam.Position = new Vector3 (mid.X, 70, mid.Z);
		BG.Position = new Vector3 (Position.X, 30, Position.Z);
	}

	// Raycast
	public Vector3 ScreenPointToRay(){
		// Get the mouse position
		spaceState = GetWorld3D().DirectSpaceState;
		mousePos = GetViewport().GetMousePosition();
		// Get the camera
		cam = GetTree().Root.GetCamera3D();
		// Get the ray
		var rayO = cam.ProjectRayOrigin(mousePos);
		var rayE = rayO + cam.ProjectRayNormal(mousePos) * 2000;
		var query = PhysicsRayQueryParameters3D.Create(rayO, rayE);
		query.CollideWithAreas = true;
		// Get the raycast
		rayA = spaceState.IntersectRay(query);
		// Return the position of the raycast
		if(rayA != null)
			return (Vector3)rayA["position"];
		return new Vector3(0,0,0); 
	}

	// Bullet firing
	public void bulletF()
	{
		// Create a bullet
		var fired = Bullet.Instantiate<RigidBody3D>();
		GetTree().Root.AddChild(fired);
		// Set the position of the bullet to the player's position
		fired.Position = Position;
		// Get the direction of the mouse
		var mousePos = ScreenPointToRay();
		mousePos.Y = Position.Y; 
		// Normalize the direction
		var direction = (mousePos - Position).Normalized();
		direction.Y = 0;

		// Set the velocity of the bullet
		fired.LinearVelocity = direction * 50;
	}

	// Weapon switching
	public void ChangeEquipped(Color color){
    	switch (Equipped)
		{
        	case Weps.Standard:
            	timer.WaitTime = 0.2;
            	break;
        	case Weps.Fast:
            	timer.WaitTime = 0.1;
            	break;
        	case Weps.Slow:
				timer.WaitTime = 0.4;
        	    break;
			// Burst fire, works differently than the others firing 5 bullets in quick succession
        	case Weps.Burst:
            	timer.WaitTime = 0.5;
            	break;
    	}
	}

	// Input
    public override void _Input(InputEvent @event)
    {
		if (@event is InputEventKey keyEvent && keyEvent.Pressed){

			// Movement controls, W and S move forward and backward, A and D strafe, might change these later
			if (Input.IsKeyPressed(Key.W)){
				var posi = ScreenPointToRay() - Position;
				LinearVelocity += new Vector3(posi.Normalized().X, 0, posi.Normalized().Z)*speed;
			}
			else if (Input.IsKeyPressed(Key.S)){
				var posi = ScreenPointToRay() - Position;
				LinearVelocity -= new Vector3(posi.Normalized().X, 0, posi.Normalized().Z)*speed;
			}
			else if (Input.IsKeyPressed(Key.A))
        	{
				var posi = (ScreenPointToRay() - Position).Normalized();
        	    LinearVelocity = new Vector3(-posi.Z, 0, posi.X)*speed;
        	}
        	else if (Input.IsKeyPressed(Key.D))
        	{
				var posi = (ScreenPointToRay() - Position).Normalized();
            	LinearVelocity = new Vector3(posi.Z, 0, -posi.X)*speed;
        	}

			// Weapon switching
			if (Input.IsKeyPressed(Key.Q)){
				Equipped -= 1;
				if(Equipped < 0){
					Equipped = 0;
				}
				ChangeEquipped(color);
			}
			if (Input.IsKeyPressed(Key.E)){
				Equipped += 1;
				if(Equipped > (Weps)3){
					Equipped = 0;
				}
				ChangeEquipped(color);
			}

			// Shooting
			if(Input.IsKeyPressed(Key.Space)){
				// If the timer is stopped, fire a bullet
				if(timer.IsStopped()){
					if(Equipped != Weps.Burst){
						bulletF();
						timer.Start();
					}
					// Burst fire, fires 5 bullets in quick succession
					else if(Equipped == Weps.Burst){
						if(BurstFire <= 4){
							BurstFire += 1;
							bulletF();
						}
						else{
							BurstFire = 0;
							timer.Start();
						}
					}
				}

			}
			if(Input.IsKeyPressed(Key.Shift)){
				speed = 3;
			}
			else{
				speed = 1;
			}
		}
    }
}


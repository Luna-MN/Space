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
	
	// Camera
	private Camera3D cam;
	
	// Raycast
	private Godot.Collections.Dictionary rayA;
	
	// Weapons
	private enum Weps { Standard, Fast, Slow, Burst }
	private Weps Equipped = Weps.Standard;
	private int BurstFire = 0;
	public override void _Ready()
	{
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
		pos = ScreenPointToRay();
		pos.Y = 0;

		sprite.LookAt(pos, Vector3.Up, true);
		
		cam.Position = new Vector3 (Position.X, 70, Position.Z);
		BG.Position = new Vector3 (Position.X, -31, Position.Z);
	}
	public Vector3 ScreenPointToRay(){
		spaceState = GetWorld3D().DirectSpaceState;
		mousePos = GetViewport().GetMousePosition();
		cam = GetTree().Root.GetCamera3D();

		var rayO = cam.ProjectRayOrigin(mousePos);
		var rayE = rayO + cam.ProjectRayNormal(mousePos) * 2000;
		var query = PhysicsRayQueryParameters3D.Create(rayO, rayE);
		query.CollideWithAreas = true;
		rayA = spaceState.IntersectRay(query);

		if(rayA != null)
			return (Vector3)rayA["position"];
		return new Vector3(0,0,0); 
	}
	public void bulletF()
	{
		var fired = Bullet.Instantiate<RigidBody3D>();
		GetTree().Root.AddChild(fired);

		fired.Position = Position;
		var mousePos = ScreenPointToRay();
		mousePos.Y = Position.Y; 
		var direction = (mousePos - Position).Normalized();
		direction.Y = 0;

		// velocity is now constant in the direction of the mouse position
		fired.LinearVelocity = direction * 50;
	}
	public void ChangeEquipped(){
    	switch (Equipped)
		{
        	case Weps.Standard:
            	timer.WaitTime = 0.2;
            	break;
        	case Weps.Fast:
            	timer.WaitTime = 0.05;
            	break;
        	case Weps.Slow:
				timer.WaitTime = 0.3;
        	    break;
        	case Weps.Burst:
            	timer.WaitTime = 0.3;
            	break;
    	}
	}
    public override void _Input(InputEvent @event)
    {
		if (@event is InputEventKey keyEvent && keyEvent.Pressed){
			if (Input.IsKeyPressed(Key.W)){
				var posi = ScreenPointToRay() - Position;
				LinearVelocity += new Vector3(posi.Normalized().X, 0, posi.Normalized().Z);
				GD.Print("W");
			}

			else if (Input.IsKeyPressed(Key.S)){
				var posi = ScreenPointToRay() - Position;
				LinearVelocity -= new Vector3(posi.Normalized().X, 0, posi.Normalized().Z);
				GD.Print("S");
			}
			else if (Input.IsKeyPressed(Key.Q)){
				Equipped -= 1;
				if(Equipped < 0){
					Equipped = 0;
				}
				ChangeEquipped();
			}
			else if (Input.IsKeyPressed(Key.E)){
				Equipped += 1;
				if(Equipped > (Weps)3){
					Equipped = 0;
				}
				ChangeEquipped();
			}
			if(Input.IsKeyPressed(Key.Space)){
				GD.Print("m1");
				if(timer.IsStopped()){
					if(Equipped != Weps.Burst){
						bulletF();
						timer.Start();
					}
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
		}
    }
}


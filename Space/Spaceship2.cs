using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Resolvers;

public partial class Spaceship2 : RigidBody3D
{
	[Export]
	public MeshInstance3D sprite;
	[Export]
	public RigidBody3D BG;
	[Export]
	public PackedScene Bullet;
	public PhysicsDirectSpaceState3D spaceState;
	public Vector2 mousePos;
	public Camera3D cam;
	public Godot.Collections.Dictionary rayA;
	public Vector3 pos;

	public override void _Ready()
	{
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
	public void bulletF(){
		var fired = Bullet.Instantiate<RigidBody3D>();
		GetTree().Root.AddChild(fired);
		fired.Position = Position;
		var posi = ScreenPointToRay() - Position;
		MeshInstance3D Bsprite = fired.GetChild<MeshInstance3D>(0);
		Bsprite.LookAt(pos, Vector3.Forward, true);
		GD.Print(Bsprite.Rotation);
		Bsprite.Rotation = new Vector3(0,0, 1.5708f);
		fired.LinearVelocity += new Vector3(posi.Normalized().X*50, 0, posi.Normalized().Z*50);
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
			if(Input.IsKeyPressed(Key.Space)){
				GD.Print("m1");
				bulletF();
			}
		}
    }
}

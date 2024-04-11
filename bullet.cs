using Godot;
using System;

public partial class bullet : RigidBody3D
{
	// Called when the node enters the scene tree for the first time.

	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	private void areaEnter(Node3D body){
	if(body != this){
			GD.Print("hit");
			GD.Print(body);
			QueueFree();
	}


	}
}

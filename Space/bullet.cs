using Godot;
using System;

public partial class bullet : RigidBody3D
{
	private Timer timer;
	private void OnTimerTimeout()
	{
		GD.Print("bullet killed");
		QueueFree();
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timer = new Timer
		{
			WaitTime = 30.0f,
			OneShot = true,
			Autostart = true
		};
		AddChild(timer);
		Callable callable = new Callable(this, nameof(OnTimerTimeout));
		timer.Connect("timeout", callable);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void areaEnter(Node3D body)
	{
		if(body != this)
		{
			GD.Print("hit");
			GD.Print(body);
			QueueFree();
		}
	}


}
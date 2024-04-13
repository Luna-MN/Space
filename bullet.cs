using Godot;
using System;

public partial class bullet : RigidBody3D
{
	private Timer lifetime;
	[Export]
	public PackedScene ParticleScene;
	// bullet lifetime
	private void OnTimerTimeout()
	{
		GD.Print("bullet killed");
		QueueFree();
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// lifetime timer
		lifetime = new Timer
		{
			WaitTime = 30.0f,
			OneShot = true,
			Autostart = true
		};
		AddChild(lifetime);
		Callable callable = new Callable(this, nameof(OnTimerTimeout));
		lifetime.Connect("timeout", callable);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	// Called when the node enters the area
	private void areaEnter(Node3D body)
	{
		if(body != this)
		{
			GD.Print("hit");
			CpuParticles3D particles = ParticleScene.Instantiate<CpuParticles3D>();
        	particles.Position = Position; 
        	GetParent().AddChild(particles);
			particles.OneShot = true;
			QueueFree();
		}
	}
}
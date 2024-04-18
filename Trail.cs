using Godot;
using System;
using System.Collections.Generic;

public partial class Trail : MeshInstance3D
{
    // List of points in the trail
    public List<Vector3> Points { get; set; } = new List<Vector3>();
    // List of widths for each point in the trail
    public List<List<Vector3>> Width { get; set; } = new List<List<Vector3>>();
    // List of life durations for each point in the trail
    public List<float> Life { get; set; } = new List<float>();
    // Last position of the trail
    public Vector3 LastPos { get; set; }
    // SurfaceTool used to create the trail
    public SurfaceTool surfaceTool;

    // Whether the trail is enabled
    [Export]
    public bool TrailEnabled { get; set; } = true;

    // Width of the trail at the start
    [Export]
    public float FromWidth { get; set; } = 0.5f;

    // Width of the trail at the end
    [Export]
    public float ToWidth { get; set; } = 0.0f;

    // Acceleration of the trail's scale
    private float _scaleAcceleration;
    [Export]
    public float ScaleAcceleration
    {
        get { return _scaleAcceleration; }
        set { _scaleAcceleration = Mathf.Clamp(value, 0.5f, 1.5f); }
    }

    // Motion data of the trail
    [Export]
    public float MotionData { get; set; } = 0.1f;

    // Lifespan of the trail
    [Export]
    public float Lifespan { get; set; } = 1.0f;

    // Color of the trail at the start
    [Export]
    public Color StartColor { get; set; } = new Color(1, 1, 1, 1);

    // Color of the trail at the end
    [Export]
    public Color EndColor { get; set; } = new Color(1, 1, 1, 0);

    // Method to add a point to the trail
    public void AppendPoint(){
        Points.Add(Position);
        Width.Add(new List<Vector3>(){GlobalTransform.Basis.X * FromWidth,
            GlobalTransform.Basis.X * FromWidth - GlobalTransform.Basis.X * ToWidth});

        Life.Add(0.0f);
    }

    // Method to remove a point from the trail
    public void RemovePoints(int i){
        Points.RemoveAt(i);
        Width.RemoveAt(i);
        Life.RemoveAt(i);
    }

    // Method called when the node is added to the scene
    public override void _Ready()
    {
        LastPos = GlobalTransform.Origin;
        Mesh = new ImmediateMesh();
    }

    // Method called every frame
    public override void _Process(double delta)
    {
        // If the trail has moved more than the motion data and the trail is enabled, add a point
        if((LastPos - GlobalTransform.Origin).Length() > MotionData && TrailEnabled){
            AppendPoint();
            LastPos = GlobalTransform.Origin;
        }

        // Iterate over the points in the trail
        int p = 0;
        int max = Points.Count;
        while (p < max)
        {
            // Increase the life of the point
            Life[p] += (float)delta;
            // If the life of the point is greater than the lifespan, remove the point
            if (Life[p] > Lifespan)
            {
                RemovePoints(p);
                p--;
                if(p < 0){
                    p = 0;
                }
            }
            max = Points.Count;
            p++;
        }

        // Create a new mesh for the trail
        Mesh = new ImmediateMesh();
        if(Points.Count < 2){
            return;
        }

        // Begin creating the surface of the trail
        surfaceTool = new SurfaceTool(); 
        surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

        // Iterate over the points in the trail
        for (int i = 0; i < Points.Count; i++) {
            // Calculate the color of the point
            float t = (float)i / (Points.Count - 1);
            var color = StartColor.Lerp(EndColor, 1- t);
            surfaceTool.SetColor(color);

            // Calculate the width of the point
            var currWidth = Width[i][0] - Mathf.Pow(1-t, ScaleAcceleration) * Width[i][1];
            var t0 = i / (float)Points.Count;
            var t1 = t;
            surfaceTool.SetUV(new Vector2(t0, 0));
            surfaceTool.AddVertex(ToLocal(Points[i]) + currWidth);
            surfaceTool.SetUV(new Vector2(t1, 1));
            surfaceTool.AddVertex(ToLocal(Points[i]) - currWidth);
            surfaceTool.SetUV(new Vector2(t0, 1));
            surfaceTool.AddVertex(ToLocal(Points[i]) + currWidth);
        }

        // Commit the changes to the mesh
        Mesh = surfaceTool.Commit();
    }
}
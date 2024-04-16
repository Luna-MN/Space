using Godot;
using System;
using System.Collections.Generic;

public partial class Trail : MeshInstance3D
{
    public List<Vector3> Points { get; set; } = new List<Vector3>();
    public List<List<Vector3>> Width { get; set; } = new List<List<Vector3>>();
    public List<float> Life { get; set; } = new List<float>();
    public Vector3 LastPos { get; set; }
    public SurfaceTool surfaceTool;

    [Export]
    public bool TrailEnabled { get; set; } = true;

    [Export]
    public float FromWidth { get; set; } = 0.5f;

    [Export]
    public float ToWidth { get; set; } = 0.0f;

    private float _scaleAcceleration;
    [Export]
    public float ScaleAcceleration
    {
        get { return _scaleAcceleration; }
        set { _scaleAcceleration = Mathf.Clamp(value, 0.5f, 1.5f); }
    }

    [Export]
    public float MotionData { get; set; } = 0.1f;

    [Export]
    public float Lifespan { get; set; } = 1.0f;

    [Export]
    public Color StartColor { get; set; } = new Color(1, 1, 1, 1);

    [Export]
    public Color EndColor { get; set; } = new Color(1, 1, 1, 0);
    public void AppendPoint(){
        Points.Add(Position);
        Width.Add(new List<Vector3>(){GlobalTransform.Basis.X * FromWidth,
            GlobalTransform.Basis.X * FromWidth - GlobalTransform.Basis.X * ToWidth});

        Life.Add(0.0f);
    }
    public void RemovePoints(int i){
        Points.RemoveAt(i);
        Width.RemoveAt(i);
        Life.RemoveAt(i);
    }
    public override void _Ready()
    {
        LastPos = GlobalTransform.Origin;
        Mesh = new ImmediateMesh();
    }
    public override void _Process(double delta)
    {
        if((LastPos - GlobalTransform.Origin).Length() > MotionData && TrailEnabled){
            AppendPoint();
            LastPos = GlobalTransform.Origin;
        }
        int p = 0;
        int max = Points.Count;
        while (p < max)
        {
            Life[p] += (float)delta;
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
        Mesh = new ImmediateMesh();
         if(Points.Count < 2){
            return;
         }
         surfaceTool = new SurfaceTool(); 
         surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
         for (int i = 0; i < Points.Count; i++) {
            float t = (float)i / (Points.Count - 1);
            var color = StartColor.Lerp(EndColor, 1- t);
            surfaceTool.SetColor(color);

            var currWidth = Width[i][0] - Mathf.Pow(1-t, ScaleAcceleration) * Width[i][1];
            var t0 = i / (float)Points.Count;
            var t1 = t;
            surfaceTool.SetUV(new Vector2(t0, 0));
            surfaceTool.AddVertex(ToLocal(Points[i]) + currWidth);
            surfaceTool.SetUV(new Vector2(t1, 1));
            surfaceTool.AddVertex(ToLocal(Points[i]) - currWidth);
         }
         surfaceTool.Commit((ArrayMesh)this.Mesh);
    }
}

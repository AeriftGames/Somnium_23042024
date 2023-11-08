using Godot;
using System;

public partial class ItemSubViewportSetting : Resource
{
    [Export] public Vector3 cameraPos { get; set; } = new Vector3(0f,0f,6f);
    [Export] public float cameraFov { get; set; } = 60.0f;
    [Export] public Vector3 lightPos { get; set; } = new Vector3(0f,0f,5f);
    [Export] public float lightPower { get; set; } = 2f;
    [Export] public float lightSize { get; set; } = 0f;
    [Export] public float lightRange { get; set; } = 20f;
    [Export] public Color lightColor { get; set; } = Colors.White;
    [Export] public Vector3 meshPos { get; set; } = new Vector3(0f,0f, 0f);
    [Export] public Vector3 meshRot { get; set; }
    [Export] public Vector3 meshScale { get; set; } = new Vector3(1f,1f,1f);
}

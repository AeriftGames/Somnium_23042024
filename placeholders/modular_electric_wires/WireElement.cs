using Godot;
using Godot.Collections;
using System;
using System.ComponentModel.DataAnnotations;

[Tool]
public partial class WireElement : Node3D
{
    public enum EWireType { SHORT_BASE, MEDIUM_BASE, LONG_BASE, SMALL_ROUND, BIG_ROUND, SHORT_CORNER_IN, MEDIUM_CORNER_IN, SHORT_CORNER_OUT, MEDIUM_CORNER_OUT };
    public enum EWireNumber { ONE, THREE, SIX }

    EWireType wireType = EWireType.SHORT_BASE;
    bool useStartHolder = true;
    bool useEndHolder = true;
    int numberWires = 1;
    
	float offsetWireX = 0.0f;
    float offsetWireY = 0.0f;
	
    Array<MeshInstance3D> ArrayWires = null;
    
	[Export(PropertyHint.Range, "1,10,")] private int _numberWires
	{
		get { return numberWires; }
		set { numberWires = value; RecreateWireElement(); }
	}
	
    [Export]
    private EWireType _wireType
    {
        get { return wireType; }
        set { wireType = value; RecreateWireElement(); }
    }

    [Export]
    private bool _useStartHolder
    {
        get { return useStartHolder; }
        set { useStartHolder = value; if (startHolderMesh != null) { startHolderMesh.Visible = useStartHolder; } }
    }

    [Export]
    private bool _useEndHolder
    {
        get { return useEndHolder; }
        set { useEndHolder = value; if (endHolderMesh != null) { endHolderMesh.Visible = useEndHolder; } }
    }

    Mesh oneWireMesh = null;
    Mesh holderMesh = null;
    Node3D wires;
    MeshInstance3D wireMeshInstance;
    MeshInstance3D startHolderMesh;
    MeshInstance3D endHolderMesh;

    public override void _Ready()
    {
        wires = GetNode<Node3D>("Wires");
        startHolderMesh = GetNode<MeshInstance3D>("StartHolderMesh");
        endHolderMesh = GetNode<MeshInstance3D>("EndHolderMesh");

        startHolderMesh.Visible = _useStartHolder;
        endHolderMesh.Visible = _useEndHolder;

        numberWires = _numberWires;
        wireType = _wireType;

        RecreateWireElement();
    }

    public override void _Process(double delta)
    {

    }

    // Pri zmene parametru, vytvorime element odznova
    public void RecreateWireElement()
    {
        if (wires == null) return;
        //DESTROY ALL WIRES
        foreach (var w in wires.GetChildren())
            w.QueueFree();

        //LOAD SPECIFIC ONE WIRE MESH
        LoadMeshesByWireType(wireType);

        //FOR LOOP FOR EVERY WIRE
        for (int i = 0; i < numberWires; i++)
        {
            MeshInstance3D newWire = AddWire();
            newWire.Position = new Vector3(i * offsetWireX, i * offsetWireY, 0);
        }

        wires.Position = new Vector3(-(offsetWireX / 2 * numberWires) + (offsetWireX / 2), 0, 0);

        //ADD HOLDERS
        startHolderMesh.Mesh = holderMesh;
        endHolderMesh.Mesh = holderMesh;

        startHolderMesh.Visible = useStartHolder;
        endHolderMesh.Visible = useEndHolder;
    }

    public void LoadMeshesByWireType(EWireType newWireType)
    {
        //HOLDER
        if (numberWires < 4)
            holderMesh = GD.Load<Mesh>("res://placeholders/modular_electric_wires/small_holder_mesh.tres");
        else if (numberWires < 7)
            holderMesh = GD.Load<Mesh>("res://placeholders/modular_electric_wires/medium_holder_mesh.tres");
        else
            holderMesh = GD.Load<Mesh>("res://placeholders/modular_electric_wires/big_holder_mesh.tres");

        //WIRE
        switch (newWireType)
        {
            case EWireType.SHORT_BASE:
                {
                    oneWireMesh = GD.Load<Mesh>("res://placeholders/modular_electric_wires/one_wire_up_short_mesh.tres");
                    startHolderMesh.Position = new Vector3(0, -0.055f, 0.021f);
                    endHolderMesh.Rotation = new Vector3(0, 0, 0);
                    endHolderMesh.Position = new Vector3(0, 0.485f, 0.021f);
                    offsetWireX = 0.02f;
                    offsetWireY = 0.00f;
                    break;
                }
            case EWireType.MEDIUM_BASE:
                {
                    oneWireMesh = GD.Load<Mesh>("res://placeholders/modular_electric_wires/one_wire_up_medium_mesh.tres");
                    startHolderMesh.Position = new Vector3(0, -0.055f, 0.021f);
                    endHolderMesh.Rotation = new Vector3(0, 0, 0);
                    endHolderMesh.Position = new Vector3(0, 1.04f, 0.021f);
                    offsetWireX = 0.02f;
                    offsetWireY = 0.0f;
                    break;
                }
            case EWireType.LONG_BASE:
                {
                    oneWireMesh = GD.Load<Mesh>("res://placeholders/modular_electric_wires/one_wire_up_long_mesh.tres");
                    startHolderMesh.Position = new Vector3(0, -0.055f, 0.021f);
                    endHolderMesh.Rotation = new Vector3(0, 0, 0);
                    endHolderMesh.Position = new Vector3(0, 1.950f, 0.021f);
                    offsetWireX = 0.02f;
                    offsetWireY = 0.00f;
                    break;
                }
            case EWireType.SMALL_ROUND:
                {
                    if (numberWires == 1)
                        oneWireMesh = GD.Load<Mesh>("res://placeholders/modular_electric_wires/one_wire_smallround_mesh.tres");
                    else if (numberWires < 4)
                        oneWireMesh = GD.Load<Mesh>("res://placeholders/modular_electric_wires/three_wires_smallround_mesh.tres");

                    startHolderMesh.Position = new Vector3(0, 0, 0);
                    endHolderMesh.Rotation = new Vector3(0, 0, Mathf.DegToRad(-90));
                    endHolderMesh.Position = new Vector3(0.204f, 0.196f, 0.0f);
                    offsetWireX = 0.02f;
                    offsetWireY = 0f;
                    break;
                }
        }
    }

    MeshInstance3D AddWire()
    {
        MeshInstance3D newWire = new MeshInstance3D();
        newWire.Mesh = oneWireMesh;
        wires.AddChild(newWire);
        return newWire;
    }
}

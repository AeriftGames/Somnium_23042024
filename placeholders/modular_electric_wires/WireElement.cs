using Godot;
using Godot.Collections;
using System;
using System.ComponentModel.DataAnnotations;

[Tool]
public partial class WireElement : Node3D
{
	public enum EWireType { SHORT_BASE, MEDIUM_BASE, LONG_BASE, SMALL_ROUND, BIG_ROUND, SHORT_CORNER_IN, MEDIUM_CORNER_IN, SHORT_CORNER_OUT, MEDIUM_CORNER_OUT };

	EWireType wireType = EWireType.SHORT_BASE;
	bool useStartHolder = true;
	bool useEndHolder = true;
	int numberWires = 1;

	Array<MeshInstance3D> ArrayWires = null;

	[Export(PropertyHint.Range, "1,10,")] private int _numberWires
	{
		get { return numberWires; }
		set { numberWires = value; RecreateWireElement(); }
	}

	[Export] private EWireType _wireType
	{
		get { return wireType; }
		set { wireType = value; RecreateWireElement(); }
	}

    [Export] private bool _useStartHolder
	{
		get { return useStartHolder; }
		set { useStartHolder = value; if (startHolderMesh != null) { startHolderMesh.Visible = useStartHolder; } }
	}

    [Export] private bool _useEndHolder
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
		foreach(var w in wires.GetChildren())
			w.QueueFree();

        //LOAD SPECIFIC ONE WIRE MESH
        LoadMeshesByWireType(wireType);
		float offsetWire = 0.02f;

		//FOR LOOP FOR EVERY WIRE
		for (int i = 0; i < numberWires; i++)
		{
            MeshInstance3D newWire = AddWire();
			newWire.Position = new Vector3(i*offsetWire,0,0);
        }

		wires.Position = new Vector3(-(offsetWire/2 *numberWires)+(offsetWire/2),0,0);

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
                    endHolderMesh.Position = new Vector3(0, 0.485f, 0);
                    break;
				}
            case EWireType.MEDIUM_BASE:
                {
                    oneWireMesh = GD.Load<Mesh>("res://placeholders/modular_electric_wires/one_wire_up_short_mesh.tres");
					endHolderMesh.Position = new Vector3(0,1.04f,0);
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

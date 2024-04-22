using Godot;
using System;

public partial class elevator_counter : Node3D
{
    [Export] public NodePath pickElevator;

    private Label3D label3D;
    private elevator_2_functional elevator;
    public override void _Ready()
    {
        base._Ready();

        label3D = GetNode<Label3D>("CounterBase/Label3D");

        if (pickElevator.IsEmpty == false)
            elevator = GetNode<elevator_2_functional>(pickElevator);
    }

    public void SetLevel( int newLevel ) { label3D.Text = newLevel.ToString(); }
}

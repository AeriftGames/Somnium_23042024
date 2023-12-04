using Godot;
using System;

public partial class Reticle : CenterContainer
{
	[Export] public float DOT_RADIUS = 1.0f;
	[Export] public Color DOT_COLOR = Colors.White;
	[Export] public Godot.Collections.Array<Line2D> RETICLE_LINES;
	[Export] public float RETICLE_SPEED = 0.25f;
	[Export] public float RETICLE_DISTANCE = 2.0f;

    public override void _Ready()
	{
		QueueRedraw();
	}

	public override void _Process(double delta)
	{
		AdjustReticleLines();
	}

    public override void _Draw()
    {
        base._Draw();
		DrawCircle(new Vector2(0.0f,0.0f),DOT_RADIUS,DOT_COLOR);
    }

	public void AdjustReticleLines()
	{
		Vector3 realVel = Vector3.Zero;
        if (CGameMaster.GM.GetGame().GetFPSCharacterBase() != null)
		{ realVel = CGameMaster.GM.GetGame().GetFPSCharacterBase().GetRealVelocity(); }

		Vector3 origin = new Vector3(0,0,0);
		Vector2 pos = new Vector2(0,0);
		float speed = origin.DistanceTo(realVel);

        RETICLE_LINES[0].Position = 
			RETICLE_LINES[0].Position.Lerp(pos + new Vector2(0, -speed * RETICLE_DISTANCE), RETICLE_SPEED);

        RETICLE_LINES[1].Position =
            RETICLE_LINES[1].Position.Lerp(pos + new Vector2(speed * RETICLE_DISTANCE, 0), RETICLE_SPEED);

        RETICLE_LINES[2].Position =
            RETICLE_LINES[2].Position.Lerp(pos + new Vector2(0, speed * RETICLE_DISTANCE), RETICLE_SPEED);

        RETICLE_LINES[3].Position =
            RETICLE_LINES[3].Position.Lerp(pos + new Vector2(-speed * RETICLE_DISTANCE, 0), RETICLE_SPEED);
    }
}

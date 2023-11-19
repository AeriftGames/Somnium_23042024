using Godot;
using System;
using System.Threading.Tasks;

public partial class ladder_system : Node3D
{
    public enum ELadderStartPosCharacter { Down, Top }
    public enum ELadderCharacterEffectProcess { Teleport,TeleportWithBlackScreen }

    private bool isLadderVisibleFromDown = false;
    private bool isLadderVisibleFromTop = false;

    private bool isCharacterInAreaTop = false;
    private bool isCharacterInAreaDown = false;
    private bool isCharacter = false;
    private bool isCharacterCanUseLadder = false;

    private bool GetIsCharacter(Node3D body)
    {
        bool result = false;

        if (body != null)
        {
            FPSCharacter_Inventory invChar = body as FPSCharacter_Inventory;
            if (invChar != null)
            {
                result = true;
            }
        }

        return result;
    }

    private FPSCharacter_Inventory GetOurCharacter()
    {
        return GameMaster.GM.GetFPSCharacter() as FPSCharacter_Inventory;
    }

    public bool GetIsCharacterCanUseLadder() { return isCharacterCanUseLadder; }

    public void _on_area_ladder_down_body_entered(Node3D body)
    {
        if (!GetIsCharacter(body)) return;

        isCharacterInAreaDown = true;
        UpdateCharacterArea();
    }

    public void _on_area_ladder_down_body_exited(Node3D body)
    {
        if (!GetIsCharacter(body)) return;

        isCharacterInAreaDown = false;
        UpdateCharacterArea();
    }

    public void _on_area_ladder_up_body_entered(Node3D body)
    {
        if (!GetIsCharacter(body)) return;

        isCharacterInAreaTop = true;
        UpdateCharacterArea();
    }

    public void _on_area_ladder_up_body_exited(Node3D body)
    {
        if (!GetIsCharacter(body)) return;

        isCharacterInAreaTop = false;
        UpdateCharacterArea();
    }

    public void _on_visible_on_screen_notifier_3d_screen_entered()
    {
        isLadderVisibleFromDown = true;
        UpdateCharacterArea();
    }

    public void _on_visible_on_screen_notifier_3d_screen_exited()
    {
        isLadderVisibleFromDown = false;
        UpdateCharacterArea();
    }

    public void _on_visible_on_screen_notifier_from_up_screen_entered()
    {
        isLadderVisibleFromTop = true;
        UpdateCharacterArea();
    }

    public void _on_visible_on_screen_notifier_from_up_screen_exited()
    {
        isLadderVisibleFromTop = false;
        UpdateCharacterArea();
    }

    public void UpdateCharacterArea()
    {
        if (isCharacterInAreaDown && isLadderVisibleFromDown)
        {
            // jsme dole u zebriku a koukame smerem na nej
            GD.Print("jsme dole u zebriku a muzeme ho pouzit");
            isCharacterCanUseLadder = true;

            if (GetOurCharacter().GetCharacterUseLadderComponent() != null)
                GetOurCharacter().GetCharacterUseLadderComponent().SetCanUseLadder(true, this);
        }
        else if (isCharacterInAreaTop && isLadderVisibleFromTop)
        {
            // jsme nahore u zebriku a koukame smerem na nej
            GD.Print("jsme nahore u zebriku a muzeme ho pouzit");
            isCharacterCanUseLadder = true;

            if (GetOurCharacter().GetCharacterUseLadderComponent() != null)
                GetOurCharacter().GetCharacterUseLadderComponent().SetCanUseLadder(true, this);
        }
        else
        {
            GD.Print("nejsme u zebriku");
            isCharacterCanUseLadder = false;

            if (GetOurCharacter().GetCharacterUseLadderComponent() != null)
                GetOurCharacter().GetCharacterUseLadderComponent().SetCanUseLadder(false, null);
        }
    }

    public Vector3 GetCharacterStartPos(ELadderStartPosCharacter newLadderStartPos)
    {
        Vector3 result = Vector3.Zero;
        switch (newLadderStartPos)
        {
            case ELadderStartPosCharacter.Down:
                GD.Print("Down");
                result = GetNode<Node3D>("StartPos/Down").GlobalPosition; break;
            case ELadderStartPosCharacter.Top:
                GD.Print("Top");
                result = GetNode<Node3D>("StartPos/Top").GlobalPosition; break;
        }

        return result;
    }

    public void UseLadder(ELadderCharacterEffectProcess newLadderCharacterEffectProcess)
    {
        switch (newLadderCharacterEffectProcess)
        {
            case ELadderCharacterEffectProcess.Teleport: UseLadder_EffectTeleport(); break;
            case ELadderCharacterEffectProcess.TeleportWithBlackScreen: UseLadder_EffectTeleportBlackScreen(); break;
        }
    }

    private async void UseLadder_EffectTeleport()
    {
        if (isCharacterInAreaTop)
        {
            float defLerpSpeed = GetOurCharacter().LerpSpeedPosObjectCamera;
            GetOurCharacter().LerpSpeedPosObjectCamera = 100.0f;
            GetOurCharacter().GlobalPosition = GetCharacterStartPos(ELadderStartPosCharacter.Down);
            await Task.Delay(50);
            GetOurCharacter().GetObjectCamera().SetInstantLookingAt(
                GetNode<Node3D>("StartPos/DownLook").GlobalPosition, false, true, true, true);
            await Task.Delay(20);
            GetOurCharacter().LerpSpeedPosObjectCamera = defLerpSpeed;
        }
        else if (isCharacterInAreaDown)
        {
            float defLerpSpeed = GetOurCharacter().LerpSpeedPosObjectCamera;
            GetOurCharacter().LerpSpeedPosObjectCamera = 100.0f;
            GetOurCharacter().GlobalPosition = GetCharacterStartPos(ELadderStartPosCharacter.Top);
            await Task.Delay(50);
            GetOurCharacter().GetObjectCamera().SetInstantLookingAt(
                GetNode<Node3D>("StartPos/TopLook").GlobalPosition, false, true, true, true);
            await Task.Delay(20);
            GetOurCharacter().LerpSpeedPosObjectCamera = defLerpSpeed;
        }
    }

    private async void UseLadder_EffectTeleportBlackScreen()
    {
        GameMaster.GM.EnableBlackScreen(true);
        GetOurCharacter().SetInputEnable(false);
        await Task.Delay(500);

        UseLadder_EffectTeleport();

        GameMaster.GM.EnableBlackScreen(false);
        await Task.Delay(200);
        GetOurCharacter().SetInputEnable(true);
    }
}
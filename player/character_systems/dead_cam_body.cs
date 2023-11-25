using Godot;
using System;

public partial class dead_cam_body : RigidBody3D
{
    AudioStreamPlayer audioPlayer = null;
    AnimationPlayer animationPlayer = null;

    public bool isActivate = false;
    public float lerpSpeed = 100.0f;

    private bool isOnceLand = false;

    ShakeLerp deadCamShakeLerp = null;

    public override void _Ready()
    {
        base._Ready();

        audioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        // lerping camera
        if(isActivate)
        {
            // lerp pos of camera
            CGameMaster.GM.GetGame().GetFPSCharacter().GetFPSCharacterCamera().GlobalPosition = 
                CGameMaster.GM.GetGame().GetFPSCharacter().GetFPSCharacterCamera().
                GlobalPosition.Lerp(GlobalPosition, lerpSpeed * (float)delta);
        }

        // deadCamShake update
        if(deadCamShakeLerp != null)
            deadCamShakeLerp.Update(delta);
    }

    public void ActivateDeadCam()
    {
        Camera3D characterCamera = CGameMaster.GM.GetGame().GetFPSCharacter().GetFPSCharacterCamera();
        Vector3 oldGlobalPosition = characterCamera.GlobalPosition;
        Vector3 oldDirectionPoint = CGameMaster.GM.GetGame().GetFPSCharacter().GlobalPosition + 
            (characterCamera.GlobalTransform.Basis.Z * -100.0f);

        // nastavi aktualni pozici na misto kde ma/mela byt kamera
        GlobalPosition = oldGlobalPosition;

        // vypne lerping kamery k characteru
        CGameMaster.GM.GetGame().GetFPSCharacter().GetObjectCamera().SetLerpToCharacterEnable(false);

        // zrusi stary child kamery s hracem
        characterCamera.GetParent().RemoveChild(characterCamera);

        // nastavi nas dead_cam_body na puvodni pozici kamery a attachne ma sebe kameru
        LookAtFromPosition(GlobalPosition,oldDirectionPoint);
        AddChild(characterCamera);

        // Physics Process logika zapnuta
        isActivate = true;
        
        // deadCamShake init
        deadCamShakeLerp = new ShakeLerp();
        deadCamShakeLerp.Init(characterCamera);
    }

    public void _on_body_entered(Node body)
    {
        if (isOnceLand) return;
        if (body as FPSCharacter_Interaction != null) return;

        FPSCharacter_Inventory char_inv = CGameMaster.GM.GetGame().GetFPSCharacter() as FPSCharacter_Inventory;
        UniversalFunctions.PlayRandomSound(audioPlayer,char_inv.GetBodyFallAudios(),0,1);
        
        animationPlayer.Play("death");
        char_inv.GetCharacterHealthComponent().GetDamageHud().StartBloodDeathHud();

        //deadCamShake start shake
        if(deadCamShakeLerp != null)
            deadCamShakeLerp.StartBasicShake(0.7f, 0.15f, 5.0f, 1f);

        isOnceLand = true;
    }

    public void OpenInGameMenu()
    {
        CGameMaster.GM.GetGame().GetInGameMenu().SetActive(true);
    }

    public void FreeAll()
    {
        deadCamShakeLerp.QueueFree();
        deadCamShakeLerp.FreeAll(); //stop and freeing
        QueueFree();
    }
}

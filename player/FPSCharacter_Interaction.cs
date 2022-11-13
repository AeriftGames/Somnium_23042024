using Godot;
using System;

/*
 * *** FPSCharacter_Interaction(0.0) ***
 * 
 * - this class is inheret from FPSCharacter_WalkingEffects and handle interaction with world
 * - TODO 1: pri aktivaci interaction_object_look se musi hrac napred zastavit a byt na zemi, 
 *   teprve pote spustit lerp
 *   (je to z duvodu nacterni rotace kamery pro plynule opetovne nastaveni do puvodniho stavu)
 * - TODO 2: zlepsit vraceni do puvodniho stavu tim ze budeme lerpovat mezi
 *   temp_hitPosition a targetLook(interact_object_look)
*/
public partial class FPSCharacter_Interaction : FPSCharacter_WalkingEffects
{
	CharacterInteractiveSystem InteractiveSystem = null;

	BasicHud basicHud = null;

	[Export] public float LengthInteractRay = 5.0f;

	Vector3 tempCamRot = Vector3.Zero;
	Vector3 tempTargetLook = Vector3.Zero;

	Vector3 tempDirectionTo = Vector3.Zero;
	Vector3 tempDistanceTo = Vector3.Zero;
	Vector3 tempHitPosition = Vector3.Zero;

	// LERPOBJECT INTERACT
	LerpObject.LerpVector3 LerpCameraPosToInteract = new LerpObject.LerpVector3();
	LerpObject.LerpVector3 LerpCameraLookToInteract = new LerpObject.LerpVector3();
	bool isActualOnLerpToNormal = false;

	// HANDS
	public Node3D HolderHands = null;
	public ObjectHands objectHands = null;

	// Simple Flashlight toggle test
	bool isFlashlightEnable = false;
	AudioStreamPlayer AudioStreamPlayer_TestItem = null;
    [ExportGroupAttribute("Simple Flashlight Settings")]
    [Export] public AudioStream AudioFlashlight_On;
    [Export] public AudioStream AudioFlashlight_Off;

	public Vector2 _MouseMotionForRotation = new Vector2(0,0);

    public override void _Ready()
	{
		base._Ready();

		// nacteni hudu
        basicHud = GetNode<BasicHud>("BasicHud");
        basicHud.SetUseVisible(false);

        // vytvoreni grab system
        InteractiveSystem = new CharacterInteractiveSystem(this,basicHud);

		// hands
		HolderHands = GetNode<Node3D>("HeadMain/HeadGimbalA/HeadGimbalB/HeadHolderCamera/HolderHands");

		// Simple Flashlight toggle test
		AudioStreamPlayer_TestItem = GetNode<AudioStreamPlayer>("AudioStreamPlayer_TestItem");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		// Toggle Simple Flashlight
		if(Input.IsActionJustPressed("ToggleFlashlight"))
			ToggleSimpleFlashlight();

		// UPDATE HANDS
		objectHands.GlobalPosition = 
			objectHands.GlobalPosition.Lerp(objectCamera.Camera.GlobalPosition, 40 * (float)delta);
		
		Vector3 hands_rot = objectHands.GlobalRotation;
		float hands_rotY = hands_rot.y;
		float hands_rotX = hands_rot.x;
		hands_rotY = Mathf.LerpAngle(hands_rotY,objectCamera.Camera.GlobalRotation.y, 30* (float)delta);
		hands_rotX = Mathf.LerpAngle(hands_rotX, objectCamera.Camera.GlobalRotation.x, 30 * (float)delta);
		hands_rot.y = hands_rotY;
		hands_rot.x = hands_rotX;
		objectHands.GlobalRotation = hands_rot;
		

		// UPDATE LERPOBJECT INTERACT
		if (LerpCameraPosToInteract.IsEnableUpdate())
			GetFPSCharacterCamera().GlobalPosition = LerpCameraPosToInteract.Update(delta);

		if (LerpCameraLookToInteract.IsEnableUpdate())
			GetFPSCharacterCamera().LookAtFromPosition(GetFPSCharacterCamera().GlobalPosition,
				LerpCameraLookToInteract.GetTarget());

		// kamera je na ceste zpet k normalu
		if(isActualOnLerpToNormal)
		{
			// jsme jiz tesne v cili ?
			if(LerpCameraPosToInteract.GetLengthToTarget() < 0.01f)
			{
				// vyresetujeme parametry, povolime input a prerusime update lerpu
				GetFPSCharacterCamera().Position = new Vector3(0.0f, 0.0f, 0.0f);
				GetFPSCharacterCamera().Rotation = tempCamRot;

				SetInputEnable(true);
				LerpCameraPosToInteract.EnableUpdate(false);
				LerpCameraLookToInteract.EnableUpdate(false);
				isActualOnLerpToNormal = false;
			}
		}

	}

    public override void _Input(InputEvent @event)
    {
		base._Input(@event);

		// RotateObject Update
		InteractiveSystem.UpdateGrabbedObjectRotate(@event);
    }

    public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		bool useNow = IsInputEnable() && Input.IsActionJustPressed("UseAction");
		bool grabNow = IsInputEnable() && Input.IsActionPressed("mouseClickLeft");
		bool throwObjectNow = IsInputEnable() && Input.IsActionJustPressed("throwObject");
		bool rotateGrabbedObject = IsInputEnable() && Input.IsActionPressed("rotateGrabbedObject");

		if(grabNow == false)
			DetectInteractiveObjectWithCameraRay();

		InteractiveSystem.Update(useNow,grabNow,delta);
		InteractiveSystem.HandGrabbingUpdate(grabNow,throwObjectNow,rotateGrabbedObject,delta);
	}

	public bool DetectInteractiveObjectWithCameraRay()
	{
		bool result = false;
        interactive_object interact_object = null;

		if (GetFPSCharacterCamera() == null) return false;

		PhysicsDirectSpaceState3D directSpace = GetWorld3d().DirectSpaceState;
		if (directSpace == null) return false;

		PhysicsRayQueryParameters3D rayParam = new PhysicsRayQueryParameters3D();
		rayParam.From = GetFPSCharacterCamera().GlobalPosition;
		rayParam.To = GetFPSCharacterCamera().GlobalPosition - 
			GetFPSCharacterCamera().GlobalTransform.basis.z * LengthInteractRay;

		var rayResult = directSpace.IntersectRay(rayParam);
		if (rayResult.Count > 0)
		{
			Node HitCollider = (Node)rayResult["collider"];
			if (HitCollider == null) return false;
			if (HitCollider.GetParent() == null) return false;

			if(HitCollider.GetParent().IsInGroup("interactive_object"))
			{
                interact_object = (interactive_object)HitCollider.GetParent();
				tempHitPosition = (Vector3)rayResult["position"];
            }
		}

		// Final
		InteractiveSystem.SetActualInteractiveObject(interact_object,tempHitPosition);
		return result;
	}

	public void DisableInputsAndCameraMoveLookTarget(Vector3 targetPos,Vector3 targetLook)
	{
		// LERPOBJECT START INTERACT
		SetInputEnable(false);
		tempCamRot = GetFPSCharacterCamera().Rotation;
		//tempTargetLook = targetLook;
		LerpCameraPosToInteract.SetAllParam(GetFPSCharacterCamera().GlobalPosition,
			targetPos, 10f);
		LerpCameraPosToInteract.EnableUpdate(true);

		LerpCameraLookToInteract.SetAllParam(GetFPSCharacterCamera().Transform.basis.z*0.1f,
			targetLook,
			1.0f);
		LerpCameraLookToInteract.EnableUpdate(true);


    }

	public void EnableInputsAndCameraToNormal()
	{
		// LERPOBJECT END INTERACT
		// !!! tip na mozne zlepseni: lerpovat mezi tempHitPosition a targetLook od interactive_objectu !!!
		isActualOnLerpToNormal = true;
		LerpCameraPosToInteract.SetTarget(HeadHolderCamera.GlobalPosition);
		LerpCameraLookToInteract.SetTarget(tempHitPosition);
		
		// pak, az budeme chtit staci vyresetovat lokalni pozice kamery a rotaci kamery na puvodni
		// zde resime v Process
	}

	// Prozatimni reseni flashlight
	public void ToggleSimpleFlashlight()
	{
		// Prepnout stav
		isFlashlightEnable = !isFlashlightEnable;

		if(isFlashlightEnable)
		{
			//ON
			GameMaster.GM.Log.WriteLog(this,LogSystem.ELogMsgType.INFO,"Flaslight ON");
			objectHands.objectFlashlight.Visible = true;

			// Audio play
			AudioStreamPlayer_TestItem.VolumeDb = -15f;
			AudioStreamPlayer_TestItem.PitchScale = 0.9f;
            AudioStreamPlayer_TestItem.Stream = AudioFlashlight_On;
            AudioStreamPlayer_TestItem.Play();
        }
		else
		{
            //OFF
            GameMaster.GM.Log.WriteLog(this, LogSystem.ELogMsgType.INFO, "Flaslight OFF");
            objectHands.objectFlashlight.Visible = false;

            // Audio play
            AudioStreamPlayer_TestItem.VolumeDb = -15f;
            AudioStreamPlayer_TestItem.PitchScale = 0.9f;
            AudioStreamPlayer_TestItem.Stream = AudioFlashlight_Off;
            AudioStreamPlayer_TestItem.Play();
        }
	}

	public CharacterInteractiveSystem GetInteractiveSystem()
	{
		return InteractiveSystem;
	}
}

using Godot;
using System;

/*
 * *** FPSCharacter_Interaction(0.0) ***
 * 
 * - this class is inheret from FPSCharacter_WalkingEffects and handle interaction with world
*/
public partial class FPSCharacter_Interaction : FPSCharacter_WalkingEffects
{
	CharacterInteractiveSystem InteractiveSystem = null;

	[ExportGroupAttribute("Camera Zoom")]
	[Export] public float CameraFovNormal = 65.0f;
	[Export] public float CameraFovZoomed = 40.0f;
	[Export] public float CameraFovInterpSpeed = 4.0f;
	[Export] public Node3D CameraZoomToObject = null;
	[Export] public float CameraZoomToObjectZoomed = 35.0f;
	[Export] public float CameraZoomToObjectInterpSpeed = 4.0f;
	[Export] public float CameraZoomToObjectInterpRotationSpeed = 7.0f;
	[Export] public bool CameraZoomToObjectSetPlayerDirection = false;
	Node3D testZoomObject = null;

	[ExportGroupAttribute("InteractiveSystem: Base Settings")]
	[Export] public bool CanUse = true;
	[Export] public bool CanGrabObject = true;
	[Export] public bool CanThrowObject = true;
	[Export] public bool CanRotateObject = true;
	[Export] public bool CanMoveFarOrNearObject = true;
	[Export] public bool MustBeInInteractiveArea = true;
	[Export] public float LengthInteractRay = 5.0f;
	[Export] public float GrabObjectPullPower = 4.0f;
	[Export] public float ThrowObjectPower = 6.0f;
	[Export] public float MoveFarOrNearObjectStep = 0.1f;
	[Export] public float MoveFarOrNearObjectOriginal = 1.5f;
	[Export] public float MoveFarOrNearObjectMin = 1.0f;
	[Export] public float MoveFarOrNearObjectMax = 2.0f;
	[Export] public float RotateObjectStep = 0.3f;

	[ExportGroupAttribute("InteractiveSystem: Rigidbody PhysicParams In Grab")]
	[Export] public Vector3 RBPhysicInGrab_Inertia = new Vector3(0.5f,0.5f,0.5f);
	[Export] public float RBPhysicInGrab_AngularDamp = 3.0f;
	[Export] public float RBPhysicInGrab_LinearDamp = 1.0f;
	[Export] public float RBPhysicInGrab_Friction = 0.15f;
	[Export] public float RBPhysicInGrab_Bounce = 0.0f;
	[Export] public float RBPhysicInGrab_Mass = 1.0f;

	BasicHud basicHud = null;

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
    [Export] public float AudioFlashlight_On_Pitch = 1.0f;
    [Export] public float AudioFlashlight_On_VolumeDb = -10.0f;
    [Export] public AudioStream AudioFlashlight_Off;
    [Export] public float AudioFlashlight_Off_Pitch = 0.8f;
    [Export] public float AudioFlashlight_Off_VolumeDb = -10.0f;

    public override void _Ready()
	{
		base._Ready();

		// nacteni hudu
		basicHud = GetNode<BasicHud>("BasicHud");
		basicHud.SetUseVisible(false);

		// vytvoreni grab system
		InteractiveSystem = new CharacterInteractiveSystem(this,basicHud);

		// hands
		HolderHands = GetNode<Node3D>("%HolderHands");

		// Simple Flashlight toggle test
		AudioStreamPlayer_TestItem = GetNode<AudioStreamPlayer>("AudioStreamPlayer_TestItem");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		// RotateObject Update
		InteractiveSystem.UpdateGrabbedItemRotate(@event);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (GameMaster.GM.GetIsQuitting()) return;
        if (objectCamera == null) return;

        base._PhysicsProcess(delta);

		bool useNow = IsInputEnable() && CanUse && Input.IsActionJustPressed("UseAction");
		bool grabNow = IsInputEnable() && CanGrabObject && Input.IsActionPressed("mouseClickLeft");
		bool throwObjectNow = IsInputEnable() && CanThrowObject && Input.IsActionJustPressed("throwObject");
		bool rotateGrabbedObject = IsInputEnable() && CanRotateObject && Input.IsActionPressed("rotateGrabbedObject");
		bool moveFarGrabbedObject = IsInputEnable() && CanMoveFarOrNearObject && Input.IsActionJustReleased("moveFarGrabbedObject");
		bool moveNearGrabbedObject = IsInputEnable() && CanMoveFarOrNearObject && Input.IsActionJustReleased("moveNearGrabbedObject");

		if (grabNow == false)
			DetectInteractiveObjectWithCameraRay();

		InteractiveSystem.BasicUpdate(useNow,grabNow,delta);
		InteractiveSystem.InteractivePhysicsUpdate(grabNow,throwObjectNow,rotateGrabbedObject,
			moveFarGrabbedObject,moveNearGrabbedObject,delta);

		// ---------------------------------------------------------------------------------
		// Toggle Simple Flashlight
		if (IsInputEnable() && Input.IsActionJustPressed("ToggleFlashlight"))
			ToggleSimpleFlashlight();

		// Camera Zoom
		if (IsInputEnable() && Input.IsActionPressed("CameraZoom"))
			SetCameraZoom(true);
		else if(Input.IsActionJustReleased("CameraZoom"))
			SetCameraZoom(false);

		// Camera Zoom To Object
		if (Input.IsActionPressed("CameraZoomToObject"))
		{
			testZoomObject = GetNodeOrNull<Node3D>("/root/worldlevel/barrel1");

			if(testZoomObject != null)
				SetCameraZoomToObject(true, testZoomObject);
		}
		else
		{
			testZoomObject = GetNodeOrNull<Node3D>("/root/worldlevel/barrel1");

			if (testZoomObject != null)
				SetCameraZoomToObject(false, testZoomObject);
		}

		// Camera Zoom to Object a i tam zmenime direction hrace
		if(Input.IsActionJustPressed("CameraZoomToObjectAndSetDirection"))
		{
			testZoomObject = GetNodeOrNull<Node3D>("/root/worldlevel/barrel1");

			if (testZoomObject != null)
				SetCameraZoomToObject(true, testZoomObject,true,30,4,6,true);
		}

		// UPDATE HANDS
		objectHands.GlobalPosition =
			objectHands.GlobalPosition.Lerp(objectCamera.Camera.GlobalPosition, 40 * (float)delta);

		Vector3 hands_rot = objectHands.GlobalRotation;
		float hands_rotY = hands_rot.Y;
		float hands_rotX = hands_rot.X;
		hands_rotY = Mathf.LerpAngle(hands_rotY, objectCamera.Camera.GlobalRotation.Y, 30 * (float)delta);
		hands_rotX = Mathf.LerpAngle(hands_rotX, objectCamera.Camera.GlobalRotation.X, 30 * (float)delta);
		hands_rot.Y = hands_rotY;
		hands_rot.X = hands_rotX;
		objectHands.GlobalRotation = hands_rot;


		// UPDATE LERPOBJECT INTERACT
		if (LerpCameraPosToInteract.IsEnableUpdate())
			GetFPSCharacterCamera().GlobalPosition = LerpCameraPosToInteract.Update(delta);

		if (LerpCameraLookToInteract.IsEnableUpdate())
			GetFPSCharacterCamera().LookAtFromPosition(GetFPSCharacterCamera().GlobalPosition,
				LerpCameraLookToInteract.GetTarget());

		// kamera je na ceste zpet k normalu
		if (isActualOnLerpToNormal)
		{
			// jsme jiz tesne v cili ?
			if (LerpCameraPosToInteract.GetLengthToTarget() < 0.01f)
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

	public bool DetectInteractiveObjectWithCameraRay()
	{
		bool result = false;
		interactive_object interact_object = null;

		if (GetFPSCharacterCamera() == null) return false;

		PhysicsDirectSpaceState3D directSpace = GetWorld3D().DirectSpaceState;
		if (directSpace == null) return false;

		PhysicsRayQueryParameters3D rayParam = new PhysicsRayQueryParameters3D();
		rayParam.From = GetFPSCharacterCamera().GlobalPosition;
		rayParam.To = GetFPSCharacterCamera().GlobalPosition - 
			GetFPSCharacterCamera().GlobalTransform.Basis.Z * LengthInteractRay;

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

		LerpCameraLookToInteract.SetAllParam(GetFPSCharacterCamera().Transform.Basis.Z*0.1f,
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
			AudioStreamPlayer_TestItem.VolumeDb = AudioFlashlight_On_VolumeDb;
			AudioStreamPlayer_TestItem.PitchScale = AudioFlashlight_On_Pitch;
			AudioStreamPlayer_TestItem.Stream = AudioFlashlight_On;
			AudioStreamPlayer_TestItem.Play();
		}
		else
		{
			//OFF
			GameMaster.GM.Log.WriteLog(this, LogSystem.ELogMsgType.INFO, "Flaslight OFF");
			objectHands.objectFlashlight.Visible = false;

			// Audio play
			AudioStreamPlayer_TestItem.VolumeDb = AudioFlashlight_Off_VolumeDb;
			AudioStreamPlayer_TestItem.PitchScale = AudioFlashlight_Off_Pitch;
			AudioStreamPlayer_TestItem.Stream = AudioFlashlight_Off;
			AudioStreamPlayer_TestItem.Play();
		}
	}

	public void SetCameraZoom(bool newZoom, float newZoomValue = -1.0f, float newZoomInterpSpeed = -1.0f)
	{
		objectCamera.SetZoom(newZoom, newZoomValue, newZoomInterpSpeed);
	}

	public void SetCameraZoomToObject(bool newZoom, Node3D newZoomedObject,bool newSetPlayerDirection = false,
		float newZoomValue = -1.0f, float newZoomInterpSpeed = -1.0f, float newZoomRotateSpeed = -1.0f,
		bool newOnHitTargetZoomToNormal = false)
	{
		objectCamera.SetZoomToObject(newZoom,newZoomedObject,newSetPlayerDirection,
			newZoomValue,newZoomInterpSpeed,newZoomRotateSpeed,newOnHitTargetZoomToNormal);
	}

	public CharacterInteractiveSystem GetInteractiveSystem()
	{
		return InteractiveSystem;
	}

	public BasicHud GetBasicHud()
	{
		return basicHud;
	}

	public InGameMenu GetInGameMenu()
	{
		return GetAllHudsControlNode().GetNode<InGameMenu>("InGameMenu");
	}

	public override void FreeAll()
	{
		base.FreeAll();
		objectHands.QueueFree();
		QueueFree();
	}
}

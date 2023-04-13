using Godot;
using System;
using System.Linq;
using static Godot.TextServer;
using static UniversalFunctions;

public partial class ObjectCamera : Node3D
{
	public Node3D GimbalLand = null;
	public Node3D NodeRotY = null;
	public Node3D NodeRotX = null;
	public Node3D NodeLean = null;
	public Camera3D Camera = null;
	public Marker3D HandGrabMarker;
	public Generic6DofJoint3D HandGrabJoint = null;
	public StaticBody3D HandStaticBody = null;

	private Vector2 _MouseMotion = new Vector2(0f, 0f);
	private Vector2 _LookVelocity = new Vector2(0f, 0f);

	FPSCharacter_BasicMoving ownerCharacter = null;

	private LerpObject.LerpVector3 LerpObject_ObjectCameraPos = new LerpObject.LerpVector3();

	// lean
	Node3D LerpPos_LeanCenter = null;
	Node3D LerpPos_LeanLeft = null;
	Node3D LerpPos_LeanRight = null;

	public enum ELeanType { Center, Left, Right };
	private ELeanType ActualLean = ELeanType.Center;

	Tween tweenLeanRot = null;
	Tween tweenLeanPos = null;

	private bool isCameraLookInputEnable = true;

	bool isStopped = false;

	// Zoom
	private float neededZoomValue;
	private LerpObject.LerpFloat LerpObject_CameraZoom = new LerpObject.LerpFloat();
	private LerpObject.LerpVector3 LerpObject_CameraZoomToObject = new LerpObject.LerpVector3();
	private Vector3 workingZoomPosVector = Vector3.Zero;
	private Node3D lookingPoint = null;
	private bool isRunningZoom = false;
	private bool isZoomRotSetDirection = false;
	private bool isOnHitTargetZoomToNormal = false;

	public override void _Ready()
	{
		NodeRotY = GetNode<Node3D>("NodeRotY");
		GimbalLand = GetNode<Node3D>("NodeRotY/GimbalLand");
		NodeRotX = GetNode<Node3D>("NodeRotY/GimbalLand/NodeRotX");
		NodeLean = GetNode<Node3D>("NodeRotY/GimbalLand/NodeRotX/NodeLean");
		Camera = GetNode<Camera3D>("NodeRotY/GimbalLand/NodeRotX/NodeLean/Camera");
		HandGrabMarker = GetNode<Marker3D>("NodeRotY/GimbalLand/NodeRotX/NodeLean/Camera/HandGrabMarker");
		HandGrabJoint = GetNode<Generic6DofJoint3D>("NodeRotY/GimbalLand/NodeRotX/NodeLean/Camera/HandGrabJoint");
		HandStaticBody = GetNode<StaticBody3D>("NodeRotY/GimbalLand/NodeRotX/" +
			"NodeLean/Camera/HandGrabMarker/HandStaticBody");

		//lean
		LerpPos_LeanCenter = GetNode<Node3D>("NodeRotY/GimbalLand/NodeRotX/LerpPos_LeanCenter");
		LerpPos_LeanLeft = GetNode<Node3D>("NodeRotY/GimbalLand/NodeRotX/LerpPos_LeanLeft");
		LerpPos_LeanRight = GetNode<Node3D>("NodeRotY/GimbalLand/NodeRotX/LerpPos_LeanRight");

		//Zoom
		neededZoomValue = Camera.Fov;
		LerpObject_ObjectCameraPos.EnableUpdate(true);
		LerpObject_CameraZoom.EnableUpdate(true);
		LerpObject_CameraZoom.SetTarget(65.0f);     // Initial setup = normal fov
		lookingPoint = GetNode<Node3D>("NodeRotY/GimbalLand/NodeRotX/NodeLean/CameraLookPoint");
		LerpObject_CameraZoomToObject.EnableUpdate(true);
	}

	public void SetCharacterOwner(FPSCharacter_BasicMoving newFPSCharacter_BasicMoving)
	{
		ownerCharacter = newFPSCharacter_BasicMoving;
	}

	public override void _Process(double delta)
	{
		if (GameMaster.GM.GetIsQuitting()) return;

		FPSCharacter_Interaction character_Interaction = (FPSCharacter_Interaction)ownerCharacter;

		// CameraZoom Process
		if (Mathf.Abs(LerpObject_CameraZoom.GetTarget() - Camera.Fov) > 0.15f)
			Camera.Fov = LerpObject_CameraZoom.ActualUpdate(Camera.Fov,delta);

		//CameraZoomToObject Process
		if (Mathf.Abs(LerpObject_CameraZoomToObject.GetTarget().DistanceTo(workingZoomPosVector)) > 0.0025f &&
			isRunningZoom)
		{
			LerpObject_CameraZoomToObject.SetActual(workingZoomPosVector);
			workingZoomPosVector = LerpObject_CameraZoomToObject.Update(delta);

			Camera.LookAtFromPosition(Camera.GlobalPosition, workingZoomPosVector, Vector3.Up);
		}
		else
		{
			// jsme u cile k normalu ?
			if(LerpObject_CameraZoomToObject.GetTarget() == lookingPoint.GlobalPosition && isRunningZoom)
			{
				if(!isZoomRotSetDirection)
				{
					Camera.Rotation = Vector3.Zero;
					ownerCharacter.SetInputEnable(true);
					SetCameraLookInputEnable(true);
					isRunningZoom = false;
				}
			}
			
			// jsme u cile k objektu ?
			else if(LerpObject_CameraZoomToObject.GetTarget() != lookingPoint.GlobalPosition && isRunningZoom)
			{
				// ma se nastavit i celkovy pohled playera na novou pozici ?
				if(isZoomRotSetDirection)
				{
					NodeRotY.LookAt(LerpObject_CameraZoomToObject.GetTarget(), Vector3.Up);
					var a = NodeRotY.Rotation;
					a.X = 0.0f;
					NodeRotY.Rotation = a;

					NodeRotX.LookAt(LerpObject_CameraZoomToObject.GetTarget(), Vector3.Up);
					a = NodeRotX.Rotation;
					a.Y = 0.0f;
					NodeRotX.Rotation = a;

					Camera.Rotation = Vector3.Zero;
					ownerCharacter.SetInputEnable(true);
					SetCameraLookInputEnable(true);
					isRunningZoom = false;

					isZoomRotSetDirection = false;

					//Zoom to normal ?
					if(isOnHitTargetZoomToNormal)
						SetZoom(false);
				}
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (GameMaster.GM.GetIsQuitting()) return;

		base._PhysicsProcess(delta);

		/* Nacte hodnoty (axis right) gamepadu */
		Vector2 JoyLook = new Vector2(Input.GetActionStrength("LookJoyRight") - Input.GetActionStrength("LookJoyLeft"),
			-(Input.GetActionStrength("LookJoyUp") - Input.GetActionStrength("LookJoyDown")));

		/* Pokud JoyLook ma nejakou hodnotu (pohnuto packou na gamepadu) = gamepad jinak mys */
		if(JoyLook.Length() > 0 && ownerCharacter.IsInputEnable())
			UpdateCameraLook(JoyLook*15.0f, delta);
		else if (ownerCharacter.IsInputEnable())
            UpdateCameraLook(_MouseMotion, delta);

		// new lerp object camera pos to player head
		LerpObject_ObjectCameraPos.SetAllParam(GlobalPosition,
			ownerCharacter.HeadHolderCamera.GlobalPosition, ownerCharacter.LerpSpeedPosObjectCamera);

		GlobalPosition = LerpObject_ObjectCameraPos.Update(delta);

		_MouseMotion = new Vector2(0, 0);
	}

	// Hadle inout for mouse
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion && ownerCharacter.IsInputEnable() && isCameraLookInputEnable)
		{
			InputEventMouseMotion mouseEventMotion = @event as InputEventMouseMotion;
			_MouseMotion = mouseEventMotion.Relative;
		}
		/*
		if(@event is InputEventJoypadMotion && ownerCharacter.IsInputEnable() && isCameraLookInputEnable)
		{
			InputEventJoypadMotion a = @event as InputEventJoypadMotion;
			//_MouseMotion.X = a.GetActionStrength("right_horizontal");
			//_MouseMotion.Y = a.GetActionStrength("right_vertical");
			if (a.Axis == JoyAxis.RightX)
				_MouseMotion.X = a.AxisValue;

			GD.Print("test");
		
		}*/
	}

	// Update CameraLook from mouse input and calculating rotation nodeRotY and nodeRotX
	public void UpdateCameraLook(Vector2 newMouseMotion, double delta)
	{
		// Lerping mouse motion for smooth look (x,y)
		_LookVelocity.X = Mathf.Lerp(_LookVelocity.X, newMouseMotion.X * ownerCharacter.MouseSensitivity,
			(float)delta * ownerCharacter.MouseSmooth);

		_LookVelocity.Y = Mathf.Lerp(_LookVelocity.Y, newMouseMotion.Y * ownerCharacter.MouseSensitivity,
			(float)delta * ownerCharacter.MouseSmooth);

		// Set new rotates
		NodeRotY.RotateY(-Mathf.DegToRad(_LookVelocity.X));
		NodeRotX.RotateX(-Mathf.DegToRad(_LookVelocity.Y));

		// Set clamp camera vertical look
		Vector3 actualRotX = NodeRotX.Rotation;
		actualRotX.X = Mathf.Clamp(actualRotX.X,
			Mathf.DegToRad(ownerCharacter.CameraVerticalLookMin),
			Mathf.DegToRad(ownerCharacter.CameraVerticalLookMax));

	   NodeRotX.Rotation = actualRotX;

		// Reset MouseMotion
		_MouseMotion = Vector2.Zero;
	}

	// Povoli ci zakaze lerp tohoto objektu k characteru hlavy
	public void SetLerpToCharacterEnable(bool newEnable)
	{
		LerpObject_ObjectCameraPos.EnableUpdate(newEnable);
	}

	public void SetActualLean(ELeanType newLeanType)
	{
		if (isStopped) return;

		// ziskame dostupnost funkci(WalkingEffects) z naseho zakladniho charactera
		FPSCharacter_WalkingEffects characterWalkingEffects = (FPSCharacter_WalkingEffects)ownerCharacter;
		if (characterWalkingEffects == null) return;

		// vypocet nove pozice pro aktualni leaning
		Vector3 finalLeanPos = CalculateLeanPositionWithRaycasts(newLeanType, 
			characterWalkingEffects.LeanRaycastsTestLength,characterWalkingEffects.LeanMaxPositionDistanceX,
			characterWalkingEffects.LeanMinCameraDistanceFromWall,characterWalkingEffects.LeanMultiRaycastDetect,
			characterWalkingEffects.LeanMultiRaycastSteps);

		// vypocet nove rotace
		Vector3 finalLeanRot = CalculateLeanRotation(newLeanType,finalLeanPos,
			characterWalkingEffects.LeanMaxPositionDistanceX,characterWalkingEffects.LeanMaxRotateDistanceZ);

		// Rot
		tweenLeanRot = CreateTween();
		tweenLeanRot.TweenProperty(NodeLean, "rotation", finalLeanRot,
			characterWalkingEffects.LeanPositionTweenTime).SetEase(Tween.EaseType.OutIn);
		
		// Pos
		tweenLeanPos = CreateTween();
		tweenLeanPos.TweenProperty(NodeLean, "position", finalLeanPos,
			characterWalkingEffects.LeanPositionTweenTime).SetEase(Tween.EaseType.OutIn);
	
	}

	public ELeanType GetActualLean() { return ActualLean; }

	private Vector3 CalculateLeanPositionWithRaycasts(ELeanType newLeanType,float newRayLength,
		float newLeanMaxPositionX,float newLeanMinDistanceFromWall, bool multiRaycast, float multiRaycastStep)
	{
		// provede raycasty detekci kolize v pozadovanem smeru pro vyklon,
		// pokud nejaky kolizni bod je nastavujeme podle vzdalenosti (center <-> collision_point) novou pozici,
		// ktera prakticky snizuje puvodni maximalni rozsah leaningu.

		// pokud zadny kolizni bod neni, pouzijeme v pozadovanem smeru plny rozsah leaningu
		// ve finale tedy vracime vypocitany novy lokalni bod(pozici) leaningu

		// multiraycast je zalozeny na 3 raycastech z predniho, prostredniho (stejny jako v singleRaycast) a zadniho

		Vector3 returnedVector = Vector3.Zero;
		float direction_x = 0;
		float ray_length = newRayLength;
		float leanMaxPositionX = newLeanMaxPositionX;
		float leanMinCameraDistanceFromWall = newLeanMinDistanceFromWall;

		switch (newLeanType)
		{
			case ELeanType.Center:
				{
					returnedVector = Vector3.Zero;
					return returnedVector;
				}
			case ELeanType.Left:
				{
					// raycast smer po ose x doleva
					direction_x = -1;
					returnedVector = new Vector3(leanMaxPositionX * direction_x, 0, 0);
					break;
				}
			case ELeanType.Right:
				{
					// raycast smer po ose x doprava
					direction_x = 1;
					returnedVector = new Vector3(leanMaxPositionX * direction_x, 0, 0);
					break;
				}
		}

		// detect by single raycast
		if (multiRaycast == false)
		{
			// 1. main raycast
			UniversalFunctions.HitResult hitResult = UniversalFunctions.IsSimpleRaycastHit(this,
				NodeRotX.GlobalPosition,
				NodeRotX.GlobalPosition +
				NodeLean.GlobalTransform.Basis.X.Normalized() * (ray_length * direction_x), 1);

			if (hitResult.isHit)
			{
				float hitLength = NodeRotX.GlobalPosition.DistanceTo(hitResult.HitPosition) -
				leanMinCameraDistanceFromWall;

				if (hitLength < leanMaxPositionX)
				{
					GameMaster.GM.GetDebugHud().CustomLabelUpdateText(4, this, "raycast for lean: " + hitLength);

					returnedVector = LerpPos_LeanCenter.Position +
						(NodeRotX.Transform.Basis.Y.Normalized() * (hitLength * direction_x));
				}
			}
		}
		else
		{
			// detect by multiraycast

			// 1. main raycast
			UniversalFunctions.HitResult hitResult = UniversalFunctions.IsSimpleRaycastHit(this,
				NodeRotX.GlobalPosition,
				NodeRotX.GlobalPosition +
				NodeRotX.GlobalTransform.Basis.X.Normalized() * (ray_length * direction_x), 1);

			// 2. predni raycast
			UniversalFunctions.HitResult hit2Result = UniversalFunctions.IsSimpleRaycastHit(this,
				NodeRotX.GlobalPosition + (NodeRotX.GlobalTransform.Basis.Z.Normalized() * -multiRaycastStep),
				NodeRotX.GlobalPosition + (NodeRotX.GlobalTransform.Basis.Z.Normalized() * -multiRaycastStep) +
				NodeRotX.GlobalTransform.Basis.X.Normalized() * ((ray_length) * direction_x), 1);

			// 3. zadni raycast
			UniversalFunctions.HitResult hit3Result = UniversalFunctions.IsSimpleRaycastHit(this,
				NodeRotX.GlobalPosition + (NodeRotX.GlobalTransform.Basis.Z.Normalized() * multiRaycastStep),
				NodeRotX.GlobalPosition + (NodeRotX.GlobalTransform.Basis.Z.Normalized() * multiRaycastStep) +
				NodeRotX.GlobalTransform.Basis.X.Normalized() * ((ray_length) * direction_x), 1);

			if (hitResult.isHit || hit2Result.isHit || hit3Result.isHit)
			{
				float hitLength = NodeRotX.GlobalPosition.DistanceTo(hitResult.HitPosition) - 
					leanMinCameraDistanceFromWall;

				float hit2Length = NodeRotX.GlobalPosition.DistanceTo(hit2Result.HitPosition) -
					leanMinCameraDistanceFromWall;

				float hit3Length = NodeRotX.GlobalPosition.DistanceTo(hit3Result.HitPosition) -
					leanMinCameraDistanceFromWall;

				float nejmensi = hitLength;
				if(hitLength < hit2Length && hitLength < hit3Length)
				{
					//hitLength je nejmensi
					nejmensi = hitLength;
				}
				else if (hit2Length < hitLength && hit2Length < hit3Length)
				{
					//hit2Length je nejmensi
					nejmensi = hit2Length;
				}
				else if(hit3Length < hitLength && hit3Length < hit2Length)
				{
					//hit3Length je nejmensi
					nejmensi = hit3Length;
				}

				if (nejmensi < leanMaxPositionX)
				{
					GameMaster.GM.GetDebugHud().CustomLabelUpdateText(4, this, "raycast for lean: " + nejmensi);

					returnedVector = LerpPos_LeanCenter.Position +
						(NodeRotX.Transform.Basis.X.Normalized() * (nejmensi * direction_x));
				}
			}
		}
		return returnedVector;
	}

	private Vector3 CalculateLeanRotation(ELeanType newLeanType, Vector3 newActualLeanPos,
		float newMaxLeanPosX, float newMaxLeanRotZ)
	{
		// Vypocet lean rotace pomoci procentualniho rozsahu pozice z nehoz vypocteme procenualni rozsahu rotace
		// na konc vracime novy vector jako novou lokalni rotaci leaningu

		Vector3 returnedVector = Vector3.Zero;
		float direction_x = 0.0f;

		switch (newLeanType)
		{
			case ELeanType.Center:
				{
					returnedVector = Vector3.Zero;
					return returnedVector;
				}
			case ELeanType.Left:
				{
					returnedVector.Z = newMaxLeanRotZ;
					direction_x = 1;
					break;
				}
			case ELeanType.Right:
				{
					returnedVector.Z = -newMaxLeanRotZ;
					direction_x = -1;
					break;
				}
		}

		// aktualni distance lean
		float testDistance = newMaxLeanPosX - (newMaxLeanPosX - Mathf.Abs(newActualLeanPos.X));

		//percentage rozsah lean pos
		float percentage_pos = testDistance / newMaxLeanPosX * 100f;
		
		//percentage rozsah lean rot
		float percentage_rot_step = newMaxLeanRotZ / 100f;
		float per_rot_final = (percentage_pos * percentage_rot_step) * direction_x;

		// nastavime finalni vector
		returnedVector.Z = per_rot_final;

		return returnedVector;
	}

	public void SetZoom(bool newZoom, float newZoomValue = -1.0f, float newZoomInterpSpeed = -1.0f)
	{
		FPSCharacter_Interaction characterInteraction = (FPSCharacter_Interaction)ownerCharacter;

		// true = zoomed, false = zoom to normal value
		if (newZoom)
		{
			// zoom value
			if(newZoomValue == -1.0f)
				//neededZoomValue = characterInteraction.CameraFovZoomed;
				LerpObject_CameraZoom.SetTarget(characterInteraction.CameraFovZoomed);
			else
				//neededZoomValue = newZoomValue;
				LerpObject_CameraZoom.SetTarget(newZoomValue);

			// speed value
			if (newZoomInterpSpeed == -1)
				LerpObject_CameraZoom.SetSpeed(characterInteraction.CameraFovInterpSpeed);
			else
				LerpObject_CameraZoom.SetSpeed(newZoomInterpSpeed);
		}
		else
		{
			if (isZoomRotSetDirection) return;

			// zoom to normal - vzdy k normal hodnote a rychlosti ktere je definovane v FPSCharaterInteraction
			LerpObject_CameraZoom.SetTarget(characterInteraction.CameraFovNormal);

			// speed value
			if (newZoomInterpSpeed == -1)
				LerpObject_CameraZoom.SetSpeed(characterInteraction.CameraFovInterpSpeed);
			else
				LerpObject_CameraZoom.SetSpeed(newZoomInterpSpeed);
		}
	}

	public void SetZoomToObject(bool newZoom, Node3D newZoomedObject,bool newSetPlayerDirection,
		float newZoomValue, float newZoomInterpSpeed, float newZoomRotateSpeed,
		bool newOnHitTargetZoomToNormal)
	{
		FPSCharacter_Interaction characterInteraction = (FPSCharacter_Interaction)ownerCharacter;

		// true = zoomed + faced to, false = zoom + faced to normal
		if (newZoom)
		{
			if (newZoomedObject == null) return;

			if (isRunningZoom == false)
				workingZoomPosVector = lookingPoint.GlobalPosition;

			isRunningZoom = true;

			SetCameraLookInputEnable(false);
			ownerCharacter.SetInputEnable(false);

			// speed value for move lerp object
			if(newZoomInterpSpeed == -1)
				LerpObject_CameraZoomToObject.SetSpeed(characterInteraction.
					CameraZoomToObjectInterpRotationSpeed);
			else
				LerpObject_CameraZoomToObject.SetSpeed(newZoomInterpSpeed);

			// Set Target Zoom
			LerpObject_CameraZoomToObject.SetTarget(Camera.GlobalPosition +
				Camera.GlobalPosition.DirectionTo(newZoomedObject.GlobalPosition) * 1.0f);

			// Zoom
			if (newZoomValue == -1)
				SetZoom(true, characterInteraction.CameraFovZoomed,
					characterInteraction.CameraFovInterpSpeed);
			else
				SetZoom(true, newZoomValue, newZoomInterpSpeed);

			// Ma se nastavit novy direction hrace na vychozi bod?
			isZoomRotSetDirection = newSetPlayerDirection;

			// Ma se vratit zoom to normal kdyz dorazi k cilovemu objektu pohled kamery?
			isOnHitTargetZoomToNormal = newOnHitTargetZoomToNormal;
		}
		else
		{
			if(isRunningZoom)
			{
				if (isZoomRotSetDirection) return;

				// Rotation to
				if (newZoomInterpSpeed == -1)
					LerpObject_CameraZoomToObject.SetSpeed(characterInteraction.
						CameraZoomToObjectInterpRotationSpeed);
				else
					LerpObject_CameraZoomToObject.SetSpeed(newZoomInterpSpeed);

				LerpObject_CameraZoomToObject.SetTarget(lookingPoint.GlobalPosition);

				// Zoom
				if (newZoomValue == -1)
					SetZoom(false, characterInteraction.CameraFovZoomed,
						characterInteraction.CameraFovInterpSpeed);
				else
					SetZoom(false, newZoomValue, newZoomInterpSpeed);
			}
		}

	}

	public Marker3D GetHandGrabMarker()
	{
		return HandGrabMarker;
	}

	public void SetCameraLookInputEnable(bool newEnable)
	{
		isCameraLookInputEnable = newEnable;
	}

	public bool GetCameraLookInputEnable()
	{
		return isCameraLookInputEnable;
	}

	public void FreeAll()
	{
		isStopped = true;
		tweenLeanPos.Stop();
		tweenLeanRot.Stop();
		tweenLeanPos.Kill();
		tweenLeanRot.Kill();
		tweenLeanPos.Dispose();
		tweenLeanRot.Dispose();
	}
}

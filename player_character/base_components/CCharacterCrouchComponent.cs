using Godot;
using System;
using System.Threading.Tasks;

public partial class CCharacterCrouchComponent : CBaseComponent
{
    [Export] public bool TOGGLE_CROUCH = false;
    [Export(PropertyHint.Range, "0.3,2,0.1")] public float CROUCH_TIME = 0.5f;

    [Export(PropertyHint.Range, "0.1,2,0.01")] public float CROUCH_CAM_POS = 1.2f;
    [Export(PropertyHint.Range, "0.1,2,0.01")] public float CROUCH_SHAPE_HEIGHT = 1.3f;
    [Export(PropertyHint.Range, "0.1,2,0.01")] public float CROUCH_SHAPE_POS = 0.65f;
    [Export(PropertyHint.Range, "0.1,2,0.01")] public float UNCROUCH_CAM_POS = 1.75f;
    [Export(PropertyHint.Range, "0.1,2,0.01")] public float UNCROUCH_SHAPE_HEIGHT = 1.8f;
    [Export(PropertyHint.Range, "0.1,2,0.01")] public float UNCROUCH_SHAPE_POS = 0.9f;

    [Export] public AudioStream AudioStreamCrouch;
    [Export] public AudioStream AudioStreamUnCrouch;

    private ShapeCast3D shapeCastUncrouch;
    private AudioStreamPlayer audioStreamPlayerCrouch;

    private bool isCrouching = false;

    Tween tweenCrouchPos = null;
    private Node3D CameraCrouch = null;
    private CollisionShape3D CharacterCollision = null;

    [Export] ShapeCast3D ShapeCastCrouchDynamic;
    Tween tweenCrouchDynamic = null;
    Node3D HeadCrouchDynamic = null;

    private bool isCrouchingExtra = false;

    public override void _Ready()
	{
        base._Ready();

        shapeCastUncrouch = GetNode<ShapeCast3D>("ShapeCastUncrouch");
        audioStreamPlayerCrouch = GetNode<AudioStreamPlayer>("AudioStreamPlayer_Crouch");
    }

    public override void PostInit(FpsCharacterBase newOurCharacter)
    {
        base.PostInit(newOurCharacter);

        shapeCastUncrouch.AddException(ourCharacterBase);
        ShapeCastCrouchDynamic.AddException(ourCharacterBase);

        CameraCrouch = ourCharacterBase.GetCharacterLookComponent().GetCameraCrouch();
        CharacterCollision = ourCharacterBase.GetCharacterCollisionShape();

        HeadCrouchDynamic = ourCharacterBase.GetNode<Node3D>("%HeadCrouchDynamic");
    }

    public void CheckAndApplyCrouch(StringName newInput)
    {
        if (ourCharacterBase.GetCharacterMovementComponent() == null) return;
        bool isOnFloor = ourCharacterBase.GetCharacterMovementComponent().GetIsOnFloor();

        if (TOGGLE_CROUCH == true)
        {
            if (Input.IsActionJustPressed(newInput) && isOnFloor)
                ToggleCrouch();
        }
        else if(TOGGLE_CROUCH == false)
        {
            if (Input.IsActionJustPressed(newInput) && isOnFloor)
                SetCrouch(true);
            else if (Input.IsActionJustReleased(newInput) || isOnFloor == false)
                if(isCrouching == true)
                    SetCrouch(false);
        }
    }

    private void ToggleCrouch(){ SetCrouch(!isCrouching); }
    private void SetCrouch(bool newCrouch)
    {
        if (newCrouch)
        {
            SetTweenCrouch(true);
            audioStreamPlayerCrouch.Stream = AudioStreamCrouch;
            audioStreamPlayerCrouch.Play();
            isCrouching = true;
        }
        else if (shapeCastUncrouch.IsColliding() == false)
        {
            SetTweenCrouch(false);
            audioStreamPlayerCrouch.Stream = AudioStreamUnCrouch;
            audioStreamPlayerCrouch.Play();
            isCrouching = false;
        }
    }

    private async void Uncrouch_Check()
    {
        if (shapeCastUncrouch.IsColliding() == false)
        {
            SetCrouch(false);
        }
        else if (shapeCastUncrouch.IsColliding() == true)
        {
            await Task.Delay(100);
            Uncrouch_Check();
        }
    }

    public bool GetIsCrouched() {  return isCrouching; }

    public void SetTweenCrouch(bool newCrouchState)
    {
        if(tweenCrouchPos != null)
            tweenCrouchPos.Kill();


        if(newCrouchState)
        {
            //GD.Print("Crouched");
            tweenCrouchPos = GetTree().CreateTween();
            tweenCrouchPos.Parallel().TweenProperty(
                CameraCrouch, "position", new Vector3(0, CROUCH_CAM_POS, 0), CROUCH_TIME)
                .SetTrans(Tween.TransitionType.Cubic);

            tweenCrouchPos.Parallel().TweenProperty(
                CharacterCollision, "position", new Vector3(0, CROUCH_SHAPE_POS, 0), CROUCH_TIME);
            tweenCrouchPos.Parallel().TweenProperty(
                CharacterCollision, "shape:height", CROUCH_SHAPE_HEIGHT, CROUCH_TIME);

        }
        else
        {
            //GD.Print("Uncrouched");
            tweenCrouchPos = GetTree().CreateTween();
            tweenCrouchPos.TweenProperty(
                CameraCrouch, "position", new Vector3(0, UNCROUCH_CAM_POS, 0), CROUCH_TIME)
                .SetTrans(Tween.TransitionType.Cubic);

            tweenCrouchPos.Parallel().TweenProperty(
                CharacterCollision, "position", new Vector3(0, UNCROUCH_SHAPE_POS, 0), CROUCH_TIME);
            tweenCrouchPos.Parallel().TweenProperty(
                CharacterCollision, "shape:height", UNCROUCH_SHAPE_HEIGHT, CROUCH_TIME);

        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        SetTweenCrouchCamDynamic();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public void SetTweenCrouchCamDynamic(bool activeDynamic = true)
    {
        Vector3 FloorPos = Vector3.Zero;

        if (ShapeCastCrouchDynamic.IsColliding())
        {
            FloorPos = ShapeCastCrouchDynamic.GetCollisionPoint(0);
            FloorPos.Y = ourCharacterBase.GlobalPosition.Y + 0.3f;
            float distance = FindUpHeightForCrouch(ourCharacterBase, FloorPos);

            if(distance != -9999)
            {
                GD.Print("height from floor: " + distance.ToString());
            }
  
            else
            {
                GD.Print("height from floor: 0");
            }

            if (tweenCrouchDynamic != null)
                tweenCrouchDynamic.Kill();

            tweenCrouchDynamic = GetTree().CreateTween();
            tweenCrouchDynamic.TweenProperty(
                HeadCrouchDynamic, "position", new Vector3(0, -CameraCrouch.Position.Y+distance, 0), 0.03f)
                .SetTrans(Tween.TransitionType.Cubic);

            isCrouchingExtra = true;
        }
        else
        {
            if (tweenCrouchDynamic != null)
                tweenCrouchDynamic.Kill();

            tweenCrouchDynamic = GetTree().CreateTween();
            tweenCrouchDynamic.TweenProperty(
                HeadCrouchDynamic, "position", new Vector3(0, 0.0f, 0), 0.03f)
                .SetTrans(Tween.TransitionType.Cubic);

            isCrouchingExtra = false;
        }
    }

    public float FindUpHeightForCrouch(Node3D newCaller, Vector3 newPos)
    {
        PhysicsDirectSpaceState3D directSpace = newCaller.GetWorld3D().DirectSpaceState;

        PhysicsRayQueryParameters3D rayParam = new PhysicsRayQueryParameters3D();
        rayParam.From = newPos;
        rayParam.To = newPos + (Vector3.Up * 1.5f);

        Vector3 hit_pos = new Vector3(-9999, -9999, -9999);
        float distance = -9999.0f;

        var rayResult = directSpace.IntersectRay(rayParam);
        if (rayResult.Count > 0)
        {
            hit_pos = (Vector3)rayResult["position"];
        }
        
        if(hit_pos != new Vector3(-9999,-9999,-9999))
        {
            distance = Mathf.Abs(newPos.DistanceTo(hit_pos));
        }

        return distance;
    }

    public bool GetIsCrouchExtra() { return isCrouchingExtra; }

}

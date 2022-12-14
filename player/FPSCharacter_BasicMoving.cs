using Godot;
using System;

/*
 * *** FPSCharacter_WalkingEffects(0.1) ***
 * 
 * - must be "block on walk" set to false !
 * - walk mode with jump and gravity
 * - fly mode
 * - mouse looking
 * - smoothed calculates move
 * - smoothed crunch mechanism with change speed and stopJumping
 * - can sprint
 * - raycast testing ceil from crunch->stand
 * - fixed sliding wall strange add speed (fixed)
 * - TODO fix strange move (reproduce oalar bug?)
 * - TODO fix onGround/Falling/InAir detect system (create new sytem ?)
*/
public partial class FPSCharacter_BasicMoving : CharacterBody3D
{
    protected Node3D HeadMain = null;
    protected Node3D HeadGimbalA = null;
    protected Node3D HeadGimbalB = null;
    public Node3D HeadHolderCamera = null;

    [Export] public ObjectCamera objectCamera;

    private CollisionShape3D CharacterCollisionStand = null;
    private CollisionShape3D CharacterCollisionCrunch = null;
    private Node3D HeadStandPosition = null;
    private Node3D HeadCrunchPosition = null;

    public enum ECharacterMode { FlyMode, WalkMode }
    public enum ECharacterPosture { Stand, Crunch }

    [ExportGroupAttribute("Movement Settings")]
    [Export] public ECharacterMode CharacterMode = ECharacterMode.WalkMode;
    [Export] public bool CanSprint = true;
    [Export] public bool CanJump = true;
    [Export] public bool CanMoveInFall = true;
    [Export] public float JumpVelocity = 4.3f;
    [Export] public float MoveSpeedInStand = 2.25f;
    [Export] public float MoveSpeedInCrunch = 1.4f;
    [Export] public float MoveSpeedInSprint = 4.1f;
    [Export] public float MoveSpeedInFall = 1.4f;
    [Export] public float AccelerateSmoothStep = 6f;
    [Export] public float DeccelerateSmoothStep = 6f;
    [Export] public float DeccelerateInFallSmoothStep = 1.0f;

    [ExportGroupAttribute("Looking Settings")]
    [Export] public float MouseSensitivity = 0.15f;
    [Export] public float MouseSmooth = 15f;
    [Export] public float CameraVerticalLookMin = -80f;
    [Export] public float CameraVerticalLookMax = 80f;
    [Export] public float LerpSpeedPosObjectCamera = 15.0f;

    [ExportGroupAttribute("Others Settings")]
    [Export] public float CrunchLerpSpeed = 5.0f;
    [Export] public float LandingLimitMoveVelocity = 1.5f;

    private float _Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
    private float _ActualSetMoveSpeed;
    protected bool _isSprint = false;
    private bool _isFalling = false;
    private ECharacterPosture _ActualCharacterPosture = ECharacterPosture.Stand;
    private Vector3 _HeadMainLerpTarget;    // uses for crunch<->stand move lerp camera
    private bool _isInputEnable = true;
    private bool _isMoveInputEnable = true;

    private bool isFallingStart = false;

    public float ActualMovementSpeed = 0.0f;
    public Vector3 LastPosition = Vector3.Zero;

    // for detect amount damage from fall
    private float lastYPosFallingStart = 0.0f;
    private float lastYPosFallingEnd = 0.0f;
    private float lastYVelocity = 0.0f;
    float heightfallingtest = 0.0f;

    private Control allHuds = null;

    public override void _Ready()
    {
        // pro dostupnost skrze gamemastera
        GameMaster.GM.SetFPSCharacter(this);

        HeadMain = GetNode<Node3D>("HeadMain");
        HeadGimbalA = GetNode<Node3D>("HeadMain/HeadGimbalA");
        HeadGimbalB = GetNode<Node3D>("HeadMain/HeadGimbalA/HeadGimbalB");
        HeadHolderCamera = GetNode<Node3D>("HeadMain/HeadGimbalA/HeadGimbalB/HeadHolderCamera");
        CharacterCollisionStand = GetNode<CollisionShape3D>("CharacterCollisionStand");
        CharacterCollisionCrunch = GetNode<CollisionShape3D>("CharacterCollisionCrunch");
        HeadStandPosition = GetNode<Node3D>("HeadStandPos");
        HeadCrunchPosition = GetNode<Node3D>("HeadCrunchPos");

        SetInputEnable(true);

        _HeadMainLerpTarget = HeadStandPosition.Position;
        _ActualSetMoveSpeed = MoveSpeedInStand;

        //
        objectCamera.SetCharacterOwner(this);

        allHuds = GetNode<Control>("AllHuds");
    }

    // Update Physical updated process
    public override void _PhysicsProcess(double delta)
    {
        if (GameMaster.GM.GetIsQuitting()) return;

        switch (CharacterMode)
        {
            case ECharacterMode.FlyMode:
                {
                    // Applying velocity for fly move
                    Velocity = UpdateVelocityFlyMove(delta);
                    MoveAndSlide();
                    break;
                }
            case ECharacterMode.WalkMode:
                {
                    // Applying velocity for walk move
                    Velocity = UpdateVelocityWalkMove(delta);
                    MoveAndSlide();
                    break;
                }
        }

        // Calculate actual movement speed 
        ActualMovementSpeed = GlobalPosition.DistanceTo(LastPosition) * 20000.0f * (float)delta;
        heightfallingtest = GlobalPosition.y - LastPosition.y;


        LastPosition = GlobalPosition;
    }

    // Update Visual updated process
    public override void _Process(double delta)
    {
    }

    // Update velocity for fly move and return this velocity
    public Vector3 UpdateVelocityFlyMove(double delta)
    {
        // Get input actions and calculate direction
        Vector2 inputDir = Input.GetVector("moveLeft", "moveRight", "moveForward", "moveBackward");
        Vector3 direction = (GetFPSCharacterCamera().GlobalTransform.basis * new Vector3(inputDir.x, 0, inputDir.y)).Normalized();

        // Get actual character velocity
        Vector3 velocity = Velocity;

        // Is any input ?
        if (_isInputEnable &&
            (direction != Vector3.Zero || Input.IsActionPressed("Jump") || Input.IsActionPressed("Crunch")))
        {
            // Up ?
            if (Input.IsActionPressed("Jump"))
                direction.y = direction.y + 1.0f;

            // Down ?
            if (Input.IsActionPressed("Crunch"))
                direction.y = direction.y - 1.0f;

            velocity = velocity.Lerp(direction * _ActualSetMoveSpeed, AccelerateSmoothStep * (float)delta);
        }
        // Is not any input ?
        else
        {
            velocity = velocity.Lerp(Vector3.Zero, DeccelerateSmoothStep * (float)delta);
        }

        return velocity;
    }

    // Update velocity for walk move and return this velocity
    public Vector3 UpdateVelocityWalkMove(double delta)
    {

        // Get input actions and calculate direction
        Vector2 inputDir = Input.GetVector("moveLeft", "moveRight", "moveForward", "moveBackward");
        Vector3 direction = (objectCamera.NodeRotY.Transform.basis * new Vector3(inputDir.x, 0, inputDir.y)).Normalized();

        // Get actual character velocity
        Vector3 velocity = Velocity;
        velocity.y = 0f;    // disable gravity at this moment

        // Is any input ?
        if (_isInputEnable && direction != Vector3.Zero)
        {
            // Is character grounded ?
            if (IsOnFloor())
            {

                if(_ActualCharacterPosture == ECharacterPosture.Stand && _isSprint)
                {
                    _ActualSetMoveSpeed = MoveSpeedInSprint;
                }
                else if (_ActualCharacterPosture == ECharacterPosture.Crunch)
                {
                    _ActualSetMoveSpeed = MoveSpeedInCrunch;
                }
                else
                {
                    _ActualSetMoveSpeed = MoveSpeedInStand;
                }

                // New move Fix pro vyreseni bugu se zdi !
                if (IsOnWall() && inputDir.y != 0)
                {
                    var newDir = GetWallNormal().Slide(direction).Normalized();
                    velocity = velocity.Lerp(newDir * _ActualSetMoveSpeed, AccelerateSmoothStep * (float)delta);
                }
                else
                {
                    velocity = velocity.Lerp(direction * _ActualSetMoveSpeed, AccelerateSmoothStep * (float)delta);
                }
            }
            else
            {
                if (CanMoveInFall)
                {
                    // add additional move velocity and set limit length for final velocity
                    velocity += (direction * MoveSpeedInFall) * (float)delta;
                    velocity = velocity.LimitLength(_ActualSetMoveSpeed);
                }
            }
        }
        // Is not any input ?
        else
        {
            // Is character on ground ?
            if (IsOnFloor())
            {
                velocity = velocity.Lerp(Vector3.Zero, DeccelerateSmoothStep * (float)delta);
            }
            else
            {
                velocity = velocity.Lerp(Vector3.Zero, DeccelerateInFallSmoothStep * (float)delta);
            }
        }

        GameMaster.GM.GetDebugHud().CustomLabelUpdateText(2, this, "velocity = " + direction);

        // Function of Falling and StartFalling
        if (!IsOnFloor())
        {
            // calculate start fall down
            if (heightfallingtest < 0.01f && isFallingStart == false)
            {
                _isFalling = true;
                // Do once effect for startFalling
                EventStartFalling();
            }
        }
        else if (IsOnFloor() && _isFalling)
        {
            EventLanding();
            velocity = velocity.LimitLength(LandingLimitMoveVelocity);  // zmensi aktualni velocity
        }
        
        // We set last gravity velocity back
        velocity.y = Velocity.y;

        // Apply gravity.
        if (!IsOnFloor())
            velocity.y -= _Gravity * (float)delta;

        // Apply jump.
        if (_isInputEnable && CanJump && _ActualCharacterPosture == ECharacterPosture.Stand &&
            IsOnFloor() && Input.IsActionJustPressed("Jump"))
        {
            velocity.y = JumpVelocity;
            EventJumping();  // override for add effects
        }

        // Apply crunch
        if (_isInputEnable && IsOnFloor() && Input.IsActionJustPressed("Crunch"))
            CrunchToggle();

        // want sprint ?
        if (_isInputEnable && CanSprint && IsOnFloor() && Input.IsActionPressed("Sprint"))
            _isSprint = true;
        else
            _isSprint = false;

        //Calculating interpolation YPosition Head/Camera for crunch<->stand process
        HeadMain.Position = HeadMain.Position.Lerp(_HeadMainLerpTarget, CrunchLerpSpeed * (float)delta);

        return velocity;
    }

    // Change character posture like stand,crunch.. its also change player movement speed
    public void ChangeCharacterPosture(ECharacterPosture newCharacterPosture)
    {
        switch (newCharacterPosture)
        {
            case ECharacterPosture.Crunch:
                {
                    CharacterCollisionStand.Disabled = true;
                    CharacterCollisionCrunch.Disabled = false;
                    _HeadMainLerpTarget = HeadCrunchPosition.Position;
                    _ActualSetMoveSpeed = MoveSpeedInCrunch;

                    _ActualCharacterPosture = ECharacterPosture.Crunch;
                    break;
                }
            case ECharacterPosture.Stand:
                {
                    CharacterCollisionStand.Disabled = false;
                    CharacterCollisionCrunch.Disabled = true;
                    _HeadMainLerpTarget = HeadStandPosition.Position;
                    _ActualSetMoveSpeed = MoveSpeedInStand;

                    _ActualCharacterPosture = ECharacterPosture.Stand;
                    break;
                }
        }
    }

    // Crunch<->Stand toggle
    public void CrunchToggle()
    {
        switch (_ActualCharacterPosture)
        {
            case ECharacterPosture.Stand: { ChangeCharacterPosture(ECharacterPosture.Crunch); break; }
            case ECharacterPosture.Crunch:
                {
                    if (CheckCanStandFromCrunch())
                        ChangeCharacterPosture(ECharacterPosture.Stand);
                    break;
                }
        }
    }

    // Function for detect space above player head
    public bool CheckCanStandFromCrunch()
    {
        bool canStand = false;
        bool r1 = false, r2 = false, r3 = false, r4 = false, r5 = false;

        float height = ((CapsuleShape3D)CharacterCollisionStand.Shape).Height - HeadCrunchPosition.Position.y;
        float radius = ((CapsuleShape3D)CharacterCollisionStand.Shape).Radius;
        Vector3 crunchHeadPos = HeadCrunchPosition.GlobalPosition;

        // R1 Center
        if (isSimpleRayHit(
            crunchHeadPos,
            crunchHeadPos + (Vector3.Up * height)))
        { r1 = true; }

        // R2 ForwardRight
        if (isSimpleRayHit(
            crunchHeadPos + (Vector3.Forward * radius) + (Vector3.Right * radius),
            crunchHeadPos + (Vector3.Forward * radius) + (Vector3.Right * radius) + (Vector3.Up * height)))
        { r2 = true; }

        // R3 ForwardLeft
        if (isSimpleRayHit(
             crunchHeadPos + (Vector3.Forward * radius) + (Vector3.Left * radius),
             crunchHeadPos + (Vector3.Forward * radius) + (Vector3.Left * radius) + (Vector3.Up * height)))
        { r3 = true; }

        // R4 BackLeft
        if (isSimpleRayHit(
            crunchHeadPos + (Vector3.Back * radius) + (Vector3.Left * radius),
            crunchHeadPos + (Vector3.Back * radius) + (Vector3.Left * radius) + (Vector3.Up * height)))
        { r4 = true; }

        // R5 Back Right
        if (isSimpleRayHit(
            crunchHeadPos + (Vector3.Back * radius) + (Vector3.Right * radius),
            crunchHeadPos + (Vector3.Back * radius) + (Vector3.Right * radius) + (Vector3.Up * height)))
        { r5 = true; }

        // Testing all rays results
        if (!r1 && !r2 && !r3 && !r4 && !r5)
            canStand = true;
        else
            canStand = false;

        return canStand;
    }

    // Simple ray detect - if hit something - return true or false
    public bool isSimpleRayHit(Vector3 from, Vector3 to)
    {
        bool isHit = false;

        PhysicsDirectSpaceState3D directSpace = GetWorld3d().DirectSpaceState;

        PhysicsRayQueryParameters3D rayParam = new PhysicsRayQueryParameters3D();
        rayParam.From = from;
        rayParam.To = to;

        var rayResult = directSpace.IntersectRay(rayParam);
        if (rayResult.Count > 0)
            isHit = true;

        return isHit;
    }

    // Enable/Disable all inters Inputs for this Fps character
    public void SetInputEnable(bool newEnable)
    {
        _isInputEnable = newEnable;

        if (_isInputEnable)
            Input.MouseMode = Input.MouseModeEnum.Captured;
        else
            Input.MouseMode = Input.MouseModeEnum.Visible;

        SetMoveInputEnable(_isInputEnable);
        //objectCamera.IsCameraLookInputEnable(_isInputEnable);
    }

    public void SetMoveInputEnable(bool newEnable)
    {
        _isMoveInputEnable = newEnable;
    }

    public void SetMouseVisible(bool newMouseVisible)
    {
        if (newMouseVisible)
            Input.MouseMode = Input.MouseModeEnum.Visible;
        else
            Input.MouseMode = Input.MouseModeEnum.Hidden;
    }

    public bool IsInputEnable()
    {
        return _isInputEnable;
    }

    // Event when character start falling
    public virtual void EventStartFalling()
    {
        // for do once effect, we just need this function like event
        isFallingStart = true;

        // save actual y pos of character
        lastYPosFallingStart = GlobalPosition.y;
        //GD.Print("start falling y= " + lastYPosFallingStart);
    }

    // Event when character hit the floor = landed
    public virtual void EventLanding()
    {
        //GameMaster.GM.Log.WriteLog(this, LogSystem.ELogMsgType.INFO, "character landing");

        // actual state of falling is false
        _isFalling = false;

        // for reset do once effect = after landing, we need check next startFalling
        isFallingStart = false;

        // save actual y pos of character
        lastYPosFallingEnd = GlobalPosition.y;
        // execute landing effect event with param of fall height
        EventLandingEffect(lastYPosFallingStart - lastYPosFallingEnd);

        // calculate height of falling
        //GameMaster.GM.Log.WriteLog(this,LogSystem.ELogMsgType.INFO, "height from start falling: " + (lastYPosFallingStart - lastYPosFallingEnd) +" m");

    }

    public virtual void EventLandingEffect(float heightfall)
    {
        // virtual
    }

    // Event when character press jump
    public virtual void EventJumping()
    {
        //GameMaster.GM.Log.WriteLog(this, LogSystem.ELogMsgType.INFO, "character jumped");
    }

    public virtual void EventMovingStopped()
    {
        //GameMaster.GM.Log.WriteLog(this, LogSystem.ELogMsgType.INFO, "character moving stoped");
    }

    // Event when character become actual dead
    public virtual void EventDead(string reasonDead)
    {
        //GameMaster.GM.Log.WriteLog(this, LogSystem.ELogMsgType.INFO, "character is dead");
    }

    // Return by linked ObjectCamera->Camera
    public Camera3D GetFPSCharacterCamera()
    {
        return objectCamera.Camera;
    }

    public Control GetAllHudsControlNode()
    {
        return allHuds;
    }

    public virtual void FreeAll()
    {
        QueueFree();
    }
}
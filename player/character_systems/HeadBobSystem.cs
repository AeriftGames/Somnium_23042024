using Godot;
using System;
using System.Buffers;
using static FPSCharacter_BasicMoving;

public partial class HeadBobSystem : Node
{
    InventoryObjectCamera invCam = null;
    FPSCharacter_Inventory invChar = null;

    [ExportGroupAttribute("HeadBobMovingPos")]
    [Export] public float headBobbingWalkValue = 0.2f;
    [Export] public float headBobbingSprintValue = 0.3f;
    [Export] public float headBobbingCrouchValue = 0.15f;
    [Export] public float headBobbingDeltaToMove = 1.0f;
    [Export] public float headBobbingDeltaToStop = 3.0f;
    [ExportGroupAttribute("HeadBobMovingRot")]
    [Export] public float headBobRotDegWalkValue = 2.0f;
    [Export] public float headBobRotDegSprintValue = 4.0f;
    [Export] public float headBobRotDegCrouchValue = 1.5f;
    [Export] public float headBobRotDeltaToMove = 1.0f;
    [Export] public float headBobRotDeltaToStop = 3.0f;
    [ExportGroupAttribute("Other")]

    // for moving bob
    float headBobMovingX = 0.0f;
    float headBobMovingXDelta = 1.0f;
    float headBobMovingYDelta = 2.0f;
    float headBobRot = 0.0f;
    float headBobRotDelta = 1.0f;
    int lastFootState = 0;

    [Export] public float WalkCameraLerpHeight = 0.15f;
    [Export] public float RunCameraLerpHeight = 0.3f;
    [Export] public float CrouchCameraLerpHeight = 0.1f;

    float lerpHeadWalkY=0;
    int actStepsToEffect = 0;
    int numStepsToEffect = 1;

    private bool isActualStopMovement = false;

    public void Init(InventoryObjectCamera ownerCharacter) 
    { 
        invCam = ownerCharacter;
    }
    public void Update(float delta)
    {
        UpdateWalkHeadBobbing(delta);


        Vector3 pos = invCam.headWalkBobNode.Position;
        pos.X = Mathf.Lerp(pos.X, headBobMovingX, headBobMovingXDelta * delta);
        pos.Y = Mathf.Lerp(pos.Y, lerpHeadWalkY, headBobMovingYDelta * delta);
        invCam.headWalkBobNode.Position = pos;

        //invCam.headWalkBobNode.Position = invCam.headWalkBobNode.Position.Lerp(new Vector3(headBobMovingX, lerpHeadWalkY, 0), (float)delta * headBobMovingXDelta);
        invCam.headWalkBobNode.RotationDegrees = invCam.headWalkBobNode.RotationDegrees.Lerp(new Vector3(0, 0, headBobRot), (float)delta * headBobRotDelta);
    }
    
    public void UpdateWalkHeadBobbing(int actualStepState, float delta)
    {
        invChar = invCam.GetCharacterOwner() as FPSCharacter_Inventory;

        if (actualStepState == lastFootState) return;

        if (actualStepState == 0)
        {
            // noha ve vzduchu
            headBobMovingX = 0.0f;
            headBobMovingXDelta = headBobbingDeltaToStop;
            headBobRot = 0.0f;
            headBobRotDelta = headBobRotDeltaToStop;
        }
        else if (actualStepState == 1)
        {
            if (invChar.GetCharacterPosture() == ECharacterPosture.Stand)
            {
                if (invChar.GetIsSprint())
                {
                    //sprint
                    headBobMovingX = headBobbingSprintValue;
                    headBobRot = headBobRotDegSprintValue;
                }
                else
                {
                    //walk
                    headBobMovingX = headBobbingWalkValue;
                    headBobRot = headBobRotDegWalkValue;
                }
            }
            else
            {
                //crouch
                headBobMovingX = headBobbingCrouchValue;
                headBobRot = headBobRotDegCrouchValue;
            }

            headBobMovingXDelta = headBobbingDeltaToMove;
            headBobRotDelta = headBobRotDeltaToMove;
        }
        else if (actualStepState == 2)
        {
            if (invChar.GetCharacterPosture() == ECharacterPosture.Stand)
            {
                if (invChar.GetIsSprint())
                {
                    //sprint
                    headBobMovingX = -headBobbingSprintValue;
                    headBobRot = -headBobRotDegSprintValue;
                }
                else
                {
                    //walk
                    headBobMovingX = -headBobbingWalkValue;
                    headBobRot = -headBobRotDegWalkValue;
                }
            }
            else
            {
                //crouch
                headBobMovingX = -headBobbingCrouchValue;
                headBobRot = -headBobRotDegCrouchValue;
            }

            headBobMovingXDelta = headBobbingDeltaToMove;
            headBobRotDelta = headBobRotDeltaToMove;
        }

        lastFootState = actualStepState;
    }

    private void UpdateWalkHeadBobbing(float delta)
    {
        invChar = invCam.GetCharacterOwner() as FPSCharacter_Inventory;

        if (invChar.GetFootstepNow())
        {
            // foot touch ground now
            if (invChar.GetCharacterPosture() == ECharacterPosture.Stand)
            {
                if (invChar.GetIsSprint())
                    lerpHeadWalkY = -RunCameraLerpHeight;
                else
                    lerpHeadWalkY = -WalkCameraLerpHeight;
            }
            else
                lerpHeadWalkY = -CrouchCameraLerpHeight;

            if (actStepsToEffect == 0)
            {
                if (invChar.GetActualStep() == true)
                    UpdateWalkHeadBobbing(1, delta);
                else
                    UpdateWalkHeadBobbing(2, delta);
            }

            actStepsToEffect++;
        }
        else
        {
            // noha above on ground
            if (invChar.GetCharacterPosture() == ECharacterPosture.Stand)
            {
                if (invChar.GetIsSprint())
                    lerpHeadWalkY = RunCameraLerpHeight;
                else
                    lerpHeadWalkY = WalkCameraLerpHeight;
            }
            else
                lerpHeadWalkY = CrouchCameraLerpHeight;
        }

        if (actStepsToEffect >= numStepsToEffect)
            actStepsToEffect = 0;

        // if actualmove is smaller than testing value, centered headlerpY and speedUP lerp to normal 
        if (invChar.ActualMovementSpeed <= 1.0f)
        {
            // dodatecny zvuk kroku pri zastaveni, vyresetovani aktualniho footstepu na opacny footstep
            if (!isActualStopMovement)
            {
                invChar.PlayFootstepSound(-5.0f, -0.1f);

                isActualStopMovement = true;
                actStepsToEffect = 0;
                invChar.SetFootstepNow(true);
                invChar.SetActualStep(!invChar.GetActualStep());
            }

            lerpHeadWalkY = 0.0f;
            headBobMovingXDelta = headBobbingDeltaToStop;
            //
            invCam.GetHeadBobSystem().UpdateWalkHeadBobbing(0, delta);
        }
        else
        {
            isActualStopMovement = false;
        }
    }

    public bool GetIsActualStopMovement(){return isActualStopMovement;}
}

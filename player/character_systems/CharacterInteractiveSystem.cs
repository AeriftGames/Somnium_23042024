using Godot;
using System;
using System.Data;
using System.Linq.Expressions;

// propojit s basichud
// zprovoznit puvodni hud texty
// zprovoznit use a grab

public partial class CharacterInteractiveSystem : Godot.Object
{
    FPSCharacter_Interaction character = null;
    BasicHud basicHud;
    Node actualInteractiveObject = null;
    RigidBody3D pickedBody = null;
    bool isGrabbing = false;
    bool isCanNewGrab = true;
    bool wantRotateNow = false;

    public struct SPhysicalGrabbedItemParams 
    {
        public Vector3 inertia;
        public float angularDamp;
        public float linearDamp;
        public float friction;
        public float bounce;
        public float mass;
    }

    SPhysicalGrabbedItemParams lastGrabbedItemOriginalParams;

    public CharacterInteractiveSystem(FPSCharacter_Interaction newCharacterOwner, BasicHud newBasichud)
    {
        character = newCharacterOwner;
        basicHud = newBasichud;
    }

    public void Update(bool newUseNow,bool newGrabNow, double delta)
    {
        // default sets on start this update
        basicHud.SetUseVisible(false);
        basicHud.SetHandGrabState(false, false);

        // neexistuje zadny novy objekt ?
        if (actualInteractiveObject == null)
        {
            basicHud.SetUseLabelText("");
            basicHud.SetUseVisible(false);
            return;
        }

        // zjistime o jaky typ se jedna
        interactive_object interactiveObject = (interactive_object)actualInteractiveObject;
        switch(interactiveObject.InteractiveType)
        {
            case interactive_object.EInteractiveType.Static:
                {
                    UpdateStatic(interactiveObject,newUseNow, delta);
                    break;
                }
            case interactive_object.EInteractiveType.Physic:
                {
                    UpdateStatic(interactiveObject, newUseNow, delta);
                    UpdatePhysic(interactiveObject,newUseNow,newGrabNow,delta);
                    break;
                }
        }
    }

    public void SetActualInteractiveObject(Node newInteractiveObject, Vector3 hitPosition)
    {
        actualInteractiveObject = newInteractiveObject;
    }

    public void UpdateStatic(interactive_object newInteractiveObject,bool newUseNow,double delta)
    {
        // DISABLED ?
        if (newInteractiveObject.InteractiveLevel == interactive_object.EInteractiveLevel.Disable)
        {
            basicHud.SetUseLabelText("");
            basicHud.SetUseVisible(false);
        }

        // UPDATE HUD
        if(newInteractiveObject.InteractiveLevel == interactive_object.EInteractiveLevel.OnlyUse ||
            newInteractiveObject.InteractiveLevel == interactive_object.EInteractiveLevel.UseAndGrab)
        {
            basicHud.SetUseLabelText(newInteractiveObject.GetUseActionName());
            basicHud.SetUseVisible(true);
        }

        // UPDATE USE
        if (newInteractiveObject.InteractiveLevel == interactive_object.EInteractiveLevel.OnlyUse ||
            newInteractiveObject.InteractiveLevel == interactive_object.EInteractiveLevel.UseAndGrab)
        {
            if(newUseNow)
                newInteractiveObject.Use(character);
        }
    }
    public void UpdatePhysic(interactive_object newInteractiveObject, bool newUseNow,bool newGrabNow, double delta)
    {
        // Update Grab
        if(newInteractiveObject.InteractiveLevel == interactive_object.EInteractiveLevel.OnlyGrab ||
            newInteractiveObject.InteractiveLevel == interactive_object.EInteractiveLevel.UseAndGrab)
        {
            if(newGrabNow && isCanNewGrab)
                StartGrabbing((RigidBody3D)actualInteractiveObject.GetParent());
        }
    }

    public void HandGrabbingUpdate(bool newGrabNow, bool newThrowObjectNow,bool newRotateGrabbedObject,
        bool newMoveFarGrabbedObject,bool newMoveNearGrabbedObject, double delta)
    {
        // default sets on start this update
        wantRotateNow = false;
        character.objectCamera.IsCameraLookInputEnable = true;

        // otestujeme zda je tento object nastaveny pro grab a zda aktualne nejaky objekt negrabujeme
        // pokd jsou podminky splneny, nastavime viditelnou normal hand
        if (actualInteractiveObject != null && newGrabNow == false)
        {
            interactive_object interactiveObject = (interactive_object)actualInteractiveObject;
            if(interactiveObject.InteractiveLevel == interactive_object.EInteractiveLevel.OnlyGrab ||
                interactiveObject.InteractiveLevel == interactive_object.EInteractiveLevel.UseAndGrab)
            {
                basicHud.SetHandGrabState(true, false);
            }
        }

        // pokud jsme aktualne ve stavu grabbing, stale prichazi input pro grab tak
        // updatujeme pozice objektu pro grabbing
        if (isCanNewGrab && isGrabbing && newGrabNow && pickedBody != null)
        {
            basicHud.SetHandGrabState(true,true);
            // grabbing
            Vector3 pickedBodyGlobalPositon = pickedBody.GlobalTransform.origin;
            Vector3 HandGrabGlobalPosition = character.objectCamera.HandGrabMarker.GlobalPosition;

            pickedBody.LinearVelocity = (HandGrabGlobalPosition - pickedBodyGlobalPositon) * character.GrabObjectPullPower;
        }
        else if(isGrabbing && newGrabNow == false)
        {
            // zahazujeme objekt
            StopGrabbing();
            pickedBody = null;
        }

        // Throw impulse
        if (isGrabbing && newGrabNow && pickedBody != null && newThrowObjectNow)
        {
            StopGrabbing();

            pickedBody.ApplyCentralImpulse(character.GetFPSCharacterCamera().
                GlobalTransform.basis.z.Normalized() * -character.ThrowObjectPower);

            isCanNewGrab = false;
            pickedBody = null;
        }

        // Reset pro novy grab, napriklad po hodu, donuti hrace pustit tlacitko pro grab, i kdyby na jeden frame
        // pokud ho pusti, resetujeme moznost znovu grabbovat
        if (isCanNewGrab == false && newGrabNow == false)
            isCanNewGrab = true;

        // Rotate Grabbed Object
        if(isGrabbing && newGrabNow && pickedBody != null && newRotateGrabbedObject)
        {
            wantRotateNow = true;
        }

        // Move Far/Near Grabbed Object
        if(isGrabbing && newGrabNow && pickedBody != null && (newMoveFarGrabbedObject || newMoveNearGrabbedObject))
        {
            Vector3 actualPosition = character.objectCamera.GetHandGrabMarker().Position;

            if (newMoveFarGrabbedObject)
                actualPosition.z -= character.MoveFarOrNearObjectStep;

            if(newMoveNearGrabbedObject)
                actualPosition.z += character.MoveFarOrNearObjectStep;

            actualPosition.z = Mathf.Clamp(actualPosition.z,
                -character.MoveFarOrNearObjectMax, -character.MoveFarOrNearObjectMin);

            character.objectCamera.GetHandGrabMarker().Position = actualPosition;
        }
    }

    // Update prichazejici z interaction character
    public void UpdateGrabbedObjectRotate(InputEvent @event)
    {
        // Rotate Grabbed Object
        if (pickedBody != null && wantRotateNow)
        {
            character.objectCamera.IsCameraLookInputEnable = false;

            if (@event is InputEventMouseMotion && character.IsInputEnable())
            {
                InputEventMouseMotion mouseEventMotion = @event as InputEventMouseMotion;

                character.objectCamera.HandStaticBody.RotateX(
                    Mathf.DegToRad(mouseEventMotion.Relative.y * character.RotateObjectStep));

                character.objectCamera.HandStaticBody.RotateY(
                    Mathf.DegToRad(mouseEventMotion.Relative.x * character.RotateObjectStep));
            }
        }
    }

    public void StartGrabbing(RigidBody3D grabbedObject)
    {
        isGrabbing = true;

        // chceme zacit grabovat s novym objektem ?
        if(pickedBody == null && grabbedObject != null)
        {
            // Set to original handGrabPosition
            Vector3 actualPosition = character.objectCamera.GetHandGrabMarker().Position;
            actualPosition.z = -character.MoveFarOrNearObjectOriginal;
            character.objectCamera.GetHandGrabMarker().Position = actualPosition;

            // Nastavi nas novy Rigidbody object a nastavi mu pozadovane fyzikalni parametry pro grab
            pickedBody = grabbedObject;
            SetRigidBodyParamForGrab(pickedBody, true);
        }
    }
    public void StopGrabbing()
    {
        basicHud.SetHandGrabState(false, false);
        SetRigidBodyParamForGrab(pickedBody, false);
        isGrabbing = false;
        character.objectCamera.HandGrabJoint.NodeB = character.objectCamera.HandGrabJoint.GetPath();

        // Set to original handGrabPosition
        Vector3 actualPosition = character.objectCamera.GetHandGrabMarker().Position;
        actualPosition.z = -character.MoveFarOrNearObjectOriginal;
        character.objectCamera.GetHandGrabMarker().Position = actualPosition;
    }

    public void SetRigidBodyParamForGrab(RigidBody3D grabbedObject, bool newGrab)
    {
        if(newGrab)
        {
            // ulozime si originalni fyzikalni data
            lastGrabbedItemOriginalParams.inertia = grabbedObject.Inertia;
            lastGrabbedItemOriginalParams.angularDamp = grabbedObject.AngularDamp;
            lastGrabbedItemOriginalParams.linearDamp = grabbedObject.LinearDamp;
            lastGrabbedItemOriginalParams.friction = grabbedObject.PhysicsMaterialOverride.Friction;
            lastGrabbedItemOriginalParams.bounce = grabbedObject.PhysicsMaterialOverride.Bounce;
            lastGrabbedItemOriginalParams.mass = grabbedObject.Mass;

            // grab for rotation
            character.objectCamera.HandGrabJoint.NodeB = grabbedObject.GetPath();

            // nastavime nova fyzikalni data pro grab
            grabbedObject.Inertia = character.RBPhysicInGrab_Inertia;
            grabbedObject.AngularDamp = character.RBPhysicInGrab_AngularDamp;
            grabbedObject.LinearDamp = character.RBPhysicInGrab_LinearDamp;
            grabbedObject.PhysicsMaterialOverride.Friction = 0.15f;
            grabbedObject.PhysicsMaterialOverride.Bounce = 0.0f;
            grabbedObject.Mass = 1.0f;
        }
        else
        {
            // pri opusteni grab, nahrajeme objektu puvodni data
            grabbedObject.Inertia = lastGrabbedItemOriginalParams.inertia;
            grabbedObject.AngularDamp = lastGrabbedItemOriginalParams.angularDamp;
            grabbedObject.LinearDamp = lastGrabbedItemOriginalParams.linearDamp;
            grabbedObject.PhysicsMaterialOverride.Friction = lastGrabbedItemOriginalParams.friction;
            grabbedObject.PhysicsMaterialOverride.Bounce = lastGrabbedItemOriginalParams.bounce;
            grabbedObject.Mass = lastGrabbedItemOriginalParams.mass;
        }
    }
}
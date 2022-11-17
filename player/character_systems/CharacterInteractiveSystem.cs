using Godot;
using System;
using System.Data;
using System.Linq.Expressions;

public partial class CharacterInteractiveSystem : Godot.Object
{
    FPSCharacter_Interaction character = null;
    BasicHud basicHud;
    interactive_object actualInteractiveObject = null;
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

    public void BasicUpdate(bool newUseNow,bool newGrabNow, double delta)
    {
        // default sets on start this update
        basicHud.SetUseVisible(false);
        basicHud.SetHandGrabState(false, false);
        basicHud.SetUseLabelText("");
        basicHud.SetUseVisible(false);

        // neexistuje zadny interactive objekt ? = opustit update
        if (actualInteractiveObject == null)
            return;

        // Neni pozadovano, nebo je pozadovano aby byl hrac v area objektu a opravdu v nem je ?
        if (character.MustBeInInteractiveArea == false ||
            (character.MustBeInInteractiveArea && actualInteractiveObject.GetIsPlayerInRange()))
        {
            switch (actualInteractiveObject.InteractiveLevel)
            {
                case interactive_object.EInteractiveLevel.OnlyUse:
                    {
                        UpdateForUse(actualInteractiveObject, newUseNow, delta);
                        break;
                    }
                case interactive_object.EInteractiveLevel.OnlyPhysic:
                    {
                        UpdateForPhysic(actualInteractiveObject, newUseNow, newGrabNow, delta);
                        break;
                    }
                case interactive_object.EInteractiveLevel.UseAndPhysic:
                    {
                        UpdateForUse(actualInteractiveObject, newUseNow, delta);
                        UpdateForPhysic(actualInteractiveObject, newUseNow, newGrabNow, delta);
                        break;
                    }
            }
        }
    }

    public void SetActualInteractiveObject(interactive_object newInteractiveObject, Vector3 hitPosition)
    {
        actualInteractiveObject = newInteractiveObject;
    }

    public void UpdateForUse(interactive_object newInteractiveObject,bool newUseNow,double delta)
    {
        //  Pokud mame pouzivame objekt pro USE - zobrazime text pro jeho akci
        if (newInteractiveObject.InteractiveLevel == interactive_object.EInteractiveLevel.OnlyUse ||
            newInteractiveObject.InteractiveLevel == interactive_object.EInteractiveLevel.UseAndPhysic)
        {
            basicHud.SetUseLabelText(newInteractiveObject.GetUseActionName());
            basicHud.SetUseVisible(true);

            // Pokud je momentalne od hrace input zadost pro USE, pouzijeme vnitrni funkci objektu pro USE
            if (newUseNow)
                newInteractiveObject.Use(character);
        }
    }
    public void UpdateForPhysic(interactive_object newInteractiveObject, bool newUseNow,bool newGrabNow, double delta)
    {
        // Je objekt oprabdu zalozeny pro physic update ? - UPDATE GRAB
        if(newInteractiveObject.InteractiveLevel == interactive_object.EInteractiveLevel.OnlyPhysic ||
            newInteractiveObject.InteractiveLevel == interactive_object.EInteractiveLevel.UseAndPhysic)
        {
            if(newGrabNow && isCanNewGrab)
                StartGrabbing((RigidBody3D)actualInteractiveObject.GetParent());
        }
    }

    public void InteractivePhysicsUpdate(bool newGrabNow, bool newThrowObjectNow, bool newRotateGrabbedObject,
        bool newMoveFarGrabbedObject, bool newMoveNearGrabbedObject, double delta)
    {
        // default sets on start this update
        wantRotateNow = false;
        character.objectCamera.IsCameraLookInputEnable = true;

        // neexistuje zadny interactive objekt ? = opustit update
        if (actualInteractiveObject == null)
            return;

        // Filter update podle typu physics interakce
        switch (actualInteractiveObject.InteractivePhysicType)
        {
            case interactive_object.EInteractivePhysicType.GrabItem:
                {
                    UpdatePhysic_GrabItem(newGrabNow, newThrowObjectNow, newRotateGrabbedObject,
                        newMoveFarGrabbedObject, newMoveNearGrabbedObject, delta);
                    break;
                }
            case interactive_object.EInteractivePhysicType.GrabJoint:
                {
                    UpdatePhysic_GrabJoint(newGrabNow, newThrowObjectNow, newRotateGrabbedObject,
                        newMoveFarGrabbedObject, newMoveNearGrabbedObject, delta);
                    break;
                }
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
            grabbedObject.PhysicsMaterialOverride.Friction = character.RBPhysicInGrab_Friction;
            grabbedObject.PhysicsMaterialOverride.Bounce = character.RBPhysicInGrab_Bounce;
            grabbedObject.Mass = character.RBPhysicInGrab_Mass;
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

    public void UpdatePhysic_GrabItem(bool newGrabNow, bool newThrowObjectNow, bool newRotateGrabbedObject,
        bool newMoveFarGrabbedObject, bool newMoveNearGrabbedObject, double delta)
    {
        // otestujeme zda je tento object nastaveny pro grab a zda aktualne nejaky objekt negrabujeme
        // pokud jsou podminky splneny, nastavime viditelnou normal hand

        if (actualInteractiveObject.InteractiveLevel == interactive_object.EInteractiveLevel.OnlyPhysic ||
            actualInteractiveObject.InteractiveLevel == interactive_object.EInteractiveLevel.UseAndPhysic)
        {
            // Neni pozadovano, nebo je pozadovano aby byl hrac v area objektu a opravdu v nem je ?
            if (character.MustBeInInteractiveArea == false ||
                (character.MustBeInInteractiveArea && actualInteractiveObject.GetIsPlayerInRange()))
            {
                if (newGrabNow == false)
                    basicHud.SetHandGrabState(true, false);
            }
            // Je pozadovano a hrac neni v aree ? *** prozatimni reseni na upusteni objektu v dalce !!! ***
            else if (character.MustBeInInteractiveArea && !actualInteractiveObject.GetIsPlayerInRange() && newGrabNow)
            {
                if (pickedBody != null)
                {
                    isCanNewGrab = false;
                    StopGrabbing();
                }

                pickedBody = null;
            }
        }

        // pokud jsme aktualne ve stavu grabbing, stale prichazi input pro grab tak
        // updatujeme pozice objektu pro grabbing
        if (isCanNewGrab && isGrabbing && newGrabNow && pickedBody != null)
        {
            basicHud.SetHandGrabState(true, true);
            // grabbing
            Vector3 pickedBodyGlobalPositon = pickedBody.GlobalTransform.origin;
            Vector3 HandGrabGlobalPosition = character.objectCamera.HandGrabMarker.GlobalPosition;

            pickedBody.LinearVelocity = (HandGrabGlobalPosition - pickedBodyGlobalPositon) * character.GrabObjectPullPower;
        }
        else if (isGrabbing && newGrabNow == false)
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
        if (isGrabbing && newGrabNow && pickedBody != null && newRotateGrabbedObject)
        {
            wantRotateNow = true;
        }

        // Move Far/Near Grabbed Object
        if (isGrabbing && newGrabNow && pickedBody != null && (newMoveFarGrabbedObject || newMoveNearGrabbedObject))
        {
            Vector3 actualPosition = character.objectCamera.GetHandGrabMarker().Position;

            if (newMoveFarGrabbedObject)
                actualPosition.z -= character.MoveFarOrNearObjectStep;

            if (newMoveNearGrabbedObject)
                actualPosition.z += character.MoveFarOrNearObjectStep;

            actualPosition.z = Mathf.Clamp(actualPosition.z,
                -character.MoveFarOrNearObjectMax, -character.MoveFarOrNearObjectMin);

            character.objectCamera.GetHandGrabMarker().Position = actualPosition;
        }
    }

    public void UpdatePhysic_GrabJoint(bool newGrabNow, bool newThrowObjectNow, bool newRotateGrabbedObject,
        bool newMoveFarGrabbedObject, bool newMoveNearGrabbedObject, double delta)
    {

    }
}

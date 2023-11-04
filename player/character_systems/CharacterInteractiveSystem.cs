using Godot;
using System;
using System.Data;
using System.Linq.Expressions;

public partial class CharacterInteractiveSystem : Godot.GodotObject
{
    FPSCharacter_Interaction character = null;
    BasicHud basicHud;
    interactive_object actualInteractiveObject = null;
    RigidBody3D pickedBody = null;
    bool isGrabbing = false;
    bool isCanNewGrab = true;
    bool wantRotateNow = false;

    bool canUse = true;
    //only for GrabActions
    bool isFirstGrabAction = true;
    Vector2 originalHandGrabbedTex = Vector2.Zero;
    Vector3 hit_offset = Vector3.Zero;

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
        if (character.objectCamera == null) return;

        // default sets on start this update
        basicHud.SetUseVisible(false);
        basicHud.SetHandGrabState(false, false);
        basicHud.SetUseLabelText("");
        basicHud.SetUseVisible(false);
        basicHud.SetHandClickVisible(false);

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
        // pokud nemame v grabu nejaky item, prepiseme interacatObject novym, ktery detekujeme jako prichozi
        if(pickedBody == null)
        {
            actualInteractiveObject = newInteractiveObject;

            if(actualInteractiveObject != null)
                hit_offset = actualInteractiveObject.GetInteractCenterGlobalPosition() - hitPosition;
        }
    }

    public void UpdateForUse(interactive_object newInteractiveObject,bool newUseNow,double delta)
    {
        //  Pokud mame pouzivame objekt pro USE - zobrazime text pro jeho akci
        if (newInteractiveObject.InteractiveLevel == interactive_object.EInteractiveLevel.OnlyUse ||
            newInteractiveObject.InteractiveLevel == interactive_object.EInteractiveLevel.UseAndPhysic)
        {
            switch (newInteractiveObject.InteractVisibleBy)
            {
                case interactive_object.EUseInteractVisibleBy.Text:
                    {
                        basicHud.SetUseLabelText(newInteractiveObject.GetUseActionName());
                        basicHud.SetUseVisible(true);
                        break;
                    }
                case interactive_object.EUseInteractVisibleBy.HandClick:
                    {
                        basicHud.SetHandClickVisible(true);
                        break;
                    }
                case interactive_object.EUseInteractVisibleBy.HandClickAndText:
                    {
                        basicHud.SetUseLabelText(newInteractiveObject.GetUseActionName());
                        basicHud.SetUseVisible(true);
                        basicHud.SetHandClickVisible(true);
                        break;
                    }
                default:
                    break;
            }

            // Pokud je momentalne od hrace input zadost pro USE, pouzijeme vnitrni funkci objektu pro USE
            if (newUseNow && canUse)
            {
                if(newInteractiveObject != null && pickedBody != null)
                {
                    StopGrabbing();
                    newInteractiveObject.Use(character);
                    pickedBody = null;
                    newInteractiveObject = null;
                }
            }
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
        character.objectCamera.SetCameraLookInputEnable(true);

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
            case interactive_object.EInteractivePhysicType.GrabAction:
                {
                    UpdatePhysic_GrabAction(newGrabNow, delta);
                    break;
                }
        }

        // Rotate Grabbed Object via gamepad
        if (pickedBody != null && wantRotateNow)
        {
            character.objectCamera.SetCameraLookInputEnable(false);

            /* Nacte hodnoty (axis right) gamepadu */
            Vector2 JoyLook = new Vector2(Input.GetActionStrength("RightStick_Right") - Input.GetActionStrength("RightStick_Left"),
            -(Input.GetActionStrength("RightStick_Up") - Input.GetActionStrength("RightStick_Down")));

            /* Pokud JoyLook ma nejakou hodnotu (pohnuto packou na gamepadu) = gamepad jinak mys */
            if (JoyLook.Length() > 0 && character.IsInputEnable())
            {
                character.objectCamera.HandStaticBody.RotateX(
                Mathf.DegToRad(JoyLook.Y * character.RotateObjectStep));

                character.objectCamera.HandStaticBody.RotateY(
                Mathf.DegToRad(JoyLook.X * character.RotateObjectStep));
            }
        }

        // Reset pro novy grab, napriklad po hodu, donuti hrace pustit tlacitko pro grab, i kdyby na jeden frame
        // pokud ho pusti, resetujeme moznost znovu grabbovat
        if (isCanNewGrab == false && newGrabNow == false)
            isCanNewGrab = true;
    }

    // Update prichazejici z interaction character
    public void UpdateGrabbedItemRotate(InputEvent @event)
    {
        // Rotate Grabbed Object
        if (pickedBody != null && wantRotateNow)
        {
            character.objectCamera.SetCameraLookInputEnable(false);

            if (@event is InputEventMouseMotion && character.IsInputEnable())
            {
                InputEventMouseMotion mouseEventMotion = @event as InputEventMouseMotion;

                character.objectCamera.HandStaticBody.RotateX(
                    Mathf.DegToRad(mouseEventMotion.Relative.Y * character.RotateObjectStep));

                character.objectCamera.HandStaticBody.RotateY(
                    Mathf.DegToRad(mouseEventMotion.Relative.X * character.RotateObjectStep));
            }
        }
    }

    public void StartGrabbing(RigidBody3D grabbedObject)
    {
        isGrabbing = true;

        // chceme zacit grabovat s novym objektem ?
        if(pickedBody == null && grabbedObject != null)
        {
            //nove
            float distance = grabbedObject.GlobalPosition.DistanceTo(character.GetObjectCamera().GlobalPosition);

            // Set to original handGrabPosition
            Vector3 actualPosition = character.objectCamera.GetHandGrabMarker().Position;
            //actualPosition.Z = -character.MoveFarOrNearObjectOriginal;
            actualPosition.Z = - distance;
            character.objectCamera.GetHandGrabMarker().Position = actualPosition;

            // Nastavi nas novy Rigidbody object a nastavi mu pozadovane fyzikalni parametry pro grab
            pickedBody = grabbedObject;
            SetRigidBodyParamForGrab(pickedBody, true);
        }
    }
    public void StopGrabbing()
    {
        basicHud.SetHandGrabState(false, false);
        basicHud.SetCrosshairVisible(true);

        SetRigidBodyParamForGrab(pickedBody, false);
        isGrabbing = false;
        character.objectCamera.HandGrabJoint.NodeB = character.objectCamera.HandGrabJoint.GetPath();

        // Set to original handGrabPosition
        Vector3 actualPosition = character.objectCamera.GetHandGrabMarker().Position;
        actualPosition.Z = -character.MoveFarOrNearObjectOriginal;
        character.objectCamera.GetHandGrabMarker().Position = actualPosition;
    }

    public void SetRigidBodyParamForGrab(RigidBody3D grabbedObject, bool newGrab)
    {
        if (grabbedObject == null) return;

        if (newGrab)
        {
            // ulozime si originalni fyzikalni data
            lastGrabbedItemOriginalParams.inertia = grabbedObject.Inertia;
            lastGrabbedItemOriginalParams.angularDamp = grabbedObject.AngularDamp;
            lastGrabbedItemOriginalParams.linearDamp = grabbedObject.LinearDamp;
            lastGrabbedItemOriginalParams.friction = grabbedObject.PhysicsMaterialOverride.Friction;
            lastGrabbedItemOriginalParams.bounce = grabbedObject.PhysicsMaterialOverride.Bounce;
            lastGrabbedItemOriginalParams.mass = grabbedObject.Mass;

            // podle physic type
            switch (actualInteractiveObject.InteractivePhysicType)
            {
                case interactive_object.EInteractivePhysicType.GrabItem:
                    {
                        // grab for rotation
                        character.objectCamera.HandGrabJoint.NodeB = grabbedObject.GetPath();

                        // nastavime nova fyzikalni data pro grab
                        grabbedObject.Inertia = character.RBPhysicInGrab_Inertia;
                        grabbedObject.AngularDamp = character.RBPhysicInGrab_AngularDamp;
                        grabbedObject.LinearDamp = character.RBPhysicInGrab_LinearDamp;
                        grabbedObject.PhysicsMaterialOverride.Friction = character.RBPhysicInGrab_Friction;
                        grabbedObject.PhysicsMaterialOverride.Bounce = character.RBPhysicInGrab_Bounce;
                        grabbedObject.Mass = character.RBPhysicInGrab_Mass;
                        break;
                    }
                case interactive_object.EInteractivePhysicType.GrabAction:
                    {
                        break;
                    }
            }
        }
        else
        {

                // pri opusteni grab, nahrajeme objektu puvodni data
                grabbedObject.Inertia = lastGrabbedItemOriginalParams.inertia;
                grabbedObject.AngularDamp = lastGrabbedItemOriginalParams.angularDamp;
                grabbedObject.LinearDamp = lastGrabbedItemOriginalParams.linearDamp;
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
            Vector3 pickedBodyGlobalPositon = pickedBody.GlobalTransform.Origin;
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
                GlobalTransform.Basis.Z.Normalized() * -character.ThrowObjectPower);

            isCanNewGrab = false;
            pickedBody = null;
        }

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
                actualPosition.Z -= character.MoveFarOrNearObjectStep;

            if (newMoveNearGrabbedObject)
                actualPosition.Z += character.MoveFarOrNearObjectStep;

            actualPosition.Z = Mathf.Clamp(actualPosition.Z,
                -character.MoveFarOrNearObjectMax, -character.MoveFarOrNearObjectMin);

            character.objectCamera.GetHandGrabMarker().Position = actualPosition;
        }
    }

    public void UpdatePhysic_GrabAction(bool newGrabNow, double delta)
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

                    // povolime pristi do once funkci firstGrabAction
                    isFirstGrabAction = true;
                    actualInteractiveObject.GrabActionEnd(character);
                }
                pickedBody = null;
            }
        }

        // pokud jsme aktualne ve stavu grabbing, stale prichazi input pro grab tak
        // spustime funkci pro start grab action
        if (isCanNewGrab && isGrabbing && newGrabNow && pickedBody != null)
        {
            // Do once - 1 update pri spusteni grabu jointu
            if (isFirstGrabAction)
            {
                isFirstGrabAction = false;
                actualInteractiveObject.GrabActionStart(character);

                // BASIC HUD - ulozime si originalni pozici hand grabbed tex
                originalHandGrabbedTex = basicHud.GetHandGrabbedTextureRect().Position;
                basicHud.SetCrosshairVisible(false);
            }

            character.objectCamera.SetCameraLookInputEnable(false);
            basicHud.SetHandGrabState(true, true);

            // BASIC HUD - UPDATE POZICE HANDGRABEDTEX
            TextureRect handGrabbedTex = basicHud.GetHandGrabbedTextureRect();
            Vector2 pos = Vector2.Zero;
            if (actualInteractiveObject.UseOffsetHitInteract == false)
            {
                pos = character.objectCamera.Camera.UnprojectPosition(
                    actualInteractiveObject.GetInteractCenterGlobalPosition());
            }
            else
            { 
                pos = character.objectCamera.Camera.UnprojectPosition(
                    actualInteractiveObject.GetInteractCenterGlobalPosition() - hit_offset);
            }
            handGrabbedTex.Position = pos;
        }
        else if (isGrabbing && newGrabNow == false)
        {
            // BASIC HUD - vratime originalni pozici pro hand grabbed tex
            basicHud.GetHandGrabbedTextureRect().Position = originalHandGrabbedTex;
            basicHud.SetCrosshairVisible(true);

            // zastavime grab
            StopGrabbing();
            pickedBody = null;

            // povolime pristi do once funkci firstGrabAction
            isFirstGrabAction = true;
            actualInteractiveObject.GrabActionEnd(character);
        }
    }

    public void ResetNeedGrabbing()
    {
        canUse = true;
    }
}

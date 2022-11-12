using Godot;
using System;
using System.Data;

// propojit s basichud
// zprovoznit puvodni hud texty
// zprovoznit use a grab

public partial class CharacterInteractiveSystem : Godot.Object
{
    FPSCharacter_Interaction character = null;
    BasicHud basicHud;
    Node actualInteractiveObject = null;

    public CharacterInteractiveSystem(FPSCharacter_Interaction newCharacterOwner, BasicHud newBasichud)
    {
        character = newCharacterOwner;
        basicHud = newBasichud;
    }

    public void Update(bool newUseNow,bool newGrabNow, double delta)
    {
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

    public void SetActualInteractiveObject(Node newInteractiveObject)
    {
        // pokud je jiny novy objekt ? prepisme ho, jinak pouzivame aktualni
        if(actualInteractiveObject != newInteractiveObject)
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
            newInteractiveObject.ApplyGrab(newGrabNow,character);
        }
    }
}

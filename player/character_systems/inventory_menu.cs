using Godot;
using Godot.Collections;
using System;
using System.Net.Sockets;

public partial class inventory_menu : Control
{
    private InventorySystem inventorySystem = null;

    public enum EActiveTypeEffect { instant, anim }
    private bool _active = false;
    private bool active_nextFrame = false;

    private AnimationPlayer anim = null;
    private AudioStreamPlayer audio = null;

    [Export] public EActiveTypeEffect ActiveTypeEffect = EActiveTypeEffect.anim;
    [Export] public Array<AudioStream> InventoryOpenAudios;
    [Export] public Array<AudioStream> InventoryCloseAudios;

    // inventory slots
    private Array<InventorySlot> allInventorySlots;

    private InventoryItemPreview itemPreview = null;
    private int actualFocusSlotID = -1;

    public override void _Ready()
    {
        anim = GetNode<AnimationPlayer>("AnimationPlayer");
        audio = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        itemPreview = GetNode<InventoryItemPreview>("Panel/Panel_ItemPreview/SubViewportContainer");

        SetActiveInstant(false);

        allInventorySlots = new Array<InventorySlot>();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (!GetActive()) return;

        // dovoli tento update az dalsi frame (kvuli inputu)
        if (!active_nextFrame) { active_nextFrame = true; return; }

        // close this inventory
        if (Input.IsActionJustPressed("toggleInventory"))
            SetActive(false);
    }

    public void Init(InventorySystem newInventorySystem) 
    {
        inventorySystem = newInventorySystem;

        RecreateAllSlotsWithItems();
    }

    public void SetActive(bool newActive)
    {
        switch (ActiveTypeEffect)
        {
            case EActiveTypeEffect.instant:
                SetActiveInstant(newActive);
                break;
            case EActiveTypeEffect.anim:
                SetActiveAnim(newActive);
                break;
            default:
                break;
        }
    }

    public void SetActiveInstant(bool newActive)
    {
        _active = newActive;
        Visible = newActive;

        // ziskame interact charactera
        FPSCharacter_Interaction interChar = (FPSCharacter_Interaction)GameMaster.GM.GetFPSCharacter();
        if (interChar == null) return;

        // ostatni akce pri zmene
        if (_active)
        {
            // vyresetuje lean a zoom hrace
            interChar.GetObjectCamera().SetActualLean(ObjectCamera.ELeanType.Center);
            interChar.SetCameraZoom(false);

            // zakaze char_inputs a zobrazi mys
            interChar.SetInputEnable(false);
            interChar.SetMouseVisible(true);
        }
        else
        {
            // povoli char_inputs + captured mouse (uvnitr funkce SetInputEnable)
            interChar.SetInputEnable(true);
            active_nextFrame = false;
        }
    }

    public void SetActiveAnim(bool newActive)
    {
        _active = newActive;

        // ziskame interact charactera
        FPSCharacter_Interaction interChar = (FPSCharacter_Interaction)GameMaster.GM.GetFPSCharacter();
        if (interChar == null) return;

        // ostatni akce pri zmene
        if (_active)
        {
            Visible = newActive;
            // vyresetuje lean a zoom hrace
            interChar.GetObjectCamera().SetActualLean(ObjectCamera.ELeanType.Center);
            interChar.SetCameraZoom(false);

            // zakaze char_inputs a zobrazi mys
            interChar.SetInputEnable(false);
            interChar.SetMouseVisible(true);

            // audio
            UniversalFunctions.PlayRandomSound(audio, InventoryOpenAudios, 0, 1);

            // anim
            anim.Play("open_inventory");

            // camera chake
            interChar.GetObjectCamera().ShakeCameraTest(0.3f, 0.35f, 1.0f, 2.0f);

            // try update items (add)
            //AddAllItemsToSlots();
        }
        else
        {
            // povoli char_inputs + captured mouse (uvnitr funkce SetInputEnable)
            interChar.SetInputEnable(true);
            active_nextFrame = false;

            // audio
            UniversalFunctions.PlayRandomSound(audio, InventoryCloseAudios, 0, 1);

            // anim
            anim.PlayBackwards("open_inventory");

            // camera shake
            interChar.GetObjectCamera().ShakeCameraTest(0.3f, 0.35f, 1.0f, 2.0f);
        }
    }

    public bool GetActive() { return _active; }

    public void _on_animation_player_animation_finished(string animName)
    {
        if (!_active)
        {
            Visible = false;
            GD.Print("anim dohrala");
            
            /*// try update items destroy
            DestroyAllUIItemsInSlots();*/
        }
    }

    public void SetTabsToCenter()
    {
        TabContainer tabs = GetNode<TabContainer>("TabContainer") as TabContainer;
        tabs.SetAnchorsPreset(LayoutPreset.Center);
    }

    /* ITEMS */

    public InventorySystem GetInventorySystem() { return inventorySystem; }

    public void RecreateAllSlotsWithItems()
    {
        // vytvori sloty a nacte do array allInventorySlots
        CreateSlots(inventorySystem.MaxInventoryCapacity);

        // nacte vsechny itemy ktere hrac vlastni do ui inventory(do jednotlivych slotu)
        AddAllItemsToSlots();
    }

    //  START
    public void AddAllItemsToSlots()
    {
        // projede postupne kazdy item u hrace a ziska jeho pripsany slotID (ktery dostal pri uspesnem pridanim)
        // ziskame konkretni slot kteremu nastavime nas item (jeho inventory data)
        for (int i = 0; i < GetInventorySystem().GetAllInventoryItems().Count; i++)
        {
            AddItemToSlot(GetInventorySystem().GetAllInventoryItems()[i],
                GetInventorySystem().GetAllInventoryItems()[i].InventoryHoldingSlotID);
        }
    }

    public void AddItemToSlot(InventoryItemData newItemData, int slotID)
    {
        GetAllInventoryItemSlots()[slotID].SetItem(newItemData);
    }

    public Array<InventorySlot> GetAllInventoryItemSlots() { return allInventorySlots; }

    // END
    public void DestroyAllUIItemsInSlots()
    {
        foreach (InventorySlot slot in GetAllInventoryItemSlots())
        {
            if (slot.HasUIItem())
                slot.DestroyUIItem();
        }
    }

    public void FocusUIItem(InventorySlot pressedInventorySlot)
    {
        if (pressedInventorySlot.HasUIItem())
        {
            GetNode<Label>("Panel/Panel/Label").Text = pressedInventorySlot.GetInventoryItemData().itemName;

            GetNode<RichTextLabel>("Panel/Panel/RichTextLabel").Text =
                pressedInventorySlot.GetInventoryItemData().itemInfoText;

            testing_render_inventory_items.ApplyItemSubViewportSetting(
            GetNode<SubViewport>("Panel/Panel_ItemPreview/SubViewportContainer/SubViewport"),
            pressedInventorySlot.GetInventoryItemData().SettingsForPreview,
            pressedInventorySlot.GetInventoryItemData().itemMeshPreview);

            itemPreview.Activate(true);
            actualFocusSlotID = pressedInventorySlot.GetID();
        }
        else
        {
            GetNode<Label>("Panel/Panel/Label").Text = "";
            GetNode<RichTextLabel>("Panel/Panel/RichTextLabel").Text = "";
            itemPreview.Deactivate();
        }
    }

    public void DisableLastFocusUIItem()
    {
        GetNode<Label>("Panel/Panel/Label").Text = "";
        GetNode<RichTextLabel>("Panel/Panel/RichTextLabel").Text = "";

        itemPreview.Deactivate();
    }

    public void PutFromInventory(InventorySlot pressedInventorySlot)
    {
        if (!pressedInventorySlot.HasUIItem()) return;

        // Call Put to world
        inventorySystem.PutItemFromInventoryToWorld(pressedInventorySlot.GetInventoryItemData());

        // pokud nas aktualni focusovany slot je stejny jako ten na kterem je item ktery chceme vyhodt
        // prerusime jeho focus
        if(pressedInventorySlot.GetID() == actualFocusSlotID)
            DisableLastFocusUIItem();

        // Destroy ui
        pressedInventorySlot.DestroyUIItem();
    }

    public int GetFirstFreeSlot()
    {
        // projde cele pole iventory slots a zjisti prvni volny
        for (int i = 0; i < GetAllInventoryItemSlots().Count; i++)
        {
            if (!GetAllInventoryItemSlots()[i].HasUIItem())
                return i;
        }

        // pokud neni zadny volny slot nalezen, vracime chybovy kod -1
        return -1;
    }

    private void CreateSlots(int newNumber, int newNumberHasNumberText = 5)
    {
        // Nacte prefab sceny slotu
        PackedScene newSlotPackedScene =
            GD.Load<PackedScene>("res://player/character_systems/inventory_menu/InventorySlot.tscn");

        // vytvori potrebny pocet instanci
        for (int i = 0; i < newNumber; i++)
        {
            InventorySlot newSlot = newSlotPackedScene.Instantiate() as InventorySlot;
            GetNode("Panel/GridContainer").AddChild(newSlot);

            // nastavi text cisla
            if (i < (newNumberHasNumberText))
            {
                int a = i + 1;
                newSlot.SetNameSlotText(a.ToString());
                newSlot.SetShowNameSlot(true);
            }

            newSlot.Init(this);
            newSlot.SetID(i);
            allInventorySlots.Add(newSlot);
        }
    }
}
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

    // drag and drop
    private float needClickTimeForDrag = 0.2f;
    private float mouseLeftClickTime = 0.0f;
    private int lastSlotClickedForTime = -1;
    DragAndDropItem dragAndDropItem = null;

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

        // Inputs update pro (drag and drop) a (Click)
        UpdateDragEndDropInputs("mouseClickLeft", delta);
    }

    private void UpdateDragEndDropInputs(StringName newDragAndDropAndFocusAction, double delta)
    {
        // DragAndDrop Start
        if (Input.IsActionPressed(newDragAndDropAndFocusAction) && GetActualDragAndDropItem() == null)
            foreach (var slot in GetAllSlots())
                if (slot.GetIsMouseHover() && slot.HasUIItem())
                {
                    // pokud slot se kterym chceme dragovat je typ attach, nechceme dragovat !
                    if (slot.inventorySlotType == InventorySlot.EInventorySlotType.socketAttach)
                        return;

                    // ochrana proti presouvani itemu ktery je jiz nekde attachnuty
                    if (slot.inventorySlotType == InventorySlot.EInventorySlotType.socketInventory &&
                        slot.GetIsAttachSlotEffectEnable()) return;

                    // podminky pro spusteni dragu
                    if (mouseLeftClickTime >= needClickTimeForDrag)
                    {
                        // Start Drag
                        StartDragAndDropItem(slot);

                        mouseLeftClickTime = 0.0f;
                        return;
                    }

                    // zajisteni aby se pricital cas jen na stejnem slotu, pokud neni = reset time
                    if (lastSlotClickedForTime == slot.GetID())
                        mouseLeftClickTime += (float)delta;
                    else
                        mouseLeftClickTime = 0.0f;

                    // pro zajisteni pocitani
                    lastSlotClickedForTime = slot.GetID();
                }

        // Released tlacitko Drag/Focus
        if (Input.IsActionJustReleased(newDragAndDropAndFocusAction))
        {
            if (GetActualDragAndDropItem() != null)
            {
                foreach (var slot in GetAllSlots())
                    if (slot.GetIsMouseHover())
                    {
                        // filtrujeme typ slotu a podle toho spustime funkce
                        switch (slot.inventorySlotType)
                        {
                            case InventorySlot.EInventorySlotType.socketInventory:
                                {
                                    // ochrana proti presouvani itemu ktery je jiz nekde attachnuty
                                    if (slot.GetIsAttachSlotEffectEnable()) return;

                                    DragItemChangeToNewSlot(slot);
                                    break;
                                }
                            case InventorySlot.EInventorySlotType.socketPlace:
                                DragItemChangeToNewSlot(slot);
                                break;
                            case InventorySlot.EInventorySlotType.socketAttach:
                                AttachItemAsHotkeyToSlot(slot);
                                InventorySlot a =
                                    GetSlotByID(dragAndDropItem.GetInventoryItemData().InventoryHoldingSlotID);
                                if (a != null)
                                    a.EnableAttachSlotEffect(true,slot.GetNameSlotText());

                                break;
                            default:
                                break;
                        }
 
                        // zrusi drag and drop
                        EndDragAndDropItem();

                        mouseLeftClickTime = 0.0f;
                        return;
                    }
            }
            else
            {
                foreach (var slot in GetAllSlots())
                    if (slot.GetIsMouseHover())
                    {
                        //focus item (click)
                        FocusUIItem(slot);

                        mouseLeftClickTime = 0.0f;
                        return;
                    }
            }
        }
    }
    
    public void Init(InventorySystem newInventorySystem)
    {
        inventorySystem = newInventorySystem;

        CreateAllSlotsWithItems();
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

    public void CreateAllSlotsWithItems()
    {
        // vytvori sloty pro inventar a nacte do array allInventorySlots
        CreateInventorySlots(inventorySystem.MaxInventoryCapacity);

        //
        InitAllSocketSlots(GetNode("Panel/AllSocketSlots"));

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
        GetAllSlots()[slotID].SetItem(newItemData);
    }

    // END
    public void DestroyAllUIItemsInSlots()
    {
        foreach (InventorySlot slot in GetAllSlots())
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

        // testovani pokud vyhazujeme item ktery je attachnut nekde ve slotu, musime napred znicit attach
        if (pressedInventorySlot.GetIsAttachSlotEffectEnable())
            ApplyDeteachItem(pressedInventorySlot);

        // Call Put to world
        inventorySystem.PutItemFromInventoryToWorld(pressedInventorySlot.GetInventoryItemData());

        // pokud nas aktualni focusovany slot je stejny jako ten na kterem je item ktery chceme vyhodt
        // prerusime jeho focus
        if(pressedInventorySlot.GetID() == actualFocusSlotID)
            DisableLastFocusUIItem();

        // Destroy ui
        pressedInventorySlot.DestroyUIItem();
    }

    public void ApplyDeteachItem(InventorySlot deteachSlot)
    {
            int press_id = deteachSlot.GetInventoryItemData().InventoryHoldingSlotID;
            foreach (var slot in GetAllSocketAttachSlotsOnly())
                if (slot.HasUIItem())
                    if (slot.GetInventoryItemData().InventoryHoldingSlotID == press_id)
                    {
                        slot.DestroyUIItem();
                        deteachSlot.EnableAttachSlotEffect(false, "");
                        break;
                    }
    }

    public int GetFirstFreeSlotInInventory()
    {
        // projde cele pole iventory slots a zjisti prvni volny
        for (int i = 0; i < GetAllInventorySlotsOnly().Count; i++)
        {
            if (!GetAllInventorySlotsOnly()[i].HasUIItem())
                return i;
        }

        // pokud neni zadny volny slot nalezen, vracime chybovy kod -1
        return -1;
    }

    private void CreateInventorySlots(int newNumber, int newNumberHasNumberText = 5)
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
            newSlot.inventorySlotType = InventorySlot.EInventorySlotType.socketInventory;
            newSlot.SetID(i);
            allInventorySlots.Add(newSlot);
        }
    }

    private void InitAllSocketSlots(Node parentOfAllSockets)
    {
        // prida do array nase sockety
        foreach (var socketNode in parentOfAllSockets.GetChildren())
        {
            InventorySlot socket = socketNode as InventorySlot;
            if (socket != null)
                if(socket.inventorySlotType == InventorySlot.EInventorySlotType.socketPlace ||
                    socket.inventorySlotType == InventorySlot.EInventorySlotType.socketAttach)
                {
                    GetAllSlots().Add(socket);
                    socket.Init(this);
                }
        }
    }

    public void StartDragAndDropItem(InventorySlot slot)
    {
        if (dragAndDropItem != null) return;

        GD.Print("StartDragAndDrop");
        // Nacte prefab sceny slotu
        PackedScene newDragAndDropPackedScene =
            GD.Load<PackedScene>("res://player/character_systems/inventory_menu/DragAndDropItem.tscn");

        dragAndDropItem = newDragAndDropPackedScene.Instantiate() as DragAndDropItem;
        AddChild(dragAndDropItem);
        dragAndDropItem.Init(slot.GetInventoryItemData());
        dragAndDropItem.SetDragUpdate(true);
    }

    public void EndDragAndDropItem()
    {
        GD.Print("EndDragAndDrop");
        dragAndDropItem.SetDragUpdate(false);
        dragAndDropItem.Destroy();
        dragAndDropItem = null;
    }

    public DragAndDropItem GetActualDragAndDropItem()
    {
        return dragAndDropItem;
    }

    public void DragItemChangeToNewSlot(InventorySlot newSlot)
    {
        if (dragAndDropItem == null || newSlot == null) return;

        if (newSlot.HasUIItem())
        {
            var holdSecond = newSlot.GetInventoryItemData();
            var holdFirst = dragAndDropItem.GetInventoryItemData();

            var a = holdFirst.InventoryHoldingSlotID;
            holdFirst.InventoryHoldingSlotID = holdSecond.InventoryHoldingSlotID;
            holdSecond.InventoryHoldingSlotID = a;

            GetSlotByID(holdFirst.InventoryHoldingSlotID).DestroyUIItem();
            GetSlotByID(holdSecond.InventoryHoldingSlotID).DestroyUIItem();

            GetSlotByID(holdFirst.InventoryHoldingSlotID).SetItem(holdFirst);
            GetSlotByID(holdSecond.InventoryHoldingSlotID).SetItem(holdSecond);
        }
        else
        {
            // vlozime do noveho slotu a stary znicime
            // pokud je potreba prerusime focus a znicime puvodni item ve slotu
            int id = dragAndDropItem.GetInventoryItemData().InventoryHoldingSlotID;

            if (GetSlotByID(id).GetID() == actualFocusSlotID)
                DisableLastFocusUIItem();

            dragAndDropItem.GetInventoryItemData().InventoryHoldingSlotID = newSlot.GetID();
            // pridame na nove misto
            newSlot.SetItem(dragAndDropItem.GetInventoryItemData());

            GetSlotByID(id).DestroyUIItem();
        }
    }

    public void AttachItemAsHotkeyToSlot(InventorySlot newSlot)
    {
        // pokud ma attachSlot item
        if(newSlot.HasUIItem())
        {
            GetSlotByID(newSlot.GetInventoryItemData().InventoryHoldingSlotID).
                EnableAttachSlotEffect(false, "");
        }

        CheckIfItemIsSomwhereAlreadyAttached(dragAndDropItem.GetInventoryItemData());

        GD.Print("test");
        newSlot.SetItem(dragAndDropItem.GetInventoryItemData());
    }

    private bool CheckIfItemIsSomwhereAlreadyAttached(InventoryItemData itemData)
    {
        foreach (InventorySlot attachSlot in GetAllSocketAttachSlotsOnly())
        {
            if (attachSlot.HasUIItem())
                if(attachSlot.GetInventoryItemData().InventoryHoldingSlotID == itemData.InventoryHoldingSlotID)
                {
                    attachSlot.DestroyUIItem();
                    return true;
                }
        }
        return false;
    }

    //
    public Array<InventorySlot> GetAllSlots()
    {
        return allInventorySlots;
    }

    public Array<InventorySlot> GetAllInventorySlotsOnly()
    {
        Array<InventorySlot> allInventorySlotsOnly= new Array<InventorySlot>();

        foreach (InventorySlot slot in GetAllSlots())
            if(slot.inventorySlotType == InventorySlot.EInventorySlotType.socketInventory)
                allInventorySlotsOnly.Add(slot);

        return allInventorySlotsOnly;
    }

    public Array<InventorySlot> GetAllSocketPlaceSlotsOnly()
    {
        Array<InventorySlot> allSocketSlotsOnly = new Array<InventorySlot>();

        foreach (InventorySlot slot in GetAllSlots())
            if (slot.inventorySlotType == InventorySlot.EInventorySlotType.socketPlace)
                allSocketSlotsOnly.Add(slot);

        return allSocketSlotsOnly;
    }

    public Array<InventorySlot> GetAllSocketAttachSlotsOnly()
    {
        Array<InventorySlot> allSocketAttachOnly = new Array<InventorySlot>();

        foreach (InventorySlot slot in GetAllSlots())
            if (slot.inventorySlotType == InventorySlot.EInventorySlotType.socketAttach)
                allSocketAttachOnly.Add(slot);

        return allSocketAttachOnly;
    }

    public InventorySlot GetSlotByID(int id)
    {
        foreach (var slot in GetAllSlots())
            if (slot.id == id)
                return slot;

        return null;
    }

    public Array<InventorySlot> GetAllAttachedSlots()
    {
        Array<InventorySlot> allSocketSlotsOnly = new Array<InventorySlot>();

        foreach (var slot in GetAllSlots())
            if(slot.inventorySlotType == InventorySlot.EInventorySlotType.socketInventory)
                if(slot.GetIsAttachSlotEffectEnable())
                    allSocketSlotsOnly.Add(slot);

        return allSocketSlotsOnly;
    }
}
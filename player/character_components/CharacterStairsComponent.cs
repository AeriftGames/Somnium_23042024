using Godot;
using System;

public partial class CharacterStairsComponent : Node
{
    [Export] public bool EnableStairsDetectEffect = true;

    private FPSCharacter_Inventory inventoryCharacter = null;

    public void StartInit(FPSCharacter_Inventory ownCharacter)
    {
        inventoryCharacter = ownCharacter;
    }
}

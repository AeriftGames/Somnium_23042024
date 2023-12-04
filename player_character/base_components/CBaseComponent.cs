using Godot;
using System;

public partial class CBaseComponent : Node
{
    protected FpsCharacterBase ourCharacterBase = null;

    public virtual void PostInit(FpsCharacterBase newCharacterBase)
    {
        ourCharacterBase = newCharacterBase;
    }
}

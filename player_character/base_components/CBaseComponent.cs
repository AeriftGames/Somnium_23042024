using Godot;
using System;

public partial class CBaseComponent : Node
{
    [Export] public bool EnableComponent = true;
    
    protected FpsCharacterBase ourCharacterBase = null;

    public virtual void PostInit(FpsCharacterBase newCharacterBase)
    {
        ourCharacterBase = newCharacterBase;
    }
}

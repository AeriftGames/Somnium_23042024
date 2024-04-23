using Godot;
using System;

public partial class CBaseAction : Resource
{
    public enum EActionInputEnum { X,F,G,H };

    [Export] public string ActionName = "Use";
    [Export] public string ActionFunction = "ActionUse";
    [Export] public EActionInputEnum ActionInputActivate = EActionInputEnum.F;
}

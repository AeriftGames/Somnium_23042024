using Godot;
using System;

public partial class CHandNode : Control
{
	public enum EHandType { Normal,Point,Grab,Off }
	private EHandType actualHandType = EHandType.Off;

	private AnimationPlayer animPlayer_HandNormal = null;
    private AnimationPlayer animPlayer_HandPoint = null;
    private AnimationPlayer animPlayer_HandGrab = null;

    public override void _Ready()
	{
		animPlayer_HandNormal = GetNode<AnimationPlayer>("%AnimationPlayer_HandNormal");
		animPlayer_HandPoint = GetNode<AnimationPlayer>("%AnimationPlayer_HandPoint");
        animPlayer_HandGrab = GetNode<AnimationPlayer>("%AnimationPlayer_HandGrab");
    }

	public void SetHandType(EHandType newHandType)
	{
		if (actualHandType == newHandType)
			return;

        if (actualHandType != newHandType)
            DisableHandType(actualHandType);

        switch (newHandType)
		{
			case EHandType.Normal:
				{
					animPlayer_HandNormal.Play("Show");
				}
				break;
			case EHandType.Point:
				{
                    animPlayer_HandPoint.Play("Show");
                }
				break;
			case EHandType.Grab:
				{
                    animPlayer_HandGrab.Play("Show");
                }
				break;
			case EHandType.Off:
				{
					DisableHandType(actualHandType);
                }
				break;
			default:
				break;
		}

        actualHandType = newHandType;
    }

	public void DisableHandType(EHandType newHandType)
	{
		switch (newHandType) 
		{
            case EHandType.Normal:
				animPlayer_HandNormal.PlayBackwards("Show");
                break;
            case EHandType.Point:
                animPlayer_HandPoint.PlayBackwards("Show");
                break;
            case EHandType.Grab:
                animPlayer_HandGrab.PlayBackwards("Show");
                break;
            case EHandType.Off:
                break;
            default:
                break;
        }
	}
}

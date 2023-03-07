using Godot;
using System;

public partial class table_lever_test : wall_lever_test
{
    public override void UpdateLever(double delta)
    {
        var newVel = GlobalTransform.Basis.Z.Normalized() * motionMouse.Y * mouseMotionSpeed;

        switch (grabMoveType)
        {
            case EGrabMoveType.Set:
                {
                    leverGrab.LinearVelocity = newVel;
                    break;
                }
            case EGrabMoveType.Add:
                {
                    if (mouseUpdated)
                        leverGrab.LinearVelocity += newVel;
                    break;
                }
        }

        // pokud je linear velocity vyysi nez pozadovany limit, nastavime hodnotu z limitu
        if (Mathf.Abs(leverGrab.LinearVelocity.Length()) > linearVelocityLimit)
            leverGrab.LinearVelocity = leverGrab.LinearVelocity.LimitLength(linearVelocityLimit);
        
        // vypocteme aktualni uhel a z nej vratime enum ktery rozhodne co se stane
        switch (CalculateReaches())
        {
            case EReachPointEnd.Top:
                {
                    if (onceIsReachPoint) return;

                    TestLight(true);
                    PlaySound(true);
                    //EmitSignal(SignalName.LeverReachEnd, true);
                    onceIsReachPoint = true;
                    break;
                }
            case EReachPointEnd.Bottom:
                {
                    if (onceIsReachPoint) return;

                    TestLight(false);
                    PlaySound(true);
                    //EmitSignal(SignalName.LeverReachEnd, false);
                    onceIsReachPoint = true;
                    break;
                }
            case EReachPointEnd.Work:
                {
                    onceIsReachPoint = false;
                    break;
                }
        }
    }
}

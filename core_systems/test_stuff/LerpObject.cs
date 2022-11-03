using Godot;
using System;

// prozatim jen na Vector3
public partial class LerpObject
{
    public class LerpVector3
    {
        /*
        // Testing our class LerpObject.LerpVector3
        LerpObject.LerpVector3 testLerp = new LerpObject.LerpVector3();
		testLerp.SetActual(GetFPSCharacterCamera().GlobalPosition);
		testLerp.SetTarget(new Vector3(0.0f,0.0f,0.0f));
		testLerp.SetSpeed(2.0f);
		testLerp.EnableUpdate(true);
		Vector3 testResult = testLerp.GetActual();
         */

        private Vector3 Actual = Vector3.Zero;
        private Vector3 Target = Vector3.Zero;
        private float Speed = 1.0f;

        private bool isEnableUpdate = false;

        public Vector3 Update(double delta)
        {
            if (isEnableUpdate)
            {
                Actual = Actual.Lerp(Target, Speed * (float)delta);
            }

            return Actual;
        }

        public void SetActual(Vector3 newActual){ Actual = newActual; }
        public Vector3 GetActual() { return Actual; }
        public void SetTarget(Vector3 newTarget) { Target = newTarget; }
        public Vector3 GetTarget() { return Target; }
        public void SetSpeed(float newSpeed) { Speed = newSpeed; }
        public float GetSpeed() { return Speed; }
        public void EnableUpdate(bool newEnable) { isEnableUpdate = newEnable; }
        public bool IsEnableUpdate() { return isEnableUpdate; }

        public void SetAllParam(Vector3 newActual, Vector3 newTarget, float newSpeed, bool newIsEnableUpdate)
        {
            Actual = newActual;
            Target = newTarget;
            Speed = newSpeed;
            isEnableUpdate = newIsEnableUpdate;
        }

        public float GetLengthToTarget()
        {
            return Actual.DistanceTo(Target);
        }
    }

    public class LerpFloat
    {

        private float Actual = 0.0f;
        private float Target = 0.0f;
        private float Speed = 1.0f;

        private bool isEnableUpdate = false;

        public float Update(double delta)
        {
            if (isEnableUpdate)
            {
                Actual = Mathf.Lerp(Actual, Target, Speed * (float)delta);
            }

            return Actual;
        }

        public void SetActual(float newActual) { Actual = newActual; }
        public float GetActual() { return Actual; }
        public void SetTarget(float newTarget) { Target = newTarget; }
        public float GetTarget() { return Target; }
        public void SetSpeed(float newSpeed) { Speed = newSpeed; }
        public float GetSpeed() { return Speed; }
        public void EnableUpdate(bool newEnable) { isEnableUpdate = newEnable; }
        public bool IsEnableUpdate() { return isEnableUpdate; }

        public void SetAllParam(float newActual, float newTarget, float newSpeed, bool newIsEnableUpdate)
        {
            Actual = newActual;
            Target = newTarget;
            Speed = newSpeed;
            isEnableUpdate = newIsEnableUpdate;
        }

        public float GetLengthToTarget()
        {
            return Mathf.Abs(GetTarget() - GetActual());
        }
    }

}

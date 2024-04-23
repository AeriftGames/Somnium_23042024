using Godot;
using System;

public class CSpring
{
    private float mass = 1.0f;
    private float value = 0.0f;
    private float k = 100.0f;
    private float velocity = 0.0f;
    private float gravity = 9.0f;
    private float damping = 10.0f;

    public float Value { get { return value - restPoint(); } }

    public float Velocity 
    { 
        get { return velocity; }
        set { velocity = value; }
    }

    private float restPoint()
    {
        return (mass * gravity);
    }

    public CSpring() { this.Reset(); }

    public void Reset()
    {
        this.value = restPoint();
        this.velocity = 0.0f;
    }

    public void Update(double delta)
    {
        var springForce = -k * value;
        var dampingForce = damping * velocity;
        var force = springForce + mass * gravity - dampingForce;
        var acceleration = force / mass;
        velocity += acceleration * (float) delta;
        value += velocity * (float) delta;
    }
}

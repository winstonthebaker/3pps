using Godot;
using System;

public partial class ControllerSM_GroundMovement : ControllerSM_Base
{
    [Export]
    private float _maxMoveSpeed;
    [Export]
    private float _snappiness = 0.5f;

    public override void ProcessState(double delta)
    {
        base.ProcessState(delta);
        Vector2 inputDirection = Input.GetVector(
            "move_left", "move_right",
            "move_forward", "move_backward"
            ).Normalized();


        Vector3 moveDirection = Context.Controller.Basis * new Vector3(inputDirection.X, 0, inputDirection.Y);
        Vector3 horizontalVelocity = Context.Controller.LinearVelocity with { Y = 0 };
        Vector3 targetVelocity = moveDirection * _maxMoveSpeed;
        GD.Print(targetVelocity.ToString());

        Vector3 difference = targetVelocity - horizontalVelocity;
        float differenceSqrMagnitude = difference.LengthSquared();
        Context.Controller.Mass = Context.Controller.BaseMass * (1f / Mathf.Max(1f, differenceSqrMagnitude * _snappiness));
        Context.Controller.ApplyCentralForce(difference.Normalized() * Mathf.Min(differenceSqrMagnitude, 1f) * Context.Controller.BaseMass);

    }
}

using Godot;
using System;

public partial class ControllerSM_Base : StateMachineNode<ControllerSMContext>
{
    public override void ProcessState(double delta)
    {
        if(Context.Controller == null)
        {
            GD.PrintErr($"Controller is null in {this}");
        }
        if (Context.GroundCheck == null)
        {
            GD.PrintErr($"GroundCheck is null in {this}");
        }
    }
}

public struct ControllerSMContext
{
    public PhysicsCharacterController Controller;
    public ShapeCast3D GroundCheck;
}
using Godot;
using System;
using Utilities;
public partial class PhysicsCharacterController : RigidBody3D
{
    [Export]
    public float BaseMass = 100f;


    private ControllerSM_Base _stateMachine;
    public override void _Ready()
    {
        _stateMachine = GetNode<ControllerSM_Base>("ControllerStateMachine");

        ControllerSMContext context = new ControllerSMContext
        {
            Controller = this,
            GroundCheck = GetNode<GroundEvaluator>("GroundCheck")
        };
        _stateMachine.SetContext(context);

        _stateMachine.Activate();
        //TODO: make the collider match the player collider and move it to appropriate height automatically
    }

    public override void _PhysicsProcess(double delta)
    {
        _stateMachine.ProcessStateRecursive(delta);

    }

    private bool _expectingJump = false;

    //public override void _IntegrateForces(PhysicsDirectBodyState3D state)
    //{
    //    if (!_hasInput)
    //    {
    //        // Directly damp velocity - no jitter, instant response
    //        Vector3 velocity = state.LinearVelocity;
    //        velocity.X *= _stopDamping;
    //        velocity.Z *= _stopDamping;
    //        if (_expectingJump)
    //        {
    //            velocity.Y = Mathf.Max(velocity.Y, _jumpVelocity);
    //            _expectingJump = false;
    //        }
    //        state.LinearVelocity = velocity;
    //    }
    //}
    
}
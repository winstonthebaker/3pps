using Godot;
using System;
using Utilities;

public partial class ControllerSM_EvaluateGround : ControllerSM_Base
{
    
    private const float COYOTE_TIME = 0.2f; //seconds
    private const float MAX_GROUND_ANGLE = 45f; //degrees

    private struct CollisionInfo
    {
        public Vector3 Point;
        public Vector3 Normal;
    }

    private CircularBuffer<Tuple<ulong, CollisionInfo>> _groundCollisionBuffer = new CircularBuffer<Tuple<ulong, CollisionInfo>>(10);
    
    private void GetGroundingInfo()
    {
        
            int collisionCount = Context.GroundCheck.GetCollisionCount();
            for (int i = 0; i < collisionCount; i++)
            {
                Vector3 point = Context.GroundCheck.GetCollisionPoint(i);
                Vector3 normal = Context.GroundCheck.GetCollisionNormal(i);

                CollisionInfo info = new CollisionInfo
                {
                    Point = point,
                    Normal = normal
                };

                _groundCollisionBuffer.Enqueue(new Tuple<ulong, CollisionInfo>(Time.GetTicksUsec(), info));
            }
            while (_groundCollisionBuffer.TryPeek(out var tuple))
            {
                ulong timeElapsed = Time.GetTicksUsec() - tuple.Item1;
                if (TunaMath.USecToSec(timeElapsed) > COYOTE_TIME)
                {
                    _groundCollisionBuffer.Dequeue();
                }
                else
                {
                    break;
                }
            }
        
    }

    private bool IsGrounded()
    {
        foreach (var tuple in _groundCollisionBuffer)
        {
            Vector3 normal = tuple.Item2.Normal;
            float angle = Mathf.RadToDeg(Mathf.Acos(normal.Dot(Vector3.Up)));
            if (angle <= MAX_GROUND_ANGLE)
            {
                return true;
            }
        }
        return false;
    }

    public override void ProcessState(double delta)
    {
        base.ProcessState(delta);
        GetGroundingInfo();
        if (IsGrounded())
        {
            Transition(_groundMovementState);
        }
        else
        {
            Transition(_airMovementState);
        }
    }

    public override void _Ready()
    {
        base._Ready();
        _groundMovementState = GetNode<ControllerSM_Base>(GroundMovementStatePath);
        if (_groundMovementState == null)
            GD.PrintErr($"Ground Movement State is null in {this}");
        _airMovementState = GetNode<ControllerSM_Base>(AirMovementStatePath);
        if (_airMovementState == null)
        {
            GD.PrintErr($"Air Movement State is null in {this}");
        }
    }

    private ControllerSM_Base _groundMovementState;
    private ControllerSM_Base _airMovementState;

    [Export]
    public NodePath GroundMovementStatePath;
    [Export]
    public NodePath AirMovementStatePath;
}

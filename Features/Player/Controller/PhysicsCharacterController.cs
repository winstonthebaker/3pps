using Godot;
using System;
using Utilities;
public partial class PhysicsCharacterController : RigidBody3D
{

    #region Context
    
    private ShapeCast3D _groundCheck;
    private RayCast3D _directionalCheck;
    public override void _Ready()
    {
        _groundCheck = GetNode<ShapeCast3D>("GroundCheck");
        _directionalCheck = GetNode<RayCast3D>("DirectionalCheck");
        
        _movementState = MovementState.Float;
        
        //TODO: make the collider match the player collider and move it to appropriate height automatically
    }
    #endregion

    #region StateMachine
    private MovementState _movementState;
    private MovementState _lastMovementState;

    private enum MovementState
    {
        Float = 0,
        Jump = 1
    }

    #endregion

    #region Loop
    public override void _PhysicsProcess(double delta)
    {
        switch (_movementState)
        {
            case MovementState.Float:
                FloatCollider(delta);
                break;
            case MovementState.Jump:
                JumpMovement(delta);
                break;
        }

    }

    #endregion


    #region Jump State

    private double _jumpTimer = 0.0;

    private void Jump()
    {
        const float jumpSpeed = 15.0f;
        _lastJumpPressTimer = 100.0;
        _groundedTimer = 100.0;
        _movementState = MovementState.Jump;
        _jumpTimer = 0.0;
        float yVel = LinearVelocity.Y;
        float diff = jumpSpeed - yVel;
        ApplyCentralImpulse(Mass * Vector3.Up * diff);
    }
    private void JumpMovement(double delta)
    {
        _jumpTimer += delta;

        if (_jumpTimer > 1.0)
        {
            _movementState = MovementState.Float;
            return;
        }
        if (LinearVelocity.Y <= 0.0f)
        {
            _movementState = MovementState.Float;
            return;
        }

        HorizontalMovement(delta, true, 3.0f);
        const float lowGravity = 20.0f;
        ApplyCentralForce(Vector3.Down * lowGravity * Mass);
    }

    #endregion

    #region FloatCollider

    private const float OrdinaryGravity = 20f;
    private const double CoyoteTime = 0.15; // 0.25 seconds
    private const double JumpBufferTime = 0.15; // 0.25 seconds

    private double _groundedTimer = 100.0;
    private double _lastJumpPressTimer = 100.0;


    void FloatCollider(double delta)
    {
        const float maxGroundAngle = 50f; //degrees
        const float halfOfColliderHeight = 0.75f;
        const float rideHeight = 1.0f;

        float bottomOfColliderY = Position.Y - halfOfColliderHeight;

        int numCollisions = _groundCheck.GetCollisionCount();
        bool onFlatGround = false;
        bool ableToJump = false;

        float highestGroundPoint = -Mathf.Inf;

        for (int i = 0; i < numCollisions; i++)
        {
            Vector3 colNormal = _groundCheck.GetCollisionNormal(i);
            Vector3 colPoint = _groundCheck.GetCollisionPoint(i);

            if (bottomOfColliderY - colPoint.Y > (rideHeight * 2.0f))
            {
                //Too far below this point to consider it ground
                continue;
            }
            float angle = Mathf.RadToDeg(Mathf.Acos(colNormal.Dot(Vector3.Up)));
            if (angle <= maxGroundAngle)
            {

                onFlatGround = true;
                if (colPoint.Y > highestGroundPoint)
                {
                    highestGroundPoint = colPoint.Y;
                }
            }

        }
        float yJumpOffPoint = highestGroundPoint;
        if (_directionalCheck.IsColliding())
        {
            Vector3 colNormal = _directionalCheck.GetCollisionNormal();
            Vector3 colPoint = _directionalCheck.GetCollisionPoint();
            if (bottomOfColliderY - colPoint.Y <= (rideHeight * 2.0f))
            {
                float angle = Mathf.RadToDeg(Mathf.Acos(colNormal.Dot(Vector3.Up)));
                if (angle <= maxGroundAngle)
                {
                    Vector3 dirColPoint = _directionalCheck.GetCollisionPoint();
                    if (dirColPoint.Y > highestGroundPoint)
                    {
                        yJumpOffPoint = dirColPoint.Y;
                    }
                }

            }
        }


        if (Input.IsActionJustPressed("jump"))
        {
            _lastJumpPressTimer = 0.0;
        }
        else
        {
            _lastJumpPressTimer += delta;
        }

        const float jumpAllowableHeight = 0.2f; //distance ABOVE ride height that we can still jump from

        if (onFlatGround)
        {
            if (bottomOfColliderY < Mathf.Max(highestGroundPoint, yJumpOffPoint) + rideHeight + jumpAllowableHeight)
            {
                ableToJump = true;
            }

            float deltaY = (highestGroundPoint + rideHeight) - bottomOfColliderY;

            float ySpeed = LinearVelocity.Y;
            ApplyCentralForce(Vector3.Up * DampedSpringForce(deltaY, ySpeed) * Mass);
            HorizontalMovement(delta, true, 8f);
            _groundedTimer = 0.0;

        }
        else
        {
            ApplyCentralForce(Vector3.Down * OrdinaryGravity * Mass);
            HorizontalMovement(delta, false, 1f);
            _groundedTimer += delta;
            if (_groundedTimer < CoyoteTime)
            {
                ableToJump = true;
            }
        }
        if (ableToJump && _lastJumpPressTimer < JumpBufferTime)
        {
            Jump();
        }


    }
    float DampedSpringForce(float deltaY, float speed)
    {
        const float k = 200.0f;
        const float damping = 10.0f;
        float springForce = k * deltaY;
        float dampingForce = -damping * speed;
        float totalForce = springForce + dampingForce;
        return totalForce;
    }

    void HorizontalMovement(double delta, bool zeroIsStop = false, float stiffness = 1.0f)
    {
        const float moveSpeed = 10f;
        Vector2 inputDir = new Vector2(Input.GetAxis("move_left", "move_right"), Input.GetAxis("move_forward", "move_backward")).Normalized();
        Vector3 horizontalMovement = Transform.Basis * new Vector3(inputDir.X, 0.0f, inputDir.Y);
        if (inputDir.LengthSquared() < 0.01f && !zeroIsStop)
        {
            return;
        }
        Vector3 desiredVel = horizontalMovement * moveSpeed;
        Vector3 planarVel = LinearVelocity;
        planarVel.Y = 0f;

        ApplyCentralForce((desiredVel - planarVel) * stiffness * Mass);
    }

    #endregion

}
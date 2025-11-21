using Godot;
using System;

public partial class CameraScroll : Node
{
    private Node3D _playerBody;

    [Export]
    private float _rotationAmount = 1f;
    public override void _Ready()
    {
        _playerBody = GetParent<Node3D>();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("camera_left"))
        {
            _playerBody.RotateObjectLocal(Vector3.Up, (MathF.PI / 8f) * _rotationAmount);
            
        }
        if(Input.IsActionJustPressed("camera_right"))
        {
            _playerBody.RotateObjectLocal(Vector3.Up, -(MathF.PI / 8f) * _rotationAmount);
            
        }

    }
}

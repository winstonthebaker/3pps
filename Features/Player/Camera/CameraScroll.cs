using Godot;
using System;
using PPS.Features.Interaction;

public partial class CameraScroll : Node
{
    private Node3D _playerBody;

    [Export] private float _rotationAmount = 1f;

    private Camera3D _cam;
    private RayCast3D _interactCheck;

    public override void _Ready()
    {
        _playerBody = GetParent<Node3D>();
        _cam = GetViewport().GetCamera3D();
        _interactCheck = GetNode<RayCast3D>("InteractCheck");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("camera_left"))
        {
            _playerBody.RotateObjectLocal(Vector3.Up, (MathF.PI / 8f) * _rotationAmount);
        }

        if (Input.IsActionJustPressed("camera_right"))
        {
            _playerBody.RotateObjectLocal(Vector3.Up, -(MathF.PI / 8f) * _rotationAmount);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("shoot"))
        {
            HandleShoot();
        }
    }

    private void HandleShoot()
    {
        Vector2 mousePos = GetViewport().GetMousePosition();
        Vector3 from = _cam.ProjectRayOrigin(mousePos);
        Vector3 to = from + _cam.ProjectRayNormal(mousePos) * 1000f;

        var query = PhysicsRayQueryParameters3D.Create(from, to);
        var result = GetViewport().World3D.DirectSpaceState.IntersectRay(query);

        if (result.Count == 0)
        {
        }
        else
        {
            Node3D hitNode = result["collider"].AsGodotObject() as Node3D;

            while (hitNode != null)
            {
                if (hitNode is IDoesInteract)
                {
                    IDoesInteract interactable = hitNode as IDoesInteract;
                    interactable.Interact();
                }

                hitNode = hitNode.GetParentNode3D();
            }
        }
    }
}
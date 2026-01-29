using Godot;

namespace PPS.Features.Interaction;

public partial class Interactable : Node3D, IDoesInteract
{
    private AnimationPlayer _animationPlayer;
    
    private bool _isOpen = false;
    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public void DoSomething()
    {
        GD.Print("Did Something");
    }
    public void Interact()
    {
        GD.Print($"{Name} Interact");
        _isOpen = !_isOpen;
        if (_isOpen)
        {
            _animationPlayer?.Play("Open");
        }
        else
        {
            _animationPlayer?.PlayBackwards("Open");
        }
        
    }
}
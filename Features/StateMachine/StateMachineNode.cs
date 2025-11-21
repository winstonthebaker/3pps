using Godot;
using System;
using System.Collections.Generic;

public abstract partial class StateMachineNode<T> : Node
{
    public bool Active { get; private set; } = false;

    public override void _Ready()
    {
        foreach(NodePath path in ActivateAutomaticallyStates)
        {
            if (GetNode<Node>(path) is StateMachineNode<T> smn)
            {
                _activateAutomatically.Add(smn);
            }
        }
    }

    public void Deactivate()
    {
        if (!Active)
            return;

        foreach (Node child in GetChildren())
        {
            if (child is StateMachineNode<T> smn && smn.Active)
            {
                smn.Deactivate();
            }
        }

        Active = false;
        OnStateExit();
    }

    public void Activate()
    {
        if (GetParent() is StateMachineNode<T> parent && !parent.Active)
        {
            parent.Activate();
        }
        if (!Active) { OnStateEnter(); }
        Active = true;
        foreach(StateMachineNode<T> smn in _activateAutomatically)
        {
            GD.Print("Activating auto state: " + smn.Name);
            smn.Activate();
        }
    }

    public void Transition(Node to)
    {
        Node current = this;
        Node commonAncestor = null;

        // Deactivate upward until we reach common ancestor
        while (current != null)
        {
            if (current.IsAncestorOf(to))
            {
                commonAncestor = current;
                break;
            }

            if (current is StateMachineNode<T> smn)
            {
                smn.Deactivate();
            }

            current = current.GetParent();
        }

        if (commonAncestor == null)
        {
            GD.PushWarning("StateMachine Transition: No common ancestor found.");
            return;
        }

        // Now activate the target node.
        if (to is StateMachineNode<T> targetState)
        {
            targetState.Activate();   // <--- the only activation needed
        }
    }

    protected T Context;
    public void SetContext(T context)
    {
        Context = context;
        foreach (Node child in GetChildren())
        {
            if (child is StateMachineNode<T> smn)
            {
                smn.SetContext(context);
            }
        }
    }

    public void ProcessStateRecursive(double delta)
    {
        if (!Active)
        {
            return;

        }
        ProcessState(delta);
        foreach (Node child in GetChildren())
        {
            if(child is StateMachineNode<T> smn && smn.Active)
            {
                smn.ProcessStateRecursive(delta);
            }
        }
    }
    public virtual void ProcessState(double delta)
    {

    }
    public virtual void OnStateEnter() { }
    public virtual void OnStateExit() { }




    [Export]
    public Godot.Collections.Array<NodePath> ActivateAutomaticallyStates = new Godot.Collections.Array<NodePath>();
    private List<StateMachineNode<T>> _activateAutomatically = new List<StateMachineNode<T>>();
}

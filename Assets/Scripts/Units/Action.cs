
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public abstract class Action
{
    [Serializable] public enum ActionState { CURRENT, COMPLETED, FAILED, FAILED_RESOLVED }


    // Declare variables
    public System.Action onUpdated = () => { };
    [SerializeField] public virtual string actionString => "Action";
    [NonSerialized] protected ActionQueue parentQueue;
    [SerializeField] public ActionState state { get; protected set; } = ActionState.CURRENT;
    [SerializeField] public Action reliantAction { get; protected set; }


    public Action(ActionQueue parentQueue_) => parentQueue = parentQueue_;


    public virtual void update()
    {
        // Handle reliant action
        if (state == ActionState.CURRENT && reliantAction != null)
        {
            if (reliantAction.state == ActionState.FAILED_RESOLVED) setState(ActionState.FAILED);
        }
    }


    public void setState(ActionState state_)
    {
        // Set state and call onUpdated
        if (state != state_)
        {
            state = state_;
            onUpdated();
        }
    }

    public virtual void setReliant(Action reliantAction_) => reliantAction = reliantAction_;
    
    public virtual ActionState getState() => state;


    public abstract void resolve();
}


[Serializable]
public class ActionQueue : Action
{
    // Declare variables
    [SerializeField] public override string actionString => "Action Queue";
    [SerializeReference] protected List<Action> actions = new List<Action>();


    public ActionQueue(ActionQueue parentQueue_) : base(parentQueue_) { }


    public override void update()
    {
        base.update();
        if (state != ActionState.CURRENT) return;

        // Complete when empty
        if (actions.Count == 0)
        {
            setState(ActionState.COMPLETED);
        }
        
        // Update action when not empty
        else
        {
            actions[0].update();
            if (actions[0].getState() == ActionState.COMPLETED) dequeue();
            else if (actions[0].getState() == ActionState.FAILED) actions[0].resolve();
            else if (actions[0].getState() == ActionState.FAILED_RESOLVED) dequeue();
        }
    }


    public void dequeue()
    {
        // Remove first action
        if (actions.Count == 0) return;
        actions.RemoveAt(0);
        onUpdated();
    }

    public void enqueue(Action action)
    {
        // Add a new action to the queue
        if (state == ActionState.FAILED) return;
        setState(ActionState.CURRENT);
        actions.Add(action);
        onUpdated();
    }

    public void enqueueBefore(Action reference, Action action)
    {
        // Add a new action before a reference action
        int index = actions.IndexOf(reference);
        if (index == -1) return;
        setState(ActionState.CURRENT);
        actions.Insert(index, action);
        onUpdated();
    }

    public void enqueueAfter(Action reference, Action action)
    {
        // Add a new action after a reference action
        int index = actions.IndexOf(reference);
        if (index == -1) return;
        setState(ActionState.CURRENT);
        actions.Insert(index + 1, action);
        onUpdated();
    }


    public override void resolve() { }


    public List<Action> getActions() => actions;
}


[Serializable]
public class PathfindAction : Action
{
    // Declare variables
    [SerializeField] public override string actionString => "Pathfind";
    [SerializeField] public Unit unit { get; protected set; }
    [SerializeField] public Node targetNode { get; protected set; }
    [SerializeField] public List<Node> path { get; protected set; }


    public PathfindAction(ActionQueue parentQueue_, Unit unit_, Node targetNode_) : base(parentQueue_)
    {
        // Initialize variables
        unit = unit_;
        targetNode = targetNode_;
    }


    public override void update()
    {
        base.update();
        if (state != ActionState.CURRENT) return;

        // Exit if in building
        if (unit.currentBuilding != null)
        {
            Action exitAction = new ExitBuildingAction(parentQueue, unit, unit.currentBuilding);
            setReliant(exitAction);
            parentQueue.enqueueBefore(this, exitAction);
            return;
        }

        // Either find or follow path
        if (path == null) findPath();
        else followPath();
    }


    private void findPath()
    {
        // Pathfind to target node
        Node currentNode = unit.currentNode;
        path = NodeGrid.instance.pathfind(currentNode, targetNode);
        onUpdated();

        // Deal with path possibilities
        if (path == null) setState(ActionState.FAILED);
        else if (path.Count == 0) setState(ActionState.COMPLETED);
    }


    private void followPath()
    {
        // Reached the end so complete
        if (path.Count == 0) { setState(ActionState.COMPLETED); return; }

        // Need to repathfind because blocked
        if (!path[0].active) { path = null; onUpdated(); return; }

        // Move towards first in list
        Vector3 target = path[0].getCentre();
        bool reached = unit.moveTowards(target);
        if (reached) path.RemoveAt(0);
    }


    public override void resolve() { }
}


[Serializable]
public class GotoPositionAction : Action
{
    // Declare variables
    [SerializeField] public override string actionString => "Goto Position";
    [SerializeField] public Unit unit { get; protected set; }
    [SerializeField] public Vector3 targetPos { get; protected set; }


    public GotoPositionAction(ActionQueue parentQueue_, Unit unit_, Vector3 pos_) : base(parentQueue_)
    {
        // Initialize variables
        unit = unit_;
        targetPos = pos_;
    }


    public override void update()
    {
        base.update();
        if (state != ActionState.CURRENT) return;

        // Move towards position
        bool reached = unit.moveTowards(targetPos);
        if (reached) setState(ActionState.COMPLETED);
    }


    public override void resolve() { }
}


[Serializable]
public class EnterBuildingAction : Action
{
    // Declare variables
    [SerializeField] public override string actionString => "Enter Building";
    [SerializeField] public Unit unit { get; protected set; }
    [SerializeField] public Building targetBuilding { get; protected set; }
    [SerializeField] public GotoPositionAction enteringAction { get; protected set; }


    public EnterBuildingAction(ActionQueue parentQueue_, Unit unit_, Building targetBuilding_): base(parentQueue_)
    {
        // Initialize variables
        unit = unit_;
        targetBuilding = targetBuilding_;
    }


    public override void update()
    {
        base.update();
        if (state != ActionState.CURRENT) return;
        
        // Entered building
        if (unit.currentBuilding == targetBuilding) { setState(ActionState.COMPLETED); return; }

        // Moved into building - reliant action has completed
        if (enteringAction != null) { unit.enterBuilding(targetBuilding); return; }

        // Exit current building if in one
        else if (unit.currentBuilding != null)
        {
            Action exitAction = new ExitBuildingAction(parentQueue, unit, unit.currentBuilding);
            setReliant(exitAction);
            parentQueue.enqueueBefore(this, exitAction);
            onUpdated();
        }

        // Create pathfinding action first
        else if (unit.currentNode != targetBuilding.entranceNode)
        {
            Action pathfindAction = new PathfindAction(parentQueue, unit, targetBuilding.entranceNode);
            setReliant(pathfindAction);
            parentQueue.enqueueBefore(this, pathfindAction);
            onUpdated();
        }

        // Enter building
        if (unit.currentBuilding == null)
        {
            enteringAction = new GotoPositionAction(parentQueue, unit, targetBuilding.entranceTarget);
            setReliant(enteringAction);
            parentQueue.enqueueBefore(this, enteringAction);
            onUpdated();
        }
    }


    public override void resolve() { }
}


[Serializable]
public class ExitBuildingAction : Action
{
    // Declare variables
    [SerializeField] public override string actionString => "Exit Building";
    [SerializeField] public Unit unit { get; protected set; }
    [SerializeField] public Building targetBuilding { get; protected set; }


    public ExitBuildingAction(ActionQueue parentQueue_, Unit unit_, Building targetBuilding_): base(parentQueue_)
    {
        // Initialize variables
        unit = unit_;
        targetBuilding = targetBuilding_;
    }


    public override void update()
    {
        base.update();
        if (state != ActionState.CURRENT) return;

        // Exit building
        if (unit.currentBuilding == targetBuilding)
        {
            unit.setPosition(targetBuilding.exitTarget);
            unit.exitBuilding(targetBuilding);
            Action gotoPosAction = new GotoPositionAction(parentQueue, unit, targetBuilding.exitNode.getCentre());
            setReliant(gotoPosAction);
            parentQueue.enqueueBefore(this, gotoPosAction);
            onUpdated();
        }

        // Complete if not in building
        if (unit.currentBuilding != targetBuilding)
        {
            setState(ActionState.COMPLETED);
        }
    }


    public override void resolve()
    {
        // Automatically resolve
        state = ActionState.FAILED_RESOLVED;
    }
}

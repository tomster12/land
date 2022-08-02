
using System.Collections.Generic;
using UnityEngine;


public class Building : NodeObject
{

    // Declare config, variables
    [Header("Building Config")]
    public Vector3Int entranceOffset;
    public Vector3 entranceTargetOffset;
    public Vector3Int exitOffset;
    public Vector3 exitTargetOffset;

    public Node entranceNode { get; private set; }
    public Vector3 entranceTarget { get; private set; }
    public Node exitNode { get; private set; }
    public Vector3 exitTarget { get; private set; }
    [Header("General")]
    [SerializeField] private List<Unit> unitsInside = new List<Unit>();


    private void Start() => placeBuilding();


    private void placeBuilding()
    {
        // Block space
        placeObject();

        // Initialize entrance / exit nodes
        Vector3Int entrancePos = Vector3Int.FloorToInt(baseGridPos + transform.rotation * entranceOffset);
        entranceNode = NodeGrid.instance.getNode(entrancePos);
        entranceTarget = entranceNode.getCentre() + transform.rotation * entranceTargetOffset;

        Vector3Int exitPos = Vector3Int.FloorToInt(baseGridPos + transform.rotation * exitOffset);
        exitNode = NodeGrid.instance.getNode(exitPos);
        exitTarget = exitNode.getCentre() + transform.rotation * exitTargetOffset;
    }


    private void unplaceBuilding() => unplaceObject();


    public void unitEnter(Unit unit) => unitsInside.Add(unit);
    public void unitLeave(Unit unit) => unitsInside.Remove(unit);
}

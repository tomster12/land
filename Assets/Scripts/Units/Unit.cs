
using System.Collections.Generic;
using UnityEngine;


public class Unit : MonoBehaviour
{
    // Declare static, references, config, variables
    public static List<Unit> units { get; private set; } = new List<Unit>();

    [Header("References")]
    [SerializeField] private GameObject bodyMesh;

    [Header("Config")]
    [SerializeField] private float movementSpeed = 0.9f;

    [Header("General")]
    [SerializeField] private ActionQueue actionQueue = new ActionQueue(null);
    public Node currentNode { get; private set; }
    public Building currentBuilding { get; private set; }
    public bool enteringBuilding { get; private set; }


    private void Start()
    {
        // Initialize currentNode
        currentNode = NodeGrid.instance.getNodeFromWorld(transform.position);
        units.Add(this);
    }


    private void Update()
    {
        actionQueue.update();
    }


    public void gotoNode(Node node)
    {
        // Create an action to go to a node
        Action pathfindAction = new PathfindAction(actionQueue, this, node);
        actionQueue.enqueue(pathfindAction);
    }

    public void gotoBuilding(Building building)
    {
        // Create an action to go to a building
        EnterBuildingAction enterBuildingAction = new EnterBuildingAction(actionQueue, this, building);
        actionQueue.enqueue(enterBuildingAction);
    }


    public ActionQueue getQueue() => actionQueue;


    public bool moveTowards(Vector3 pos)
    {
        // Move in direction
        Vector3 dir = pos - transform.position;
        float distance = movementSpeed * Time.deltaTime;

        // Reached position
        if (dir.magnitude < distance) { transform.position = pos; return true; }

        // Move towards
        transform.position += dir.normalized * distance;
        currentNode = NodeGrid.instance.getNodeFromWorld(transform.position);
        return false;
    }

    public void setPosition(Vector3 pos)
    {
        // set to position
        transform.position = pos;
    }

    public bool enterBuilding(Building building)
    {
        // Enter new building if possible
        if (currentBuilding != null) return false;
        building.unitEnter(this);
        currentBuilding = building;
        bodyMesh.SetActive(false);
        return true;
    }

    public bool exitBuilding(Building building)
    {
        // Exit current building if possible
        if (currentBuilding == null) return false;
        if (currentBuilding != building) return false;
        currentBuilding.unitLeave(this);
        currentBuilding = null;
        bodyMesh.SetActive(true);
        return true;
    }
}


using System;
using System.Collections.Generic;
using UnityEngine;


public class NodeGrid : MonoBehaviour
{
    // Declare static, references, variables
    public static Action<NodeGrid> instantiated;
    public static NodeGrid instance { get; private set; }
    private static Vector3Int[] neighbourDirections = {
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 0, 1),
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, 0, -1) };

    [Header("References")]
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private Vector2Int initialGridSize;
    
    private Dictionary<string, Node> nodes;


    public void Awake()
    {
        // Initialize variables
        instance = this;
        nodes = new Dictionary<string, Node>();
        createGrid(Vector3Int.zero, initialGridSize);
        instantiated(this);
    }


    public void createGrid(Vector3Int start, Vector2Int size)
    {
        // Loop over every position in the grid
        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z < size.y; z++)
            {
                // Calculate key and check if space is clear
                Vector3Int pos = new Vector3Int(start.x + x, start.y, start.z + z);
                if (!hasNode(pos))
                {
                    // Create a node
                    GameObject nodeGO = Instantiate(nodePrefab);
                    Node node = nodeGO.GetComponent<Node>();
                    nodeGO.transform.parent = transform;
                    nodeGO.transform.localPosition = pos;
                    setNode(pos, node);
                }
            }
        }

        // Connect all new grid squares
        connectGrid(start, size);
    }


    public void connectGrid(Vector3Int start, Vector2Int size)
    {
        // Loop over every position in the grid
        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z < size.y; z++)
            {
                // Get node at the given position
                Vector3Int pos = new Vector3Int(start.x + x, start.y, start.z + z);
                Node node = getNode(pos);

                // For each direction
                for (int i = 0; i < 4; i++)
                {
                    // Calculate neighbour key and add if exists
                    Vector3Int nPos = (new Vector3Int(pos.x, pos.y, pos.z)) + neighbourDirections[i];
                    if (hasNode(nPos))
                    {
                        Node nNode = getNode(nPos);
                        nNode.addNeighbour(node);
                        node.addNeighbour(nNode);
                    }
                }
            }
        }
    }
    
    
    public string getKey(Vector3Int pos) => pos.x + "," + pos.y + "," + pos.z;
    public void setNode(Vector3Int pos, Node node) => nodes[getKey(pos)] = node;
    public Node getNode(Vector3Int pos) => nodes[getKey(pos)];
    public bool hasNode(Vector3Int pos) => nodes.ContainsKey(getKey(pos));
    public Vector3Int getGridPos(Vector3 pos) => Vector3Int.FloorToInt(pos);
    public Node getNodeFromWorld(Vector3 pos) => getNode(getGridPos(pos));


    public List<Node> pathfind(Node start, Node end)
    {
        // Initialize variables
        List<Node> path = new List<Node>();
        List<Node> openSet = new List<Node>();
        List<Node> closedSet = new List<Node>();
        foreach (var nodePair in nodes) nodePair.Value.pf.reset();

        // Initially add start node
        openSet.Add(start);
        start.pf.scoreH = Vector3.Distance(start.transform.position, end.transform.position);

        // Find lowest current node
        while (openSet.Count > 0)
        {
            Node current = openSet[0];
            foreach (Node node in openSet)
            {
                if (node.pf.scoreF() < current.pf.scoreF()) current = node;
            }

            // Check if reached goal
            if (current == end)
            {
                while (current.pf.parent != null)
                {
                    path.Add(current);
                    current = current.pf.parent;
                }
                path.Add(current);
                path.Reverse();
                return path;
            }

            // Check neighbours of current
            openSet.Remove(current);
            closedSet.Add(current);
            foreach (Node neighbour in current.neighbours)
            {
                if (!closedSet.Contains(neighbour) && neighbour.active)
                {
                    float newScoreG = current.pf.scoreG + Vector3.Distance(current.transform.position, neighbour.transform.position);
                    if (neighbour.pf.parent == null || newScoreG < neighbour.pf.scoreG)
                    {
                        neighbour.pf.parent = current;
                        neighbour.pf.scoreG = newScoreG;
                        neighbour.pf.scoreH = Vector3.Distance(neighbour.transform.position, end.transform.position);
                        if (!openSet.Contains(neighbour)) openSet.Add(neighbour);
                    }
                }
            }
        }

        // Something gone wrong
        return null;
    }
}

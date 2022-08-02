
using System.Collections.Generic;
using UnityEngine;


public class NodeObject : MonoBehaviour
{
    // Declare config, variables
    [Header("NodeObject Config")]
    [SerializeField] private bool autoPlace;
    [SerializeField] private int shapeShapeWidth;
    [SerializeField] private int shapeShapeHeight;
    [SerializeField] private int[] shapeData;

    protected Vector3 baseGridPos;
    protected List<Node> nodesInside = new List<Node>();


    private void Start()
    {
        // Place on start
        if (autoPlace) placeObject();
    }


    protected void placeObject()
    {
        // Initialize variables
        baseGridPos = NodeGrid.instance.getGridPos(transform.position);
        nodesInside.Clear();

        // Loop over potential positions
        for (int x = 0; x < shapeShapeWidth; x++)
        {
            for (int y = 0; y < shapeShapeHeight; y++)
            {
                if (shapeData[y * shapeShapeWidth + x] == 1)
                {
                    // Get node and set blocked
                    Vector3Int nodePos = Vector3Int.FloorToInt(baseGridPos + transform.rotation * (new Vector3Int(x, 0, y)));
                    Node node = NodeGrid.instance.getNode(nodePos);
                    node.block();
                    nodesInside.Add(node);
                }
            }
        }
    }


    protected void unplaceObject()
    {
        // Unblock all nodes and clear
        foreach (Node node in nodesInside) node.unblock();
        nodesInside.Clear();
    }
}

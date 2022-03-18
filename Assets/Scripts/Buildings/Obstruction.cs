
using System.Collections.Generic;
using UnityEngine;


public class Obstruction : MonoBehaviour
{
    // Declare config, variables
    [Header("Obstruction Config")]
    [SerializeField] private int obstructShapeWidth;
    [SerializeField] private int obstructShapeHeight;
    [SerializeField] private int[] obstructData;

    protected Vector3 baseGridPos;
    protected List<Node> nodesInside = new List<Node>();


    private void Start() => obstruct();


    protected void obstruct()
    {
        // Initialize variables
        baseGridPos = NodeGrid.instance.getGridPos(transform.position);
        nodesInside.Clear();

        // Loop over potential positions
        for (int x = 0; x < obstructShapeWidth; x++)
        {
            for (int y = 0; y < obstructShapeHeight; y++)
            {
                if (obstructData[y * obstructShapeWidth + x] == 1)
                {
                    // Get node and set blocked
                    Vector3Int nodePos = Vector3Int.FloorToInt(baseGridPos + transform.rotation * (new Vector3Int(x, 0, y)));
                    Node node = NodeGrid.instance.getNode(nodePos);
                    node.obstruct();
                    nodesInside.Add(node);
                }
            }
        }
    }


    protected void unobstruct()
    {
        // Unblock all nodes and clear
        foreach (Node node in nodesInside) node.unobstruct();
        nodesInside.Clear();
    }
}

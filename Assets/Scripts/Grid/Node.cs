
using System.Collections.Generic;
using UnityEngine;


public class Node : MonoBehaviour
{
    // Pathfind helper class
    public class PFInfo
    {
        public Node parent;
        public float scoreG, scoreH;
        public void reset() { parent = null; scoreG = 0.0f; scoreH = 0.0f; }
        public float scoreF() { return scoreG + scoreH; }
    }


    // Declare references, variables
    [Header("References")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material activeMaterial;
    [SerializeField] private Material inactiveMaterial;

    [Header("General")]
    public PFInfo pf = new PFInfo();
    public HashSet<Node> neighbours { get; private set; } = new HashSet<Node>();
    private int unitCount = 0;
    private bool blocked = false;
    public bool active { get; private set; } = true;


    public void addNeighbour(Node node) { if (!hasNeighbour(node)) neighbours.Add(node); }
    public bool hasNeighbour(Node node) => neighbours.Contains(node);
    public Vector3 getCentre() => transform.position + new Vector3(0.5f, 0.0f, 0.5f);


    public void unitEnter() => unitCount++;
    public void unitExit() => unitCount--;


    public void block()
    {
        // Node becomes blocked
        blocked = true;
        setActive(false);
    }

    public void unblock()
    {
        // Node becomes unblocked
        blocked = false;
        setActive(true);
    }


    private void setActive(bool active_)
    {
        // Update active and material
        active = active_;
        meshRenderer.material = active ? activeMaterial : inactiveMaterial;
    }
}

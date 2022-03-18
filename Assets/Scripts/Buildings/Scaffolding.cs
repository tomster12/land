
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[ExecuteAlways]
public class Scaffolding : MonoBehaviour
{
    // Declare config, variables
    [Header("References")]
    [SerializeField] private GameObject piecePrefab;

    [Header("Config")]
    [SerializeField, Range(1, 10)] private int length = 1;
    private int _previousLength = 1;


    private void Awake()
    {
        if (!Application.IsPlaying(gameObject)) return; // Play Mode Only

        // Create grid right after instantiated
        if (NodeGrid.instance == null) {
            NodeGrid.instantiated += _ => createGrid();
        } else createGrid();
    }


    private void createGrid()
    {
        // Create grid
        Vector3Int gridPos = NodeGrid.instance.getGridPos(transform.position);
        Vector3Int rawSize = Vector3Int.RoundToInt(transform.rotation * new Vector3Int(length, 0, 1));
        gridPos += new Vector3Int(Mathf.Min(-1, rawSize.x) + 1, 0, Mathf.Min(-1, rawSize.z) + 1);
        Vector2Int gridSize = new Vector2Int(Mathf.Abs(rawSize.x), Mathf.Abs(rawSize.z));
        NodeGrid.instance.createGrid(gridPos, gridSize);
    }


    private void OnValidate()
    {
        if (Application.IsPlaying(gameObject)) return; // Editor Mode Only
        if (PrefabUtility.GetPrefabType(this) != PrefabType.PrefabInstance) return;

        // If value has been changed
        if (length != _previousLength)
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                // Destroy old pieces
                List<GameObject> children = new List<GameObject>();
                foreach (Transform child in transform) children.Add(child.gameObject);
                children.ForEach(child => DestroyImmediate(child));

                // Create new pieces
                for (int x = 0; x < length; x++)
                {
                    GameObject piece = Instantiate(piecePrefab);
                    piece.transform.parent = transform;
                    piece.transform.localPosition = Vector3.right * x;
                    piece.transform.localRotation = Quaternion.identity;
                }

                // Update variable
                _previousLength = length;
            };
        }
    }
}

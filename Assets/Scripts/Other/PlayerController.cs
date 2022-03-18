
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    // Declare references, variables
    [Header("References")]
    [SerializeField] private UIInspector inspector;


    private void Update()
    {
        // Raycast and detect click
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100.0f))
            {

                // No unit selected currently
                if (inspector.inspectingState == UIInspector.InspectingState.NONE)
                {
                    // Clicked on a unit so inspect
                    if (hit.transform.gameObject.tag == "Unit Mesh") inspector.inspectUnit(hit.transform.GetComponentInParent<Unit>());

                    // Clicked on a building so inspect
                    else if (hit.transform.gameObject.tag == "Building Mesh") inspector.inspectBuilding(hit.transform.GetComponentInParent<Building>());
                }

                // Currently has unit inspected
                else if (inspector.inspectingState == UIInspector.InspectingState.UNIT)
                {
                    // Clicked on a unit so inspect
                    if (hit.transform.gameObject.tag == "Unit Mesh") inspector.inspectUnit(hit.transform.GetComponentInParent<Unit>());

                    // Clicked on a node so pathfind to
                    else if (hit.transform.gameObject.tag == "Node Mesh") inspector.inspectedUnit.gotoNode(hit.transform.GetComponentInParent<Node>());

                    // Clicked on a building so enter
                    else if (hit.transform.gameObject.tag == "Building Mesh") inspector.inspectedUnit.gotoBuilding(hit.transform.GetComponentInParent<Building>());
                    
                    // Unselect if clicked on nothing
                    else inspector.uninspect();
                }
            }

            // Unselect if clicked on nothing
            else if (inspector.inspectingState != UIInspector.InspectingState.NONE) inspector.uninspect();
        }
    }
}

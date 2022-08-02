
using UnityEngine;


public class UIBuildingInspector : MonoBehaviour
{
    // Declare variables
    public Building inspectedBuilding { get; private set; }


    public void inspectBuilding(Building building)
    {
        if (inspectedBuilding != null) uninspect();

        // Inspect a new building
        gameObject.SetActive(true);
        inspectedBuilding = building;
    }


    public void uninspect()
    {
        // Uninspect current building
        gameObject.SetActive(false);
        inspectedBuilding = null;
    }
}

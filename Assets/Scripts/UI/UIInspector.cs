
using UnityEngine;


public class UIInspector : MonoBehaviour
{
    public enum InspectingState { NONE, UNIT, BUILDING };

    // Declare references, variables
    [Header("References")]
    [SerializeField] private UIUnitInspector unitInspector;
    [SerializeField] private UIBuildingInspector buildingInspector;

    public InspectingState inspectingState { get; private set; }
    public Unit inspectedUnit { get { return unitInspector.inspectedUnit; } }
    public Building inspectedBuilding { get { return buildingInspector.inspectedBuilding; } }


    private void Awake() => uninspect();


    public void inspectUnit(Unit unit)
    {
        // Inspect a new unit
        buildingInspector.uninspect();
        unitInspector.inspectUnit(unit);
        inspectingState = InspectingState.UNIT;
        gameObject.SetActive(true);
    }

    public void inspectBuilding(Building building)
    {
        // Inspect a new building
        unitInspector.uninspect();
        buildingInspector.inspectBuilding(building);
        inspectingState = InspectingState.BUILDING;
        gameObject.SetActive(true);
    }


    public void uninspect()
    {
        // Uninspect all inspectors
        gameObject.SetActive(false);
        unitInspector.uninspect();
        buildingInspector.uninspect();
        inspectingState = InspectingState.NONE;
    }
}

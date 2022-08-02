
using UnityEngine;


public class UIUnitInspector : MonoBehaviour
{
    // Declare references, variables
    [SerializeField] private UIActionQueueInspector actionQueueInspector;

    public Unit inspectedUnit { get; private set; }


    public void inspectUnit(Unit unit)
    {
        if (inspectedUnit != null) uninspect();

        // Inspect a new unit
        actionQueueInspector.inspectActionQueue(unit.getQueue());
        inspectedUnit = unit;
        inspectedUnit.GetComponent<Outline>().enabled = true;
        gameObject.SetActive(true);
    }


    public void uninspect()
    {
        // Uninspect current unit
        gameObject.SetActive(false);
        if (inspectedUnit != null) inspectedUnit.GetComponent<Outline>().enabled = false;
        actionQueueInspector.uninspect();
        inspectedUnit = null;
    }
}

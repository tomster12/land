
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class UIActionQueueInspector : MonoBehaviour
{
    // Declare references, variables
    [SerializeField] private Transform UIElementsParent;
    [SerializeField] private GameObject UIActionPrefab;
    [SerializeField] private GameObject UIActionQueuePrefab;
    
    public ActionQueue inspectedActionQueue { get; private set; }
    private List<GameObject> UIElements = new List<GameObject>();
    private bool isInspecting = false;


    public void inspectActionQueue(ActionQueue actionQueue)
    {
        // Inspect a new unit
        inspectedActionQueue = actionQueue;
        inspectedActionQueue.onUpdated += updateElements;
        isInspecting = true;
        updateElements();
        gameObject.SetActive(true);
    }


    private void updateElements()
    {
        // Destroy current children and elements
        foreach (Transform child in UIElementsParent) DestroyImmediate(child.gameObject);
        UIElements.ForEach(child => DestroyImmediate(child.gameObject));

        // Setup helper functions then call on inspected queue
        System.Action<List<Action>, Transform> createActionsList = null;
        System.Action<Action, Transform> createAction = null;
        createAction = (Action action, Transform parent) =>
        {
            GameObject actionElement = Instantiate(UIActionPrefab);
            actionElement.transform.SetParent(parent);
            TextMeshProUGUI text = actionElement.GetComponentInChildren<TextMeshProUGUI>();
            text.text = action.actionString;
            UIElements.Add(actionElement);
        };
        createActionsList = (List<Action> actions, Transform parent) =>
        {
            foreach (Action action in actions)
            {
                if (action is ActionQueue)
                {
                    GameObject actionQueueElement = Instantiate(UIActionQueuePrefab);
                    actionQueueElement.transform.SetParent(parent);
                    UIElements.Add(actionQueueElement);
                    createActionsList(((ActionQueue)action).getActions(), actionQueueElement.transform);
                }
                else createAction(action, parent);
            }
        };
        createActionsList(inspectedActionQueue.getActions(), UIElementsParent);
    }


    public void uninspect()
    {
        // Uninspect current unit
        gameObject.SetActive(false);
        if (inspectedActionQueue != null) inspectedActionQueue.onUpdated -= updateElements;
        inspectedActionQueue = null;
        isInspecting = false;
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class InteractionButton : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField]
    private CGUI gui;
    private int interactionIndex;

    public void SetIndex(int value)
    {
        interactionIndex = value;
    }

    public int GetIndex()
    {
        return interactionIndex;
    }
    [SerializeField]
    private Text interactionName;
    public void SetText(string value)
    {
        interactionName.text = value;
    }

    public void Interact()
    {
        gui.StartInteraction(interactionIndex);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        gui.SelectInteraction(interactionIndex);
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseHoverImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image iamge;
    public void Start()
    {
        iamge = GetComponent<Image>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        iamge.color = Color.grey;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        iamge.color = Color.black;
    }
}
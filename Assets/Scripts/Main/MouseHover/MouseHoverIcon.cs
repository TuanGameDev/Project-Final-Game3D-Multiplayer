using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MouseHoverIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image iamge;
    public void Start()
    {
        iamge = GetComponent<Image>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        iamge.color = Color.white;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        iamge.color = Color.black;
    }
}

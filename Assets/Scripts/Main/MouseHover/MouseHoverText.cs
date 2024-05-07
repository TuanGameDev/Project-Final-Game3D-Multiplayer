using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseHoverText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI textMeshPro;
    public void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        textMeshPro.color = Color.white;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        textMeshPro.color = Color.grey;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        textMeshPro.color = Color.grey;
    }
}
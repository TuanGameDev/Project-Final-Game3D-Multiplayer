using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    [SerializeField]
    private CGUI ui;
    private int itemIndex;

    public void SetIndex(int value)
    {
        itemIndex = value;
    }

    public int GetIndex()
    {
        return itemIndex;
    }
    [SerializeField]
    private Text itemName;
    public void SetText(string value)
    {
        itemName.text = value;
    }

    public void Select()
    {
        ui.SelectItem(GetComponent<RectTransform>(), itemIndex);
    }
}
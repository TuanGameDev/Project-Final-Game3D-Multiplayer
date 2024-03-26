using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class QualityButton : MonoBehaviour {
    private int index;
    public void SetIndex(int value)
    {
        index = value;
    }
    [SerializeField]
    private Text qualityName;
    public void SetName(string value)
    {
        qualityName.text = value;
    }

    public void ChangeQuality()
    {
        QualitySettings.SetQualityLevel(index, true);
    }
}

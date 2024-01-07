using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour
{
    public GameObject loadingBar;
    public TextMeshProUGUI loadingText;
    public Slider loadingSlider;
    public float loadSpeed = 0.5f;
    private float targetProgress = 0f;
    public GameObject mainPopup;
    public GameObject loadingbarPopup;

    private void Start()
    {
        loadingBar.SetActive(true);
    }
    void Update()
    {
        if (loadingSlider.value < targetProgress)
        {
            loadingSlider.value += loadSpeed * Time.deltaTime;
        }
        else
        {
            loadingSlider.value = targetProgress;
        }

        loadingText.text = "Loading:... " + (loadingSlider.value * 100f).ToString("F0") + "%";
        if (loadingSlider.value >= 1f)
        {
            mainPopup.SetActive(true);
            loadingbarPopup.SetActive(false);
        };
    }

    public void SetProgress(float progress)
    {
        targetProgress = Mathf.Clamp01(progress);
    }
}

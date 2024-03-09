using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using System.Collections;

public class VideoPlayerController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Camera _camera;
    public TextMeshProUGUI skipText;
    public float fadeDuration = 1f;

    private bool isSkipping;

    private void Start()
    {
        // Đăng ký sự kiện khi video chạy hết
        videoPlayer.loopPointReached += VideoPlayer_OnLoopPointReached;

        // Chạy video
        videoPlayer.Play();
        _camera.gameObject.SetActive(false);
        skipText.gameObject.SetActive(false);
        StartCoroutine(FadeInSkipText());
    }

    private void Update()
    {
        // Khi nhấn phím E và đang hiển thị Text "Nhấn E để skip video"
        if (Input.GetKeyDown(KeyCode.E) && !isSkipping && skipText.gameObject.activeSelf)
        {
            SkipVideo();
        }
    }

    private void VideoPlayer_OnLoopPointReached(VideoPlayer source)
    {
        // Ẩn game object chứa video player
        StartCoroutine(FadeOutVideo());
    }

    private void SkipVideo()
    {
        // Dừng video player
        videoPlayer.Stop();

        // Ẩn game object chứa video player
        gameObject.SetActive(false);

        // Hiện camera
        _camera.gameObject.SetActive(true);
    }

    private IEnumerator FadeInSkipText()
    {
        // Hiển thị Text "Nhấn E để skip video"
        skipText.gameObject.SetActive(true);

        // Tạo một màu bắt đầu với độ trong suốt là 0
        Color startColor = skipText.color;
        startColor.a = 0f;
        skipText.color = startColor;

        // Tính thời gian bắt đầu và thời gian kết thúc của hiệu ứng fadeIn
        float startTime = Time.time;
        float endTime = startTime + fadeDuration;

        while (Time.time < endTime)
        {
            // Tính toán độ trong suốt hiện tại dựa trên thời gian
            float timeRatio = (Time.time - startTime) / fadeDuration;
            float alpha = Mathf.Lerp(0f, 1f, timeRatio);

            // Đặt độ trong suốt cho Text
            Color currentColor = skipText.color;
            currentColor.a = alpha;
            skipText.color = currentColor;

            yield return null;
        }

        // Đảm bảo độ trong suốt cuối cùng là 1
        Color finalColor = skipText.color;
        finalColor.a = 1f;
        skipText.color = finalColor;

        isSkipping = false;
    }

    private IEnumerator FadeOutVideo()
    {
        // Tính thời gian bắt đầu và thời gian kết thúc của hiệu ứng fadeOut
        float startTime = Time.time;
        float endTime = startTime + fadeDuration;

        while (Time.time < endTime)
        {
            // Tính toán độ tối đa hiện tại dựa trên thời gian
            float timeRatio = (Time.time - startTime) / fadeDuration;
            float maxDarkness = Mathf.Lerp(0f, 1f, timeRatio);

            // Đặt độ tối đa cho camera
            _camera.backgroundColor = new Color(0f, 0f, 0f, maxDarkness);

            yield return null;
        }

        // Đảm bảo độ tối đa cuối cùng là 1
        _camera.backgroundColor = new Color(0f, 0f, 0f, 1f);

        SkipVideo();
    }
}
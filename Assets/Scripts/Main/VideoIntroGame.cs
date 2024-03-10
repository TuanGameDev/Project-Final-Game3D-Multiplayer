using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;
using Photon.Pun;
public class VideoIntroGame : MonoBehaviourPun
{
    public VideoPlayer videoPlayer;
    public Camera _camera;
    public GameObject leaderboardPopup;
    public GameObject hudPopup;
    public TextMeshProUGUI skipText;
    public float fadeDuration = 1f;
    private bool isSkipping;
    public Canvas canvas;
    private void Start()
    {
        videoPlayer.loopPointReached += VideoPlayer_OnLoopPointReached;
        videoPlayer.Play();
        skipText.gameObject.SetActive(false);
        leaderboardPopup.SetActive(false);
        hudPopup.SetActive(false);
        StartCoroutine(FadeInSkipText());
       if(!photonView.IsMine)
        {
            canvas.enabled = false;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isSkipping && skipText.gameObject.activeSelf)
        {
            SkipVideo();
        }
    }
    private void VideoPlayer_OnLoopPointReached(VideoPlayer source)
    {
        StartCoroutine(FadeOutVideo());
    }
    private void SkipVideo()
    {
        videoPlayer.Stop();
        gameObject.SetActive(false);
        leaderboardPopup.SetActive(true);
        hudPopup.SetActive(true);
    }
    private IEnumerator FadeInSkipText()
    {
        skipText.gameObject.SetActive(true);
        Color startColor = skipText.color;
        startColor.a = 0f;
        skipText.color = startColor;
        float startTime = Time.time;
        float endTime = startTime + fadeDuration;

        while (Time.time < endTime)
        {
            float timeRatio = (Time.time - startTime) / fadeDuration;
            float alpha = Mathf.Lerp(0f, 1f, timeRatio);
            Color currentColor = skipText.color;
            currentColor.a = alpha;
            skipText.color = currentColor;

            yield return null;
        }
        Color finalColor = skipText.color;
        finalColor.a = 1f;
        skipText.color = finalColor;

        isSkipping = false;
    }
    private IEnumerator FadeOutVideo()
    {
        float startTime = Time.time;
        float endTime = startTime + fadeDuration;

        while (Time.time < endTime)
        {
            float timeRatio = (Time.time - startTime) / fadeDuration;
            float maxDarkness = Mathf.Lerp(0f, 1f, timeRatio);
            _camera.backgroundColor = new Color(0f, 0f, 0f, maxDarkness);

            yield return null;
        }
        _camera.backgroundColor = new Color(0f, 0f, 0f, 1f);
        SkipVideo();
    }
}

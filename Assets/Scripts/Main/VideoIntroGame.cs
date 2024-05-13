using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class VideoIntroGame : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject objectToHide;

    private void Start()
    {
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        objectToHide.SetActive(false);
    }
}
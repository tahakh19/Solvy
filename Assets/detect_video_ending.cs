using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;


public class detect_video_ending : MonoBehaviour
{
    [SerializeField]
    VideoPlayer myVideoPlayer;

    // Start is called before the first frame update
    void Start()
    {
        myVideoPlayer.loopPointReached += DoSomethingwhenVideoFinish;
        
    }

    // Update is called once per frame

    void DoSomethingwhenVideoFinish(VideoPlayer vp)
    {
        Debug.Log("Dsds");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroTransition : MonoBehaviour
{
    public string nextSceneName;
    public GameObject videoPlayer;
    public float videoEndTime;

    void Awake()
    {
        StartCoroutine(IntroRoutine());
    }

    IEnumerator IntroRoutine()
    {
        if (videoPlayer != null)
        {
            videoPlayer.SetActive(true);
            yield return new WaitForSeconds(videoEndTime);
            Destroy(videoPlayer);
            SceneManager.LoadScene(nextSceneName);
        }
    }
}

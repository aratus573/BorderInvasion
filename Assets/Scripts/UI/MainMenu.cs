using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class MainMenu : MonoBehaviour
{
    [SerializeField] float delayTime;
    public PlayableDirector CutscenePlayer;

    public void Awake()
    {
        Time.timeScale = 1;
    }

    public void LoadLevel(int level)
    {
        CutscenePlayer.Play();
        StartCoroutine(delayLoadLevel(level));
    }
    IEnumerator delayLoadLevel(int level)
    {
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene(level);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MainAudioController : MonoBehaviour
{
    public static MainAudioController Instance;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip winClip;
    [SerializeField] private AudioClip loseClip1;
    [SerializeField] private AudioClip loseClip2;

    private void Awake() {
        Instance = this;
    }
    public void PlayWinClip()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(winClip);
    }
    public void PlayLoseClip()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(loseClip1);
        audioSource.PlayOneShot(loseClip2);
    }
}

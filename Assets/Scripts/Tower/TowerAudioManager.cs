using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class TowerAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip clip;
    [SerializeField] private List<AudioClip> clips;
    [SerializeField] private bool isBasicTower;

    public void Play()
    {
        if (isBasicTower)
        {
            int randomNum = Random.Range(0,clips.Count);
            source.PlayOneShot(clips[randomNum]);
        }
        else
        {
            if (!source.isPlaying)
            {
                source.Play();
            }
        }
    }


}

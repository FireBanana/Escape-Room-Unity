using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public List<AudioClip> AudioList = new List<AudioClip>();
    private AudioSource audioSource;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudio(int audioId)
    {
        audioSource.clip = AudioList[audioId];
        audioSource.Play();
    }

    public void StopAudio()
    {
        audioSource.Stop();
    }
}

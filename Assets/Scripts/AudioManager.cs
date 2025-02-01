using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioClip backgroundMusic;
    public AudioClip flipCardSound;
    public AudioClip matchSound;
    public AudioClip mismatchSound;
    public AudioClip shuffleSound;

    private AudioSource audioSource;

    void Awake()
    {
        // Singleton pattern ile AudioManager her sahnede kullanılabilir
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Oyun sahnesi değiştiğinde müzik kesilmesin
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void PlayFlipCardSound()
    {
        audioSource.PlayOneShot(flipCardSound);
    }

    public void PlayMatchSound()
    {
        audioSource.PlayOneShot(matchSound);
    }

    public void PlayMismatchSound()
    {
        audioSource.PlayOneShot(mismatchSound);
    }

    public void PlayShuffleSound()
    {
        audioSource.PlayOneShot(shuffleSound);
    }
}


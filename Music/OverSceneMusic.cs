using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverSceneMusic : MonoBehaviour
{
    AudioSource m_audio;

    private void Awake()
    {
        m_audio = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    public void Play()
    {
        if (!m_audio.isPlaying)
            m_audio.Play();
    }

    public void Stop()
    {
        m_audio.Stop();
    }
}

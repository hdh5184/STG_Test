using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public AudioSource audio;

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    public void StartEffect()
    {
        audio.Play();
    }

    public void InitEffect()
    {
        audio.Stop();
        gameObject.SetActive(false);
    }
}

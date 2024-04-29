using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioClip[] audioClip; // 오디오 파일 모음

    private void Awake()
    {
        if (instance != this && instance != null)
        {
            Destroy(gameObject); return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // 오디오 선택
    public AudioClip getAudioClip(string clipName)
    {
        switch (clipName)
        {
            case "Explode":         return audioClip[0];
            case "ExplodeBoss":     return audioClip[1];
            case "ExplodePlayer":   return audioClip[2];
            case "PShoot":          return audioClip[3];
            case "EShotL":          return audioClip[4];
            case "EShotS":          return audioClip[5];
            case "GetCoin":         return audioClip[6];
            case "GetItem":         return audioClip[7];
            case "MoveDown":        return audioClip[8];
            case "Button":          return audioClip[9];
            case "Trigger":         return audioClip[10];
            default:
                Debug.Log($"\"{clipName}\" 오디오를 찾을 수 없습니다.");
                return null;
        }
    }
}

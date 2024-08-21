using UnityEngine;

/* <Effect 클래스> */
public class Effect : MonoBehaviour
{
    // 오디오 저장
    public AudioSource audio;

    /// <summary> Effect 초기 설정 </summary>
    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    /// <summary> Effect 출현 </summary>
    public void StartEffect()
    {
        audio.Play();
    }

    /// <summary> Effect 종료 </summary>
    public void InitEffect()
    {
        audio.Stop();
        gameObject.SetActive(false);
    }
}

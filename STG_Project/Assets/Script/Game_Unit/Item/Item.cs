using UnityEngine;

/* <Item 클래스> */
public class Item : MonoBehaviour
{
    public AudioManager audioManager;





    /*************** 게임 루프 ***************/

    /// <summary> Item 초기 설정 </summary>
    protected virtual void Awake()
    {
        audioManager = AudioManager.instance;
    }

    /// <summary> Item 생성 </summary>
    protected virtual void OnEnable() { }

    /// <summary> Item 로직 </summary>
    protected virtual void Update() { }





    /*************** 기타 메서드 모음 ***************/

    /// <summary> Item 획득 시 동작 </summary>
    public virtual void GetItem(AudioSource audio) { }
}

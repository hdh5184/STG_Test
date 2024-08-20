using UnityEngine;
using static StageManager;

/* <Bullet 클래스> */
public class Bullet : MonoBehaviour
{
    // 1. 타입
    public string getBulletType;

    // 2. 탄 속성
    public float speed = 0f;
    public byte power = 0;
    float degreeZ = 0f;

    // 3. 시간
    float fieldTime = 0f;



    // d. 대리자
    public delegate void Set_Move(ref MoveData data);
    public Set_Move set_Mov;

    // s. 데이터 구조체
    public struct MoveData
    {
        public Transform transform;
        public Vector3 targetPos;
        public Vector2 moveVec;
        public Vector2 shootVec;
        public float degreeZ;
        public float fieldTime;
        public bool isEnemyBullet;
    }

    public MoveData moveData;





    /*************** 게임 루프 ***************/

    /// <summary> Bullet 생성 </summary>
    protected virtual void OnEnable()
    {
        Init_Attribute();
        Init_Data();
    }

    /// <summary> Bullet 로직 실행</summary>
    protected virtual void Update()
    {
        if (!isGamePlay) return;

        Timing();
        Move();
    }





    /*************** 초기화 모음 ***************/

    /// <summary> Bullet 속성 지정 </summary>
    public void Set_Attribute(string bulletType, float fireSpeed)
    {
        getBulletType = bulletType;
        speed = fireSpeed;
    }

    /// <summary> Bullet 속성 초기화 </summary>
    protected void Init_Attribute()
    {
        degreeZ = 0f;
        fieldTime = 0.5f;
    }

    /// <summary> Bullet 내 데이터 초기화 </summary>
    protected void Init_Data()
    {
        moveData.transform = transform;
        moveData.degreeZ = degreeZ;
        moveData.fieldTime = fieldTime;
    }





    /*************** Bullet 동작 로직 ***************/

    /// <summary> 시간 갱신 </summary>
    void Timing() => fieldTime += Time.deltaTime;

    /// <summary> 이동 로직 </summary>
    protected virtual void Move()
    {
        moveData.fieldTime = fieldTime;
        set_Mov(ref moveData);
    }





    /*************** Bullet 충돌 로직 모음 ***************/

    /// <summary> 충돌 (필드 이탈) </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Field"))
        {
            gameObject.SetActive(false);
        }
    }
}

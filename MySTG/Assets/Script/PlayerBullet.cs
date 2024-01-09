using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public PlayerBulletType PBType;


    float shootTime = 0;

    public enum PlayerBulletType
    {
        Bullet, Missile, Laser
    }

    void Start()
    {
        
    }

    private void OnEnable()
    {
        shootTime = 0;
    }

    void Update()
    {
        shootTime += 0.05f;
        switch (PBType)
        {
            case PlayerBulletType.Bullet:   transform.Translate(Vector2.up * 20f * Time.deltaTime); break;
            case PlayerBulletType.Missile:  transform.Translate(Vector2.up * 1f * shootTime * Time.deltaTime); break;
            case PlayerBulletType.Laser:
                transform.Translate(Vector2.up * 25f * Time.deltaTime);
                transform.position =
                    new Vector2(transform.position.x + GameManager.instance.playerMovingVec.x, transform.position.y);
                break;
        }
        

        if (transform.position.y > GameManager.instance.transform.position.y + 8f) gameObject.SetActive(false);
        if (Input.GetKeyUp(KeyCode.Z) && PBType == PlayerBulletType.Laser) gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Field"))
        {
            gameObject.SetActive(false);
        }
    }
}

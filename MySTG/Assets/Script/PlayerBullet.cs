using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        transform.Translate(Vector2.up * 15f * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Field"))
        {
            gameObject.SetActive(false);
        }
    }
}

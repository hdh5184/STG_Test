using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class backgroundTest : MonoBehaviour
{
    public bool backgroundTurning = false;
    public byte RotateNum = 0;
    public float RotateZ = 0;

    private void OnEnable()
    {
        backgroundTurning = false;
        RotateNum = 0;
        RotateZ = 0;
        transform.position = new Vector3(5.725f, 16f, 0);
    }

    void Update()
    {
        if (StageManager.stageState == StageManager.StageState.End) return;

        if (transform.position.y <= -8f && !backgroundTurning)
        {
            backgroundTurning = true; RotateNum++;
        }

        if (backgroundTurning)
        {
            transform.RotateAround(
                Camera.main.ScreenToWorldPoint(
                    new Vector2(Screen.width * 0.5f, Screen.height * 0.5f)),
                Vector3.forward, 10 * Time.deltaTime);

            RotateZ = transform.rotation.eulerAngles.z;

            if (RotateZ > 180 && RotateNum == 1)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                transform.position = new Vector3(5.725f ,transform.position.y, 0);
                backgroundTurning = false;
                RotateZ = transform.rotation.eulerAngles.z;
            }

            if (RotateZ < 10 && RotateNum == 2)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                transform.position = new Vector3(5.725f, transform.position.y, 0);
                backgroundTurning = false;
                RotateZ = transform.rotation.eulerAngles.z;
                RotateNum = 0;
            }
        }
        transform.Translate(Vector2.down * Time.deltaTime, Space.World);


    }
}

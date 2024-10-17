using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElderlyWelfare : MonoBehaviour
{
    float time = 0;

    private void FixedUpdate()
    {
        //노인 복지
        time += Time.deltaTime;
        if (time >= 10)
        {
            time = 0;
            gameObject.SetActive(false);
        }
    }
}

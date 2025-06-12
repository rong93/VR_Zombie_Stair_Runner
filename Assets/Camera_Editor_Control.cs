using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Editor_Control : MonoBehaviour
{
#if UNITY_EDITOR
    public float speed = 5f;
    // Update is called once per frame
    void Update()
    {
        float horizontal  = Input.GetAxis("Mouse X") * speed;
        float vertical = Input.GetAxis("Mouse Y") * speed;

        //transform.Rotate(0f, horizontal, 0f, Space.World);
        //transform.Rotate(-vertical, 0f, 0f, Space.Self);
    }
#endif
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] float move_speed = 1000f;

    private GvrReticlePointer reticle;  // 卡片板的瞄準器
    private Vector3 initialPosition;   // 3D角色物件的初始位置
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // private void Awake()
    // {
    //     // 獲取CardboardReticle組件
    //     reticle = GetComponent<GvrReticlePointer>();

    //     // 記錄3D角色物件的初始位置
    //     initialPosition = transform.position;
    // }


    // Update is called once per frame
    void Update()
    {
       

        if(Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(move_speed * Time.deltaTime,0 , 0);
        }
        else if(Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(-move_speed * Time.deltaTime,0, 0);
        }
        
        // float h = Input.GetAxis("Mouse X");
        // float v = Input.GetAxis("Mouse Y");
        // transform.Translate(h* move_speed * Time.deltaTime, v* move_speed * Time.deltaTime, 0);

        GameObject wallObject = GameObject.FindWithTag("Wall_front");
        Transform camera = Camera.main.transform;
        RaycastHit hit;
        Ray ray = new Ray(camera.position, camera.rotation * Vector3.forward);
            
        if (Physics.Raycast(ray, out hit))
        {
            if(hit.transform.gameObject == wallObject)
            {
                GameObject character = GameObject.FindWithTag("Character");
                float step = move_speed * Time.deltaTime;
                Vector3 target_point = new Vector3(hit.point.x, 0, 0); 
                transform.position = Vector3.MoveTowards(transform.position, target_point, step);

                Debug.Log(target_point);
            }
        }

    }

    void OnCollisionEnter(Collision other) 
    {
        if(other.gameObject.tag == "Floor1")
        {
            Debug.Log("撞到1");
        }
        else if(other.gameObject.tag == "Floor2")
        {
            Debug.Log("撞到2");
        }
        
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.tag == "End_Line")
        {
            Debug.Log("你輸了");
        }
    }
}

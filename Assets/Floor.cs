﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [SerializeField] float moveSpeed = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, moveSpeed * Time.deltaTime, 0);
        if(transform.position.y > 26f)
        {
            Destroy(gameObject);
            transform.parent.GetComponent<FloorManager>().CreateFloor();
        }
    }
}

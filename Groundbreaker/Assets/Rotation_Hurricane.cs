﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation_Hurricane : MonoBehaviour {

    public float speed = 10f;


    void Update()
    {
        transform.Rotate(Vector3.back, speed * Time.deltaTime);
    }
}

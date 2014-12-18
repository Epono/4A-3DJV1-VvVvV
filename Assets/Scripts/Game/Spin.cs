﻿using UnityEngine;
using System.Collections;

public class Spin : MonoBehaviour {
    public float speed = 10f;


    void Update() {
        transform.Rotate(Vector3.down, speed * Time.deltaTime);
        transform.Rotate(Vector3.left, speed * Time.deltaTime);
    }
}
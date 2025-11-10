using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotator : MonoBehaviour
{
    public float rotationSpeed = 30f;
    public bool clockwise = true;
    void Update()
    {
        float dir = clockwise ? 1f : -1f;
        transform.Rotate(0, 0, dir * rotationSpeed * Time.deltaTime);
    }
}

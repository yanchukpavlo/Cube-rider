using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] float angle;

    void Update()
    {
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }
}

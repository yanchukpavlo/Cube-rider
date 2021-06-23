using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class BoxController : MonoBehaviour
{
    private void Start()
    {
        GetComponent<MeshRenderer>().material = GameManager.instance.BoxMaterial;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            transform.parent = GameManager.instance.LevelTransform;
            EventsManager.instance.AddBoxTrigger(-1);
        }
    }

    void OffAnimation()
    {
        GetComponent<Animation>().enabled = false;
    }
}

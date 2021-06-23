using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Animator))]
public class AdditionalBoxController : MonoBehaviour
{
    [SerializeField] int boxAdd = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventsManager.instance.AddBoxTrigger(boxAdd);
            GetComponent<Animator>().SetTrigger("fadeIn");

            Destroy(gameObject, 2f);
        }
    }
}

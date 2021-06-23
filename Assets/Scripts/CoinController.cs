using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Animator))]
public class CoinController : MonoBehaviour
{
    [SerializeField] int coinAmount = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventsManager.instance.AddCoinTrigger(coinAmount);
            GetComponent<Animator>().SetTrigger("fadeIn");

            Destroy(gameObject, 2f);
        }
    }
}

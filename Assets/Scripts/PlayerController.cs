using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [Header("Box")]
    [SerializeField] Transform boxParent;
    [SerializeField] GameObject boxPref;
    [SerializeField] float boxHeight = 1;

    [Header("Character")]
    [SerializeField] Rigidbody characterRb;
    //[SerializeField] float jumpPower;

    public Vector3[] TargetsForCamera { get { return GetTargets(); } }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        EventsManager.instance.onChangeStateTrigger += ChangeStateTrigger;
        EventsManager.instance.onAddBoxTrigger += AddBoxTrigger;
    }

    private void OnDestroy()
    {
        EventsManager.instance.onChangeStateTrigger -= ChangeStateTrigger;
        EventsManager.instance.onAddBoxTrigger -= AddBoxTrigger;
    }

    Vector3[] GetTargets()
    {
        Vector3[] vectors = { characterRb.position, boxParent.GetChild(0).transform.position };
        return vectors;
    }

    private void ChangeStateTrigger(EventsManager.GameState state)
    {
        switch (state)
        {
            case EventsManager.GameState.Menu:
                break;

            case EventsManager.GameState.Play:
                break;

            case EventsManager.GameState.Win:
                break;

            case EventsManager.GameState.Dead:
                break;

            default:
                break;
        }
    }

    private void AddBoxTrigger(int amount)
    {
        if (amount < 0)
        {
            if (boxParent.childCount < 1)
            {
                EventsManager.instance.ChangeStateTrigger(EventsManager.GameState.Dead);
            }
        }
        else
        {
            //characterRb.AddForce(Vector3.up * (jumpPower + amount), ForceMode.Impulse);

            for (int i = 0; i < amount; i++)
            {
                GameObject box = Instantiate(boxPref, boxParent);
                box.transform.position = new Vector3(boxParent.transform.position.x,
                    boxParent.childCount * boxHeight,
                    boxParent.transform.position.z);
            }

            characterRb.position = new Vector3(characterRb.position.x, boxParent.childCount * boxHeight + boxHeight, characterRb.position.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("End"))
        {
            EventsManager.instance.ChangeStateTrigger(EventsManager.GameState.Win);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerScript : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float distToGround = 0.2f;
    [SerializeField] LayerMask groundMask;

    Animator animator;
    bool isGrounded;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        EventsManager.instance.onChangeStateTrigger += ChangeStateTrigger;
    }

    private void OnDisable()
    {
        EventsManager.instance.onChangeStateTrigger -= ChangeStateTrigger;
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, distToGround, groundMask);
        animator.SetBool("isFalling", !isGrounded);
    }

    private void ChangeStateTrigger(EventsManager.GameState state)
    {
        switch (state)
        {
            case EventsManager.GameState.Play:
                break;

            case EventsManager.GameState.Win:
                animator.SetTrigger("win");
                break;

            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            EventsManager.instance.ChangeStateTrigger(EventsManager.GameState.Dead);
        }
    }

}

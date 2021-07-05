using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] float speed;
    [SerializeField] float speedAside = 2;
    [SerializeField] float turnSmoothTime = 0.1f;

    [Header("Check")]
    [SerializeField] Transform road—heckingForward;
    [SerializeField] Transform road—heckingRight;
    [SerializeField] Transform road—heckingLeft;
    [SerializeField] Transform ground—heckingRight;
    [SerializeField] Transform ground—heckingLeft;
    [SerializeField] float rayLength = 50f;
    [SerializeField] LayerMask checkLayer;

    VariableJoystick joystick;
    Rigidbody rb;

    public float targetAngle = 0;
    float turnSmoothVelocity;
    bool isMove;
    bool isCheckTurn = true;
    bool canMoveR = true;
    bool canMoveL = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        joystick = FindObjectOfType<VariableJoystick>();

        EventsManager.instance.onChangeStateTrigger += ChangeStateTrigger;

        isMove = GameManager.instance.InGame;
        float width = GameManager.instance.LevelWidth;
        road—heckingForward.position = road—heckingForward.position + transform.forward * (width * 2/3);
        road—heckingRight.position = road—heckingRight.position + transform.right * width * 3;
        road—heckingLeft.position = road—heckingLeft.position + -transform.right * width * 3;
    }

    private void OnDestroy()
    {
        EventsManager.instance.onChangeStateTrigger -= ChangeStateTrigger;
    }

    private void Update()
    {
        if (isMove)
        {
            Move();
        }
    }

    private void ChangeStateTrigger(EventsManager.GameState state)
    {
        switch (state)
        {
            case EventsManager.GameState.Menu:
                StopMove();
                break;

            case EventsManager.GameState.Play:
                isMove = true;
                break;

            case EventsManager.GameState.Win:
                StopMove();
                break;

            case EventsManager.GameState.Dead:
                StopMove();
                break;

            default:
                break;
        }
    }

    void Move()
    {
        float horizontal = joystick.Horizontal;

        if (isCheckTurn)
        {
            if (!Physics.Raycast(road—heckingForward.position, Vector3.down, rayLength, checkLayer))
            {
                Debug.Log("Need to turn.");

                if (Physics.Raycast(road—heckingRight.position, Vector3.down, rayLength, checkLayer))
                {
                    Debug.Log("Turn right.");
                    //transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 90, 0);
                    targetAngle += 90;
                }
                else if (Physics.Raycast(road—heckingLeft.position, Vector3.down, rayLength, checkLayer))
                {
                    Debug.Log("Turn left.");
                    //transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y - 90, 0);
                    targetAngle -= 90;
                }
                else
                {
                    Debug.LogError("Has nowhere to turn!");
                    StopMove();
                    Debug.Break();
                }

                StartCoroutine(WaitToCheckTurnOn());
            }
        }

        canMoveR = Physics.Raycast(ground—heckingRight.position, Vector3.down, rayLength, checkLayer);
        canMoveL = Physics.Raycast(ground—heckingLeft.position, Vector3.down, rayLength, checkLayer);

        if (horizontal > 0 && !canMoveR)
        {
            horizontal = 0;
        }
        else if (horizontal < 0 && !canMoveL)
        {
            horizontal = 0;
        }

        Vector3 direction = transform.right * horizontal * speedAside + transform.forward;

        //rb.velocity = direction * Time.deltaTime * speed;
        rb.MovePosition(transform.localPosition + (direction * Time.deltaTime * speed));

        if (!isCheckTurn)
        {
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);
        }
    }

    void StopMove()
    {
        rb.velocity = Vector3.zero;
        isMove = false;
    }

    IEnumerator WaitToCheckTurnOn()
    {
        isCheckTurn = false;
        yield return new WaitForSeconds(0.75f);
        isCheckTurn = true;
    }
}

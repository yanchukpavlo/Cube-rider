using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    [Header("Zoom")]
    [SerializeField] float smoothTime = 0.5f;
    [SerializeField] float minZoom = 15f;
    [SerializeField] float maxZoom = 40f;
    [SerializeField] float zoomLimiter = 0.5f;

    bool multipleTarget;
    GameObject fakeTransformObj;
    CinemachineVirtualCamera cinemachine;
    Vector3[] targetsPos;

    Vector3 velocity;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        cinemachine = GetComponent<CinemachineVirtualCamera>();
        fakeTransformObj = new GameObject("Fake Transform Obj");
    }

    private void Start()
    {
        EventsManager.instance.onChangeStateTrigger += ChangeStateTrigger;
    }

    private void OnDestroy()
    {
        EventsManager.instance.onChangeStateTrigger -= ChangeStateTrigger;
    }

    private void ChangeStateTrigger(EventsManager.GameState state)
    {
        switch (state)
        {
            case EventsManager.GameState.Menu:
                break;

            case EventsManager.GameState.Play:
                multipleTarget = true;
                SetTarget(FindObjectOfType<PlayerController>().transform);
                break;

            case EventsManager.GameState.Win:
                multipleTarget = false;
                break;

            case EventsManager.GameState.Dead:
                multipleTarget = false;
                break;

            default:
                break;
        }
    }

    public void SetTarget(Transform target)
    {
        if (multipleTarget)
        {
            fakeTransformObj.transform.position = target.position;
            fakeTransformObj.transform.parent = target;
            fakeTransformObj.transform.rotation = target.rotation;

            cinemachine.LookAt = fakeTransformObj.transform;
            cinemachine.Follow = fakeTransformObj.transform;
        }
        else
        {
            cinemachine.LookAt = target;
            cinemachine.Follow = target;
        }
    }

    public void SetLookAt(Transform target)
    {
        cinemachine.LookAt = target;
    }

    private void LateUpdate()
    {
        if (multipleTarget)
        {
            targetsPos = PlayerController.instance.TargetsForCamera;
            if (targetsPos.Length == 0)
                return;

            Move();
            Zoom();
        }
    }

    #region Multiple target methods

    void Move()
    {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPos = centerPoint;
        fakeTransformObj.transform.position = Vector3.SmoothDamp(fakeTransformObj.transform.position, newPos, ref (velocity), smoothTime);
    }

    void Zoom()
    {
        float newZoom = Mathf.Lerp(minZoom, maxZoom, GetGreatestDistance()) / zoomLimiter;
        cinemachine.m_Lens.FieldOfView = Mathf.Lerp(cinemachine.m_Lens.FieldOfView, newZoom, Time.deltaTime);
    }

    float GetGreatestDistance()
    {
        var bounds = new Bounds(targetsPos[0], Vector3.zero);
        for (int i = 0; i < targetsPos.Length; i++)
        {
            bounds.Encapsulate(targetsPos[i]);
        }

        return bounds.size.x;
    }

    Vector3 GetCenterPoint()
    {
        if (targetsPos.Length == 1)
        {
            return targetsPos[0];
        }

        var bounds = new Bounds(targetsPos[0], Vector3.zero);
        for (int i = 0; i < targetsPos.Length; i++)
        {
            bounds.Encapsulate(targetsPos[i]);
        }

        return bounds.center;
    }

    #endregion
}

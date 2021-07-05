using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Prefabs")]
    [SerializeField] GameObject groundPref;
    [SerializeField] GameObject playerPref;
    [SerializeField] GameObject finishPref;
    [SerializeField] GameObject coinPref;


    [Header("Level")]
    [SerializeField] float levelLength = 200;
    float levelWidth = 5.3f;
    [SerializeField] int turnsCount = 2;
    [SerializeField] float offsetForwardZ = 7;
    [SerializeField] float offsetBackZ = 5;
    [SerializeField] float bonusObstacleZ = 2;
    [SerializeField] float distanceBetweenBox = 5;
    [SerializeField] float distanceBetweenObstacle = 8;
    [SerializeField] float distanceBetweenCoin = 15;

    [Header("Box")]
    [SerializeField] float boxHeight = 1;
    [SerializeField] GameObject additionalBoxPref;
    [SerializeField] List<GameObject> obstaclePrefs;
    [SerializeField] List<Material> playerBoxMaterials;

    Material currentMaterial;
    GameObject level;

    float angle = 0;
    bool notReadyForGame;
    int coinAmount;
    bool inGame;

    //public float LevelLength { get { return levelLength; } }
    public bool InGame { get { return inGame; } }
    public float LevelWidth { get { return levelWidth; } }
    public Transform LevelTransform { get { return level.transform; } }
    public Material BoxMaterial { get { return currentMaterial; } }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Application.targetFrameRate = 61;

        EventsManager.instance.onChangeStateTrigger += ChangeStateTrigger;
        coinAmount = PlayerPrefs.GetInt("Coin", 0);
        PrepareToGame();
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
                inGame = true;
                if (notReadyForGame) PrepareToGame();
                break;

            case EventsManager.GameState.Win:
                notReadyForGame = true;
                coinAmount += UI_Controller.instance.Coin;
                PlayerPrefs.SetInt("Coin", coinAmount);
                break;

            case EventsManager.GameState.Dead:
                notReadyForGame = true;
                break;

            default:
                break;
        }
    }

    void PrepareToGame()
    {
        SetBoxMaterial();
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        if (level != null) Destroy(level);

        level = new GameObject("Level");

        angle = 0;
        GameObject spawn;
        Transform previousTransform = level.transform;
        Vector3 newPos = Vector3.zero;
        Vector3 sizeOverlapBox = new Vector3(levelWidth, 0.2f, 0.2f);
        bool inRight = false;
        bool inLeft = false;
        float roadLength = levelLength / (turnsCount + 1);
        float maxZ = roadLength - offsetForwardZ- offsetBackZ;

        for (int i = 0; i <= turnsCount; i++)
        {
            GameObject ground = Instantiate(groundPref, level.transform);
            ground.transform.localScale = new Vector3(levelWidth, 1, roadLength);
            ground.transform.position = Vector3.zero;

            float zBox = offsetForwardZ;
            float zOffset = offsetForwardZ + bonusObstacleZ;
            float zCoin = offsetForwardZ;

            if (i == 0)
            {
                previousTransform = ground.transform;

                while (zBox < maxZ)
                {
                    spawn = Instantiate(additionalBoxPref,
                    previousTransform.position + previousTransform.transform.forward * zBox + previousTransform.transform.right * Random.Range(-levelWidth / 2 + (levelWidth / 6), levelWidth / 2 - (levelWidth / 6)) + Vector3.up * boxHeight,
                    Quaternion.identity);
                    spawn.transform.parent = level.transform;

                    zBox += distanceBetweenBox;
                }

                while (zOffset < maxZ)
                {
                    int q = 0;
                    bool inLoop = true;
                    do
                    {
                        q++;
                        Vector3 pos = previousTransform.position + previousTransform.transform.forward * zOffset + Vector3.up * boxHeight;
                        
                        if (Physics.OverlapBox(pos, sizeOverlapBox, previousTransform.transform.rotation).Length == 0)
                        {
                            spawn = Instantiate(obstaclePrefs[Random.Range(0, obstaclePrefs.Count)],
                                pos,
                                previousTransform.rotation);
                            spawn.transform.parent = level.transform;

                            zOffset += distanceBetweenObstacle;
                            inLoop = false;
                            break;
                        }
                        else
                        {
                            Debug.Log("The place is taken");
                            zOffset += 1;
                        }

                        if (q > (int)roadLength) break;
                    }
                    while (inLoop);
                }

                continue;
            }

            if (inRight)
            {
                RotateRoadObject(ground.transform, false);
                inRight = false;
            }
            else if (inLeft)
            {
                RotateRoadObject(ground.transform, true);
                inLeft = false;
            }
            else
            {
                float r = Random.value;
                if (r > 0.5f)
                {
                    RotateRoadObject(ground.transform, true);
                    inRight = true;
                }
                else
                {
                    RotateRoadObject(ground.transform, false);
                    inLeft = true;
                }
            }

            ground.transform.position = GetNewRoadPos(previousTransform, roadLength);
            previousTransform = ground.transform;

            while (zBox < maxZ)
            {
                spawn = Instantiate(additionalBoxPref,
                    previousTransform.position + previousTransform.transform.forward * zBox + previousTransform.transform.right * Random.Range(-levelWidth / 2 + (levelWidth / 6), levelWidth / 2 - (levelWidth / 6)) + Vector3.up * boxHeight,
                    Quaternion.identity);
                spawn.transform.parent = level.transform;

                zBox += distanceBetweenBox;
            }

            while (zOffset < maxZ)
            {
                int q = 0;
                bool inLoop = true;
                do
                {
                    q++;
                    Vector3 pos = previousTransform.position + previousTransform.transform.forward * zOffset + Vector3.up * boxHeight;

                    if (Physics.OverlapBox(pos, sizeOverlapBox, previousTransform.transform.rotation).Length == 0)
                    {
                        spawn = Instantiate(obstaclePrefs[Random.Range(0, obstaclePrefs.Count)],
                            pos,
                            previousTransform.rotation);
                        spawn.transform.parent = level.transform;

                        zOffset += distanceBetweenObstacle;
                        inLoop = false;
                        break;
                    }
                    else
                    {
                        Debug.Log("The place is taken");
                        zOffset += 1;
                    }

                    if (q > (int)roadLength) break;
                }
                while (inLoop);
            }

            while (zCoin < maxZ)
            {
                int q = 0;
                bool inLoop = true;
                do
                {
                    q++;
                    Vector3 pos = previousTransform.position + previousTransform.transform.forward * zCoin + previousTransform.transform.right * Random.Range(-levelWidth / 2 + (levelWidth / 6), levelWidth / 2 - (levelWidth / 6)) + Vector3.up * boxHeight;

                    if (Physics.OverlapBox(pos, sizeOverlapBox, previousTransform.transform.rotation).Length == 0)
                    {
                        spawn = Instantiate(coinPref,
                            pos,
                            Quaternion.identity);
                        spawn.transform.parent = level.transform;

                        zCoin += distanceBetweenCoin;
                        inLoop = false;
                        break;
                    }
                    else
                    {
                        Debug.Log("The place is taken");
                        zCoin += 2;
                    }

                    if (q > (int)roadLength) break;
                }
                while (inLoop);
            }
        }

        GameObject finis = Instantiate(finishPref, 
            previousTransform.position + (previousTransform.forward * roadLength) - (previousTransform.forward * roadLength / 8), 
            previousTransform.rotation);
        finis.transform.localScale = new Vector3(levelWidth, finis.transform.localScale.y, finis.transform.localScale.z);
        finis.transform.parent = level.transform;

        GameObject player = Instantiate(playerPref, level.transform);
        player.transform.position = new Vector3(0, 2, 2);
        CameraController.instance.SetTarget(player.transform);

        notReadyForGame = false;
    }

    Vector3 GetNewRoadPos(Transform previous, float length)
    {
        Vector3 temp = previous.position;
        temp += previous.transform.forward * length;
        
        temp.x += levelWidth / 2;
        temp.y -= 0.01f;
        temp.z -= levelWidth / 2;

        return temp;
    }

    void RotateRoadObject(Transform objTransform, bool right)
    {
        if (right)
        {
            angle += 90;
        }
        else
        {
            angle -= 90;
        }

        objTransform.rotation = Quaternion.Euler(0, objTransform.rotation.eulerAngles.y + angle, 0);
    }

    public void SetBoxMaterial(bool isRandom = true, Material material = null)
    {
        Material tempMaterial;

        if (isRandom)
        {
            tempMaterial = playerBoxMaterials[Random.Range(0, playerBoxMaterials.Count)];
        }
        else
        {
            if (material == null)
            {
                Debug.LogError("You need to specify the material.");
                return;
            }
            tempMaterial = material;
        }

        currentMaterial = tempMaterial;
    }

    void TestCube(Vector3 pos, Quaternion rot, Vector3 size)
    {
        GameObject sda = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sda.transform.position = pos;
        sda.transform.rotation = rot;
        sda.transform.localScale = size;
    }
}

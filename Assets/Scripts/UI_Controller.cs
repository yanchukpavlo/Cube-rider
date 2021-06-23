using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Controller : MonoBehaviour
{
    public static UI_Controller instance;

    [Header("Panels")]
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject gamePanel;
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject endPanel;

    [Header("Other")]
    [SerializeField] GameObject joystick;
    [SerializeField] TMP_Text coinText;

    GameObject curentPanel;
    int coin;
    public int Coin { get { return coin; } }

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
        curentPanel = menuPanel;
        curentPanel.SetActive(true);

        EventsManager.instance.onChangeStateTrigger += ChangeStateTrigger;
        EventsManager.instance.onAddCoinTrigger += AddCoinTrigger;
    }

    private void OnDestroy()
    {
        EventsManager.instance.onChangeStateTrigger -= ChangeStateTrigger;
        EventsManager.instance.onAddCoinTrigger -= AddCoinTrigger;
    }

    private void AddCoinTrigger(int x)
    {
        coin += x;
        UpdateCoinText();
    }

    private void ChangeStateTrigger(EventsManager.GameState state)
    {
        curentPanel.SetActive(false);

        switch (state)
        {
            case EventsManager.GameState.Menu:
                curentPanel = menuPanel;
                break;

            case EventsManager.GameState.Play:
                coin = 0;
                UpdateCoinText();
                curentPanel = gamePanel;
                break;

            case EventsManager.GameState.Win:
                curentPanel = winPanel;
                break;

            case EventsManager.GameState.Dead:
                curentPanel = endPanel;
                break;

            default:
                break;
        }

        curentPanel.SetActive(true);
    }

    void UpdateCoinText()
    {
        coinText.text = coin.ToString();
    }

    public void ButtonStart()
    {
        EventsManager.instance.ChangeStateTrigger(EventsManager.GameState.Play);
    }

    #region Test

    public void AddBoxToPlayer()
    {
        EventsManager.instance.AddBoxTrigger(1);
    }

    public void GenerateLevel()
    {
        GameManager.instance.GenerateLevel();
    }

    #endregion
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsManager : MonoBehaviour
{
    public static EventsManager instance;

    public enum GameState
    {
        Menu,
        Play,
        Win,
        Dead
    }

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

    public event Action<GameState> onChangeStateTrigger;
    public void ChangeStateTrigger(GameState state)
    {
        onChangeStateTrigger?.Invoke(state);
    }

    public event Action<int> onAddBoxTrigger;
    public void AddBoxTrigger(int amount)
    {
        onAddBoxTrigger?.Invoke(amount);
    }

    public event Action<int> onAddCoinTrigger;
    public void AddCoinTrigger(int amount)
    {
        onAddCoinTrigger?.Invoke(amount);
    }
}
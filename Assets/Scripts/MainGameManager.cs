using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    public static MainGameManager Instance;
    [HideInInspector] public NetworkHandler NetworkHandlerInstance;
    [HideInInspector] public string TeamName;
    [HideInInspector] public int Score;

    private ConcurrentQueue<Action> CallbackQueue = new ConcurrentQueue<Action>();

    [Header("Fields")] public TMP_InputField AuthenticationInputField;

    [Space] public GameObject PauseScreen;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        NetworkHandlerInstance = new NetworkHandler();

        StartCoroutine(CallbackQueueRoutine());
    }

    public void AuthenticateTeam()
    {
        if (AuthenticationInputField.text == "" || string.IsNullOrWhiteSpace(AuthenticationInputField.text))
        {
            Debug.LogError("No team name");
            return;
        }

        NetworkHandlerInstance.SendAuthentication(AuthenticationInputField.text, () =>
        {
            CallbackQueue.Enqueue(() =>
            {
                TeamName = AuthenticationInputField.text;
                NavigationManager.Instance.ActivateInitalScreen();
            });
            print("Callback received");
        });
    }

    public void AddToCallbackQueue(Action method)
    {
        CallbackQueue.Enqueue(method);
    }

    public void ToggleGame(bool isPaused)
    {
        //Pause timer
        //show screen
        if (isPaused)
        {
            PauseScreen.SetActive(true);
        }
        else
        {
            PauseScreen.SetActive(false);
        }
    }

    IEnumerator CallbackQueueRoutine()
    {
        //add loop to handle all commands at once
        while (true)
        {
            yield return new WaitForSeconds(2);

            if (!CallbackQueue.IsEmpty)
            {
                Action method;

                if (CallbackQueue.TryDequeue(out method))
                {
                    method.Invoke();
                }
            }
        }
    }

    private void OnDestroy()
    {
        NetworkHandlerInstance.Dispose();
    }
}
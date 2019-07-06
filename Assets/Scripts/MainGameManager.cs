using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainGameManager : MonoBehaviour
{
    public const int MAXIMUM_TIME = 3600;
    
    public static MainGameManager Instance;
    [HideInInspector] public NetworkHandler NetworkHandlerInstance;
    [HideInInspector] public string TeamName;
    [HideInInspector] public int Score;

    private ConcurrentQueue<Action> CallbackQueue = new ConcurrentQueue<Action>();

    [Header("Fields")] public TMP_InputField AuthenticationInputField;
    
    [Space] public GameObject PauseScreen;
    public bool IsDebug;

    [HideInInspector]public float ElapsedTime;
    private int previousMinute;
    private bool timerStarted;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        NetworkHandlerInstance = new NetworkHandler(IsDebug);
        Score = 1500;
        StartCoroutine(CallbackQueueRoutine());
    }

    private void Update()
    {
        if (timerStarted)
        {
            if (ElapsedTime < MAXIMUM_TIME)
            {
                //save min
                ElapsedTime += Time.deltaTime;
                var mins = Utilities.GetMinutes((int)ElapsedTime);

                if (mins > previousMinute)
                {
                    previousMinute = mins;
                    UpdatePoints(-25);
                }
            }
            else
            {
                timerStarted = false;
                NavigationManager.Instance.ActivateGameEndScreen();
                print("GAME END");
                DialogManager.Instance.DisableDialogue();
            }
        }
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
            timerStarted = true;
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

    public void UpdatePoints(int points)
    {
        Score += points;
        NetworkHandlerInstance.SendPointsUpdate(TeamName, Score);
    }

    IEnumerator CallbackQueueRoutine()
    {
        //add loop to handle all commands at once
        while (true)
        {
            yield return new WaitForSeconds(1);

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

    public void ResetGame()
    {
        SceneManager.LoadScene(0);
    }
}
﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
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

    [HideInInspector] public float ElapsedTime;
    private int previousMinute;
    private int previousSecond;
    private bool timerStarted;
    private bool gamePaused;

    [HideInInspector] public bool GameEnded;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        NetworkHandlerInstance = new NetworkHandler(IsDebug);
        Score = 1500;
        StartCoroutine(CallbackQueueRoutine());
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

    }

    private void Update()
    {
        NetworkHandlerInstance.Poll();

        if (GameEnded)
            return;

        if (timerStarted && !gamePaused)
        {
            if (ElapsedTime < MAXIMUM_TIME)
            {
                //save min
                ElapsedTime += Time.deltaTime;
                var mins = Utilities.GetMinutes((int)ElapsedTime);

                if (mins > previousMinute)
                {
                    previousMinute = mins;

                    if (ElapsedTime < MAXIMUM_TIME)
                        StartCoroutine(SendPointUpdateWithDelay(-25));
                    else
                        Score -= 25;
                }

                if (Mathf.FloorToInt(ElapsedTime) > previousSecond)
                {
                    previousSecond = Mathf.FloorToInt(ElapsedTime);
                    NetworkHandlerInstance.SendTimerHeartBeat(TeamName, previousSecond);
                }
            }
            else
            {
                //UpdatePoints(-25, false);
                timerStarted = false;
                AudioManager.Instance.PlayAudio(12);
                NavigationManager.Instance.ActivateGameEndScreen();
                print("GAME END");
                DialogManager.Instance.DisableDialogue();
            }
        }
    }

    IEnumerator SendPointUpdateWithDelay(int points)
    {
        yield return new WaitForSeconds(0.1f);
        UpdatePoints(points, false);
        yield return null;
    }

    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(3);
        UnityWebRequest req = UnityWebRequest.Get(new Uri("http://192.168.1.17:14999/start"));
        req.SendWebRequest();
        timerStarted = true;
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
                StartCoroutine(StartDelay());
                print("Callback received");
            });
        });
    }

    public void AddToCallbackQueue(Action method)
    {
        CallbackQueue.Enqueue(method);
    }

    public void ToggleGame(bool isPaused)
    {
        gamePaused = isPaused;

        if (isPaused)
        {
            PauseScreen.SetActive(true);
        }
        else
        {
            PauseScreen.SetActive(false);
        }
    }

    public void UpdatePoints(int points, bool isHidden)
    {
        Score += points;
        NetworkHandlerInstance.SendPointsUpdate(TeamName, points, isHidden);
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

    private void OnApplicationQuit()
    {
        NetworkHandlerInstance.Dispose();
    }

    public void ResetGame()
    {
        NetworkHandlerInstance.Dispose();
        Instance = null;
        SceneManager.LoadScene(0);
    }
}
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    public static HintManager Instance;
    public List<GameObject> HintStars;
    private int hintCount;

    private void Awake()
    {
        Instance = this;
    }

    public void SendHintRequest()
    {
        if (hintCount >= 3)
        {
            DialogManager.Instance.EnableDialogue("Hint Request", "You have exceeded your maximum hint requests. Additional hint will deduct 50 points from your score. Press OK to continue to request a hint, or press outside this dialog to return back.",
                "OK", true, () =>
                {
                    var Score = MainGameManager.Instance.Score;
                    Score -= 50;
                    MainGameManager.Instance.Score = Score;
                    MainGameManager.Instance.NetworkHandlerInstance.SendPointsUpdate(MainGameManager.Instance.TeamName, Score);
                    DialogManager.Instance.DisableDialogue();
                });
        }
        else
        {
            HintStars[hintCount].SetActive(false);
        }

        hintCount++;
        MainGameManager.Instance.NetworkHandlerInstance.SendHintRequest(MainGameManager.Instance.TeamName);
    }
}

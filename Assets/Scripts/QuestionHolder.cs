﻿using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class QuestionHolder : MonoBehaviour
{
    [Header("Question Bodies")] public List<GameObject> QuestionBodies;
    public List<Button> FinalButtons;
    private int currentActiveBodyId = -1;

    /// Deprecated
    public void SelectQuestion(int id)
    {
        return;
        if (currentActiveBodyId == id)
        {
            MinimizeBody(id);
            currentActiveBodyId = -1;
        }
        else
        {
            if (currentActiveBodyId != -1)
            {
                MinimizeBody(currentActiveBodyId);
            }
            
            MaximizeBody(id);
            currentActiveBodyId = id;
        }
    }

    void MinimizeBody(int id)
    {
        var element = QuestionBodies[id].GetComponent<RectTransform>();
        element.DOSizeDelta(new Vector2(element.sizeDelta.x, 0), 0.5f);
    }

    void MaximizeBody(int id)
    {
        var element = QuestionBodies[id].GetComponent<RectTransform>();
        element.DOSizeDelta(new Vector2(element.sizeDelta.x, 254), 0.5f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public List<QuestionHolder> QuestionHolderList;
    public GameObject QrHolder;
    
    private int currentSelectedLevel;

    public void SelectLevel(int levelIndex)
    {
        if(levelIndex == currentSelectedLevel)
            return;
        
        if(currentSelectedLevel == -1)
            QrHolder.SetActive(false);
        else
            QuestionHolderList[currentSelectedLevel].gameObject.SetActive(false);
        
        currentSelectedLevel = levelIndex;
        QuestionHolderList[currentSelectedLevel].gameObject.SetActive(true);
    }

    public void SelectQr()
    {
        QuestionHolderList[currentSelectedLevel].gameObject.SetActive(false);
        currentSelectedLevel = -1;
        QrHolder.SetActive(true);
    }
}

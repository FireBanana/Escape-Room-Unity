using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public List<QuestionHolder> QuestionHolderList;
    public List<Button> LevelButtons;
    public GameObject QrHolder;
    
    private int currentSelectedLevel = -2; //None

    public void SelectLevel(int levelIndex)
    {
        if(levelIndex == currentSelectedLevel)
            return;

        if (currentSelectedLevel == -1)
        {
            QrHolder.SetActive(false);
            NavigationManager.Instance.DeactivateQr();
        }
        else
            QuestionHolderList[currentSelectedLevel].gameObject.SetActive(false);
        
        if (levelIndex == -2)
        {
            return;
        }
        
        currentSelectedLevel = levelIndex;
        QuestionHolderList[currentSelectedLevel].gameObject.SetActive(true);
    }

    public void UnlockLevel(int level)
    {
        if (level == 6)
        {
            //DialogManager.Instance.EnableDialogue("Simulation Terminated", "", "OK", DialogManager.Instance.DisableDialogue);
            return;
        }
        
        LevelButtons[level].interactable = true;
        DialogManager.Instance.EnableDialogue("Level Unlocked!", "You have unlocked a new level!", "OK", true, DialogManager.Instance.DisableDialogue);
    }

    public void SelectQr()
    {
        if (MainGameManager.Instance.IsDebug)
        {
            //Success code
            return;
        }
        
        if(currentSelectedLevel != -2)
            QuestionHolderList[currentSelectedLevel].gameObject.SetActive(false);
        currentSelectedLevel = -1;
        QrHolder.SetActive(true);
        QrManager.Instance.QrDecoder.StartWork();
    }

    public void DeselectQr()
    {
        QrHolder.SetActive(false);
        currentSelectedLevel = -2;
    }
}

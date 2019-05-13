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
    private bool firstTimeSelected;

    public void SelectLevel(int levelIndex)
    {
        if(levelIndex == currentSelectedLevel)
            return;

        if (levelIndex == 0 && !firstTimeSelected)
        {
            firstTimeSelected = true;
            DialogManager.Instance.EnableDialogue("Super Duper",
                "Each level has 5 question. Get the answer correct and you will be rewarded 100 points. Get the answer wrong and you will lose 200 points.\n\nAnswer the fifth question correctly in the current level to unlock the next level",
                "Got It", false, () =>
                {
                    DialogManager.Instance.DisableDialogue();
                    DialogManager.Instance.EnableDialogue("Finally...",
                        "If you need them, your team will be provided three free hints if you get stuck in the simulation. These are represented by the stars below.\n\nUse all three, and each additional hint you request will cost your team 50 points.\n\nTo use a hint, click the \"Hint\" button, then wait for the hintbot to display the \"Ready for Hint\" text on your simulation scoreboard located above the window.\n\nThen clearly speak your question.\n\nFor non-simulation assistance, click the \"Help\" button.",
                        "Sheesh, is that it?", false, () =>
                        {
                            DialogManager.Instance.DisableDialogue();
                            DialogManager.Instance.EnableDialogue("Holy Mackerel",
                                "You sure are itchin' to go at it, huh?\n\nOkay then, you should probably start by turning on the power in the cabin. You can't collect the data in the dark...",
                                "Thanks Captain Obvious", true, DialogManager.Instance.DisableDialogue);
                        });
                });
        }

        if (currentSelectedLevel == -1)
        {
            QrHolder.SetActive(false);
            QrManager.Instance.QrDecoder.StopWork();
            NavigationManager.Instance.DeactivateQr();
        }
        else if(currentSelectedLevel >= 0)
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
        
        if(currentSelectedLevel >= 0)
            QuestionHolderList[currentSelectedLevel].gameObject.SetActive(false);
        currentSelectedLevel = -1;
        QrHolder.SetActive(true);
        QrManager.Instance.QrDecoder.StartWork();
    }

    public void DeselectQr()
    {
        QrHolder.SetActive(false);
    }
}

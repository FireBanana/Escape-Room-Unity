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

    public QuestionHolder QuestionHolderRef;
    public List<Button> LevelButtons;
    public GameObject QrHolder;
    public GameObject QrMask;
    
    private int currentSelectedLevel = -2; //None
    private bool firstTimeSelected;
    
    List<int> openedLevel = new List<int>();

    public void SelectLevel(int levelIndex) //NOTE: Removed voice from here
    {
        if(levelIndex == currentSelectedLevel)
            return;

        if (levelIndex == 0 && !firstTimeSelected)
            firstTimeSelected = true;

        if (currentSelectedLevel == -1)
        {
            QrHolder.SetActive(false);
            QrManager.Instance.QrDecoder.StopWork();
            NavigationManager.Instance.DeactivateQr();
        }
        else if(currentSelectedLevel >= 0)
            QuestionHolderRef.gameObject.SetActive(false);
        
        if (levelIndex == -2)
        {
            return;
        }
        
        currentSelectedLevel = levelIndex;
        QuestionHolderRef.gameObject.SetActive(true);
    }

    //public void UnlockLevel(int level)
    //{
    //    if(openedLevel.Contains(level))
    //        return;
        
    //    openedLevel.Add(level);
        
    //    if (level == 5)
    //    {
    //        //DialogManager.Instance.EnableDialogue("Simulation Terminated", "", "OK", DialogManager.Instance.DisableDialogue);
    //        return;
    //    }
        
    //    AudioManager.Instance.PlayAudio(7);
    //    LevelButtons[level].interactable = true;
    //    DialogManager.Instance.EnableDialogue("Level Unlocked!", "You have unlocked a new level!", "OK", true, DialogManager.Instance.DisableDialogue);
    //}

    public void SelectQr()
    {
        if (MainGameManager.Instance.IsDebug)
        {
            //Success code
            return;
        }
        
        if(currentSelectedLevel >= 0)
            QuestionHolderRef.gameObject.SetActive(false);
        currentSelectedLevel = -1;
        QrMask.SetActive(true);
        QrHolder.SetActive(true);
        QrManager.Instance.QrDecoder.StartWork();
    }

    public void DeselectQr()
    {
        QrMask.SetActive(false);
        QrHolder.SetActive(false);
    }
}

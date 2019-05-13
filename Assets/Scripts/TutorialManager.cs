using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void DoorScanCompleteHelp()
    {
        DialogManager.Instance.EnableDialogue("Excellent!",
            "You just scored 100 points for scanning your first QR Code!\n\nAnytime you find a QR, click on the \"Scan QR\" tab to scan the score.\n\nNow click Level 1 to start collecting data.",
            "OK", true, DialogManager.Instance.DisableDialogue);
    }

    public void LevelClickedHelp()
    {
        
    }

    public void ShowQrFailed()
    {
        DialogManager.Instance.EnableDialogue("Thats the wrong QR Code",
            "You need to play the correct tone on the keyboard provided.",
            "OK", false, DialogManager.Instance.DisableDialogue);
    }

    void ShowHintHelp()
    {
        
    }

    void ShowFinalHelp()
    {
        
    }
}

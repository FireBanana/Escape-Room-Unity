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
            "OK", DialogManager.Instance.DisableDialogue);
    }

    public void LevelClickedHelp()
    {
        DialogManager.Instance.EnableDialogue("Super Duper",
            "Each level has 5 question. Get the answer correct and you will be rewarded 100 points. Get the answer wrong and you will lose 200 points.\n\nAnswer the fifth question correctly in the current level to unlock the next level",
            "Got It", () =>
            {
                DialogManager.Instance.DisableDialogue();
                ShowHintHelp();
            });
    }

    public void ShowQrFailed()
    {
        DialogManager.Instance.EnableDialogue("Thats the wrong QR Code",
            "You need to play the correct tone on the keyboard provided.",
            "OK", DialogManager.Instance.DisableDialogue);
    }

    void ShowHintHelp()
    {
        DialogManager.Instance.EnableDialogue("Finally...",
            "If you need them, your team will be provided three free hints if you get stuck in the simulation. These are represented by the stars below.\n\nUse all three, and each additional hint you request will cost your team 50 points.\n\nTo use a hint, click the \"Hint\" button, then wait for the hintbot to display the \"Ready for Hint\" text on your simulation scoreboard located above the window.\n\nThen clearly speak your question.\n\nFor non-simulation assistance, click the \"Help\" button.",
            "Sheesh, is that it?",  () =>
            {
                DialogManager.Instance.DisableDialogue();
                ShowFinalHelp();
            });
    }

    void ShowFinalHelp()
    {
        DialogManager.Instance.EnableDialogue("Holy Mackerel",
            "You sure are itchin' to go at it, huh?\n\nOkay then, you should probably start by turning on the power in the cabin. You can't collect the data in the dark...",
            "Thanks Captain Obvious", DialogManager.Instance.DisableDialogue);
    }
}

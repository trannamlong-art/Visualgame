using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public DialogueManager dialogueManager;
    private bool isBusy = false;
    [Header("UI References")]
    public GameObject TuongtacPanel; 
    public TMP_Text TuongtacText;    
    public GameObject dialogueBox;
    public GameObject DirangoaiPanel;
    public TMP_Text DirangoaiText;   
    public GameObject TuidoPanel;


    [Header("Dialogue Settings")]
    public string TuongtacDialogue = "Nên nói chuyện gì với Yuzuha đây nhỉ?";
    public string DirangoaiDialogue = "Tôi nên đi đâu?";

    public void OnTuongTacButtonClicked()
    { 
        if (isBusy) return;
        bool isActive = TuongtacPanel.activeSelf; 

        TuongtacPanel.SetActive(!isActive);

        if (!isActive)
        {
            TuongtacText.text = TuongtacDialogue;
            ShowDialogueBox();

            DirangoaiPanel.SetActive(false);
            TuidoPanel.SetActive(false);
        }
        else
        {
            dialogueBox.SetActive(false);
        }
    }

    public void OnNoiChuyenButtonClicked(string action)
    {
        if (action == "NoiChuyen")
        {
            var data = new DialogueManager.DialogueData
            {
                lines = new List<DialogueManager.DialogueLine>()
                {
                    new DialogueManager.DialogueLine { speaker = "Yuzuha", text = "Chào buổi sáng, cậu dậy sớm nhỉ!" },
                    new DialogueManager.DialogueLine { speaker = "Người chơi", text = "Ừ, hôm nay trời đẹp mà." },
                    new DialogueManager.DialogueLine { speaker = "Yuzuha", text = "Cậu muốn đi dạo không?" }
                }
            };

            dialogueManager.StartDialogue(data);
        }
    }

    public void OnDirangoaiButtonClicked()
    {
        if (isBusy) return;
        bool isActive = DirangoaiPanel.activeSelf;

        DirangoaiPanel.SetActive(!isActive);

        if(!isActive)
        {
            DirangoaiText.text = DirangoaiDialogue;
            ShowDialogueBox();

            TuongtacPanel.SetActive(false);
            TuidoPanel.SetActive(false);
        }
        else
        {
            dialogueBox.SetActive(false);
        }
    }

    public void OnTuidoButtonClicked()
    {
        if (isBusy) return;
        bool isActive = TuidoPanel.activeSelf;

        TuidoPanel.SetActive(!isActive);

        if(!isActive)
        {
            dialogueBox.SetActive(false);
            TuongtacPanel.SetActive(false);
            DirangoaiPanel.SetActive(false);
        }
    }

    public void OnLamViecButtonClicked()
    {
        if (isBusy) return;
        isBusy = true;

        DirangoaiPanel.SetActive(false);
        TuidoPanel.SetActive(false);
        TuongtacPanel.SetActive(false);
        dialogueBox.SetActive(false);

        StartCoroutine(WaitRoutine());
    }

    public void OnNghiNgoiButtonClicked()
    {
        if (isBusy) return;
        isBusy = true;

        DirangoaiPanel.SetActive(false);
        TuidoPanel.SetActive(false);
        TuongtacPanel.SetActive(false);
        dialogueBox.SetActive(false);

        StartCoroutine(WaitRoutine());
    }

    private System.Collections.IEnumerator WaitRoutine()
    {
        yield return new WaitForSeconds(3f);
        isBusy = false;
    }
    void ShowDialogueBox()
    {
        dialogueBox.SetActive(true);
    }


}

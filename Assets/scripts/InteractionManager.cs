using UnityEngine;
using TMPro;

public class InteractionManager : MonoBehaviour
{
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
        bool isActive = TuongtacPanel.activeSelf; // kiểm tra đang bật hay tắt

        // Nếu đang bật thì tắt, nếu đang tắt thì bật và hiển thị chữ
        TuongtacPanel.SetActive(!isActive);

        // Nếu vừa bật thì hiển thị chữ và bật hộp thoại
        if (!isActive)
        {
            TuongtacText.text = TuongtacDialogue;
            ShowDialogueBox();

            // Tắt cái còn lại nếu đang mở
            DirangoaiPanel.SetActive(false);
            TuidoPanel.SetActive(false);
        }
        else
        {
            // Nếu tắt thì ẩn luôn khung hội thoại
            dialogueBox.SetActive(false);
        }
    }

    public void OnDirangoaiButtonClicked()
    {
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
        bool isActive = TuidoPanel.activeSelf;

        TuidoPanel.SetActive(!isActive);

        if(!isActive)
        {
            dialogueBox.SetActive(false);
        }
        else
        {
            TuongtacPanel.SetActive(false);
            DirangoaiPanel.SetActive(false);
        }
    }

    void ShowDialogueBox()
    {
        dialogueBox.SetActive(true);
    }
}

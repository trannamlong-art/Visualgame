using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speaker;
        public string text;
    }

    [System.Serializable]
    public class DialogueData
    {
        public List<DialogueLine> lines;
    }

    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public GameObject choicePanel;
    public TMP_Text choice1Text;
    public TMP_Text choice2Text;

    private DialogueData currentDialogue;
    private int currentIndex = 0;
    private bool isChoiceActive = false;

    void Start()
    {
        choicePanel.SetActive(false);
    }

    void Update()
    {
        // Khi người chơi bấm chuột trái hoặc Enter → chuyển câu tiếp theo
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return))
        {
            ShowNextLine();
        }
    }
    public void StartDialogue(DialogueData dialogue)
    {
        currentDialogue = dialogue;
        currentIndex = 0;
        ShowNextLine();
    }

    public void ShowNextLine()
    {
        if (isChoiceActive) return;

        if (currentIndex >= currentDialogue.lines.Count)
        {
            EndDialogue();
            return;
        }

        var line = currentDialogue.lines[currentIndex];
        nameText.text = line.speaker;
        dialogueText.text = line.text;
        currentIndex++;

        // Giả dụ sau câu thứ 3 sẽ hiện lựa chọn
        if (currentIndex == 3)
        {
            ShowChoices();
        }
    }

    void ShowChoices()
    {
        isChoiceActive = true;
        choicePanel.SetActive(true);
        choice1Text.text = "Khen cô ấy";
        choice2Text.text = "Im lặng";
    }

    public void OnChoiceSelected(int choice)
    {
        isChoiceActive = false;
        choicePanel.SetActive(false);

        if (choice == 1)
        {
            dialogueText.text = "Yuzuha mỉm cười, độ thiện cảm tăng!";
            //FindObjectOfType<Tainguyen>().TangThienCam(5);
        }
        else
        {
            dialogueText.text = "Không khí trở nên ngượng ngùng...";
        }
    }

    void EndDialogue()
    {
        dialogueText.text = "(Kết thúc hội thoại)";
    }
}

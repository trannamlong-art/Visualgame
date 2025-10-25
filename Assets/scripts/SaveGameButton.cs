using UnityEngine;
using UnityEngine.UI;

public class SaveGameButton : MonoBehaviour
{
    [Header("Nút save game")]
    public Button saveButton;

    private void Start()
    {
        if (saveButton != null)
        {
            saveButton.onClick.AddListener(SaveGame);
        }
        else
        {
            Debug.LogWarning("⚠️ Chưa gán Button trong SaveGameButton!");
        }
    }

    private void SaveGame()
    {
        if (GameDataManager.instance != null)
        {
            GameDataManager.instance.SaveGame();
            Debug.Log("💾 Người chơi đã nhấn Save → tất cả chỉ số đã lưu!");
        }
        else
        {
            Debug.LogWarning("⚠️ Không tìm thấy GameDataManager để lưu dữ liệu!");
        }
    }
}

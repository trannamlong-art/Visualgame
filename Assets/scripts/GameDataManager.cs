using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance;

    [Header("Dữ liệu game")]
    public int thienCam;
    public int tien;
    public int nangLuong;
    public int ngay;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            LoadGame(); // Load dữ liệu khi khởi động
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt("ThienCam", thienCam);
        PlayerPrefs.SetInt("Tien", tien);
        PlayerPrefs.SetInt("NangLuong", nangLuong);
        PlayerPrefs.SetInt("Ngay", ngay);
        PlayerPrefs.Save();

        Debug.Log($"✅ Đã lưu game: Thiện cảm={thienCam}, Tiền={tien}, Năng lượng={nangLuong}, Ngày={ngay}");
    }

    public void LoadGame()
    {
        thienCam = PlayerPrefs.GetInt("ThienCam", 0);
        tien = PlayerPrefs.GetInt("Tien", 0);
        nangLuong = PlayerPrefs.GetInt("NangLuong", 100);
        ngay = PlayerPrefs.GetInt("Ngay", 1);

        Debug.Log("📥 Đã load dữ liệu game từ PlayerPrefs");
    }

    public void ResetDataMacDinh()
    {
        thienCam = 0;
        tien = 0;
        nangLuong = 100;
        ngay = 1;

        SaveGame(); // Lưu luôn vào PlayerPrefs

        Debug.Log("🔄 Dữ liệu game đã reset về mặc định.");
    }
}

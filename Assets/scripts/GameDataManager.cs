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

    private bool daChuyenNgay2 = false;
    private bool daChuyenNgay5 = false;

    private void Awake()
    {
        instance = this;

        
        ResetDataMacDinh();
        //LoadGame();

        // 🔁 Tự động load dữ liệu mỗi khi scene được load lại
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadGame();
        Debug.Log($"📥 Dữ liệu đã được load lại khi vào scene: {scene.name}");
    }

    void Update()
    {
        if (ngay == 2 && !daChuyenNgay2)
        {
            daChuyenNgay2 = true;
            PlayerPrefs.SetInt("DaChuyenNgay2", 1); // ✅ lưu trạng thái
            SaveGame();
            SceneManager.LoadScene("Ngày 2");
        }

        if (ngay == 5 && !daChuyenNgay5)
        {
            daChuyenNgay5 = true;
            PlayerPrefs.SetInt("DaChuyenNgay5", 1); // ✅ lưu trạng thái
            SaveGame();
            SceneManager.LoadScene("Ngày 5");
        }
    }


    public void SaveGame()
    {
        PlayerPrefs.SetInt("ThienCam", thienCam);
        PlayerPrefs.SetInt("Tien", tien);
        PlayerPrefs.SetInt("NangLuong", nangLuong);
        PlayerPrefs.SetInt("Ngay", ngay);
        PlayerPrefs.Save();

        Debug.Log($"💾 Lưu game: Thiện cảm={thienCam}, Tiền={tien}, Năng lượng={nangLuong}, Ngày={ngay}");
    }

    public void LoadGame()
    {
        thienCam = PlayerPrefs.GetInt("ThienCam", 0);
        tien = PlayerPrefs.GetInt("Tien", 0);
        nangLuong = PlayerPrefs.GetInt("NangLuong", 100);
        ngay = PlayerPrefs.GetInt("Ngay", 1);

        daChuyenNgay2 = PlayerPrefs.GetInt("DaChuyenNgay2", 0) == 1;
        daChuyenNgay5 = PlayerPrefs.GetInt("DaChuyenNgay5", 0) == 1;

        Debug.Log($"📥 Load dữ liệu: Thiện cảm={thienCam}, Tiền={tien}, Năng lượng={nangLuong}, Ngày={ngay}");
    }

    public void ResetDataMacDinh()
    {
        thienCam = 0;
        tien = 0;
        nangLuong = 100;
        ngay = 1;
        SaveGame();

        

        Debug.Log("🔄 Dữ liệu game đã reset về mặc định.");
    }
}

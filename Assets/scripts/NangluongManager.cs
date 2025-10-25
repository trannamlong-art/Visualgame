using UnityEngine;
using TMPro;

public class NangLuongManager : MonoBehaviour
{
    public static NangLuongManager instance;

    [Header("Chỉ số năng lượng")]
    public int nangLuongToiDa = 100;
    public int nangLuongHienTai = 100;

    [Header("Hiển thị UI")]
    public TMP_Text nangLuongText;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 🔹 Reset năng lượng khi vào Play Mode
        nangLuongHienTai = nangLuongToiDa;
        CapNhatDuLieuChinh();
    }

    void Start()
    {
        // Đồng bộ dữ liệu từ GameDataManager nếu có
        if (GameDataManager.instance != null)
            nangLuongHienTai = GameDataManager.instance.nangLuong;

        CapNhatNangLuongUI();
    }

    public void TruNangLuong(int soLuong)
    {
        nangLuongHienTai -= soLuong;
        if (nangLuongHienTai < 0) nangLuongHienTai = 0;

        CapNhatNangLuongUI();
        CapNhatDuLieuChinh();
    }

    public void HoiNangLuong(int soLuong)
    {
        nangLuongHienTai += soLuong;
        if (nangLuongHienTai > nangLuongToiDa)
            nangLuongHienTai = nangLuongToiDa;

        CapNhatNangLuongUI();
        CapNhatDuLieuChinh();
    }

    public bool DaDayNangLuong() => nangLuongHienTai >= nangLuongToiDa;

    void CapNhatNangLuongUI()
    {
        if (nangLuongText != null)
            nangLuongText.text = $"Năng lượng: {nangLuongHienTai}/{nangLuongToiDa}";
    }

    void CapNhatDuLieuChinh()
    {
        if (GameDataManager.instance != null)
        {
            GameDataManager.instance.nangLuong = nangLuongHienTai;
            GameDataManager.instance.SaveGame();
        }
    }
}

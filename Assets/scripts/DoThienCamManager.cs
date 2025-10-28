using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class DoThienCamManager : MonoBehaviour
{
    public static DoThienCamManager instance;

    [Header("UI Hiển thị")]
    public TMP_Text thienCamText;
    public int maxThienCam = 100;

    [Header("Giá trị hiện tại")]
    public int thienCamHienTai = 0;

    [Header("Hiệu ứng + / - thiện cảm")]
    public GameObject hieuUngPrefab;
    public Transform canvasHieuUng;
    public Vector2 hieuUngOffset = new Vector2(0, 50f);
    public float quangDuongBay = 60f;
    public float thoiGianBay = 1f;

    private bool[] daKichHoatMoc = new bool[4]; // Để tránh kích hoạt lặp lại

    private void Awake()
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
    }

    void Start()
    {
        // Đồng bộ dữ liệu từ GameDataManager nếu có
        if (GameDataManager.instance != null)
            thienCamHienTai = GameDataManager.instance.thienCam;

        CapNhatUI();
    }

    public void TangThienCam(int soLuong)
    {
        if (soLuong <= 0) return;
        StartCoroutine(HienHieuUngVaCapNhat(soLuong, true));
    }

    public void GiamThienCam(int soLuong)
    {
        if (soLuong <= 0) return;
        StartCoroutine(HienHieuUngVaCapNhat(soLuong, false));
    }

    IEnumerator HienHieuUngVaCapNhat(int soLuong, bool laTang)
    {
        yield return StartCoroutine(HienHieuUngThienCam(
            (laTang ? "+" : "-") + soLuong.ToString(),
            laTang ? Color.magenta : Color.gray
        ));

        if (laTang)
        {
            thienCamHienTai += soLuong;
            if (thienCamHienTai > maxThienCam)
                thienCamHienTai = maxThienCam;
        }
        else
        {
            thienCamHienTai -= soLuong;
            if (thienCamHienTai < 0)
                thienCamHienTai = 0;
        }

        LuuLai();
        CapNhatUI();
        KiemTraMocThienCam();
    }

    IEnumerator HienHieuUngThienCam(string noiDung, Color mau)
    {
        if (hieuUngPrefab == null || canvasHieuUng == null || thienCamText == null)
            yield break;

        GameObject fx = Instantiate(hieuUngPrefab, canvasHieuUng);
        TMP_Text fxText = fx.GetComponent<TMP_Text>();

        fxText.text = noiDung;
        fxText.color = mau;

        RectTransform fxRT = fx.GetComponent<RectTransform>();
        RectTransform textRT = thienCamText.GetComponent<RectTransform>();

        fxRT.anchoredPosition = textRT.anchoredPosition + hieuUngOffset;

        Vector2 startPos = fxRT.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(0, quangDuongBay);

        float timer = 0f;
        Color c = fxText.color;

        while (timer < thoiGianBay)
        {
            timer += Time.deltaTime;
            float t = timer / thoiGianBay;
            fxRT.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            c.a = Mathf.Lerp(1f, 0f, t);
            fxText.color = c;
            yield return null;
        }

        Destroy(fx.gameObject);
    }

    void CapNhatUI()
    {
        if (thienCamText != null)
            thienCamText.text = $"Độ thiện cảm: {thienCamHienTai}/{maxThienCam}";
    }

    void LuuLai()
    {
        PlayerPrefs.SetInt("ThienCam", thienCamHienTai);
        PlayerPrefs.Save();
        if (GameDataManager.instance != null)
        {
            GameDataManager.instance.thienCam = thienCamHienTai;
            GameDataManager.instance.SaveGame(); // ✅ Tự động lưu mỗi khi cập nhật
        }
    }

    void KiemTraMocThienCam()
    {
        if (thienCamHienTai >= 25 && !daKichHoatMoc[0]) { daKichHoatMoc[0] = true; DiChuyenScene("Event25"); }
        else if (thienCamHienTai >= 50 && !daKichHoatMoc[1]) { daKichHoatMoc[1] = true; DiChuyenScene("Event50"); }
        else if (thienCamHienTai >= 75 && !daKichHoatMoc[2]) { daKichHoatMoc[2] = true; DiChuyenScene("Event75"); }
        else if (thienCamHienTai >= 100 && !daKichHoatMoc[3]) { daKichHoatMoc[3] = true; DiChuyenScene("Event100"); }
    }

    void DiChuyenScene(string tenScene)
    {
        if (GameDataManager.instance != null)
            GameDataManager.instance.SaveGame(); // ✅ Lưu toàn bộ dữ liệu trước khi chuyển

        SceneManager.LoadScene(tenScene);
    }
}

using UnityEngine;
using TMPro;
using System.Collections;

public class TienManager : MonoBehaviour
{
    public static TienManager instance;
    public int tienHienTai = 0;
    public TMP_Text tienText;

    [Header("Hiệu ứng + / - tiền")]
    public GameObject hieuUngPrefab;
    public Transform canvasHieuUng;

    [Header("Tuỳ chỉnh vị trí & chuyển động")]
    public Vector2 hieuUngOffset = new Vector2(0, 50f);
    public float quangDuongRoi = 60f;
    public float thoiGianRoi = 1.0f;

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

        // 🔹 Reset tiền khi vào Play Mode
        tienHienTai = 0;
        CapNhatDuLieuChinh();
    }

    void Start()
    {
        // Đồng bộ dữ liệu từ GameDataManager nếu có
        if (GameDataManager.instance != null)
            tienHienTai = GameDataManager.instance.tien;

        CapNhatTienUI();
    }

    public void CongTien(int soTien)
    {
        if (soTien <= 0) return;
        StartCoroutine(HienHieuUngVaCapNhat(soTien, true));
    }

    public void TruTien(int soTien)
    {
        if (soTien <= 0) return;
        StartCoroutine(HienHieuUngVaCapNhat(soTien, false));
    }

    IEnumerator HienHieuUngVaCapNhat(int soTien, bool laCong)
    {
        yield return StartCoroutine(HienHieuUngTien(
            (laCong ? "+" : "-") + (soTien / 1000f).ToString("0.#") + "K",
            laCong ? Color.green : Color.red
        ));

        tienHienTai += laCong ? soTien : -soTien;
        if (tienHienTai < 0) tienHienTai = 0;

        CapNhatTienUI();
        CapNhatDuLieuChinh();
    }

    void CapNhatTienUI()
    {
        if (tienText != null)
            tienText.text = tienHienTai.ToString("N0") + " VND";
    }

    void CapNhatDuLieuChinh()
    {
        if (GameDataManager.instance != null)
        {
            GameDataManager.instance.tien = tienHienTai;
            GameDataManager.instance.SaveGame();
        }
    }

    IEnumerator HienHieuUngTien(string noiDung, Color mau)
    {
        if (hieuUngPrefab == null || canvasHieuUng == null || tienText == null)
            yield break;

        GameObject fx = Instantiate(hieuUngPrefab, canvasHieuUng);
        TMP_Text fxText = fx.GetComponent<TMP_Text>();

        fxText.text = noiDung;
        fxText.color = mau;

        RectTransform fxRT = fx.GetComponent<RectTransform>();
        RectTransform textRT = tienText.GetComponent<RectTransform>();

        fxRT.anchoredPosition = textRT.anchoredPosition + hieuUngOffset;

        Vector2 startPos = fxRT.anchoredPosition;
        Vector2 endPos = startPos - new Vector2(0, quangDuongRoi);

        float timer = 0f;
        Color c = fxText.color;

        while (timer < thoiGianRoi)
        {
            timer += Time.deltaTime;
            float t = timer / thoiGianRoi;
            fxRT.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            c.a = Mathf.Lerp(1f, 0f, t);
            fxText.color = c;
            yield return null;
        }

        Destroy(fx.gameObject);
    }
}

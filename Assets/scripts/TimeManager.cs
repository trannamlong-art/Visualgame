using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;

    [Header("UI hiển thị")]
    public TMP_Text buoiText;       // Text hiển thị buổi
    public TMP_Text ngayText;       // Text hiển thị ngày
    public Image backgroundImage;   // Ảnh nền
    public GameObject fadePanel;    // Panel đen để làm hiệu ứng chuyển cảnh

    [Header("Ảnh nền từng buổi")]
    public Sprite sangBG, truaBG, chieuBG, toiBG;

    private int chiSoBuoi = 0; // 0 = sáng, 1 = trưa, 2 = chiều, 3 = tối
    private readonly string[] cacBuoi = { "Buổi sáng", "Buổi trưa", "Buổi chiều", "Buổi tối" };

    private Image fadeImage;
    private int ngayHienTai = 1;

    private void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

#if UNITY_EDITOR
            // 🔹 Reset ngày mỗi khi Play Mode để test
            ngayHienTai = 1;
            if (GameDataManager.instance != null)
                GameDataManager.instance.ngay = ngayHienTai;
#else
            // Load ngày từ GameDataManager nếu có
            if (GameDataManager.instance != null)
                ngayHienTai = GameDataManager.instance.ngay;
#endif
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (fadePanel != null)
            fadeImage = fadePanel.GetComponent<Image>();

        CapNhatBuoiNgayLapTuc();
        CapNhatNgayUI();

        if (fadePanel != null)
            fadePanel.SetActive(false);
    }

    public void NextTime()
    {
        StartCoroutine(ChuyenBuoiCoFade());
    }

    private IEnumerator ChuyenBuoiCoFade()
    {
        if (fadePanel == null)
        {
            Debug.LogWarning("⚠️ Chưa gán FadePanel trong TimeManager!");
            CapNhatBuoiNgayLapTuc();
            yield break;
        }

        fadePanel.SetActive(true);

        // Fade in (đen dần)
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime;
            SetFadeAlpha(time);
            yield return null;
        }

        // Chuyển buổi
        chiSoBuoi++;

        // Nếu vượt quá buổi tối → sáng ngày mới
        bool quaNgay = false;
        if (chiSoBuoi >= cacBuoi.Length)
        {
            chiSoBuoi = 0;
            ngayHienTai++;
            quaNgay = true;
        }

        CapNhatBuoiNgayLapTuc();

        // Nếu qua ngày mới
        if (quaNgay)
        {
            StartCoroutine(HienNgayMoi());
            HoiNangLuongKhiQuaNgay();
        }

        yield return new WaitForSeconds(0.5f);

        // Fade out (sáng dần)
        time = 1f;
        while (time > 0f)
        {
            time -= Time.deltaTime;
            SetFadeAlpha(time);
            yield return null;
        }

        fadePanel.SetActive(false);
    }

    private void SetFadeAlpha(float alpha)
    {
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = Mathf.Clamp01(alpha);
            fadeImage.color = c;
        }
    }

    private void CapNhatBuoiNgayLapTuc()
    {
        if (buoiText != null)
            buoiText.text = cacBuoi[chiSoBuoi];

        if (backgroundImage != null)
        {
            switch (chiSoBuoi)
            {
                case 0: backgroundImage.sprite = sangBG; break;
                case 1: backgroundImage.sprite = truaBG; break;
                case 2: backgroundImage.sprite = chieuBG; break;
                case 3: backgroundImage.sprite = toiBG; break;
            }
        }
    }

    private void CapNhatNgayUI()
    {
        if (ngayText != null)
            ngayText.text = $"Ngày {ngayHienTai}";

        // Đồng bộ ngày với GameDataManager
        if (GameDataManager.instance != null)
        {
            GameDataManager.instance.ngay = ngayHienTai;
            GameDataManager.instance.SaveGame();
        }
    }

    private IEnumerator HienNgayMoi()
    {
        CapNhatNgayUI();

        if (ngayText != null)
        {
            Color c = ngayText.color;

            // Sáng dần
            for (float t = 0; t <= 1; t += Time.deltaTime)
            {
                c.a = t;
                ngayText.color = c;
                yield return null;
            }

            yield return new WaitForSeconds(1.5f);

            // Mờ dần
            for (float t = 1; t >= 0; t -= Time.deltaTime)
            {
                c.a = t;
                ngayText.color = c;
                yield return null;
            }

            c.a = 1f;
            ngayText.color = c;
        }
    }

    private void HoiNangLuongKhiQuaNgay()
    {
        if (NangLuongManager.instance != null)
        {
            NangLuongManager.instance.HoiNangLuong(20);
            Debug.Log("💤 Qua ngày mới → hồi 20 năng lượng!");
        }
    }

    // 🔹 Hàm Save Game thủ công, có thể gắn vào Button
    public void SaveGameNow()
    {
        if (GameDataManager.instance != null)
        {
            GameDataManager.instance.SaveGame();
            Debug.Log("💾 Save game thủ công thành công!");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[System.Serializable]
public class CauThoai
{
    [Tooltip("Tên nhân vật (để trống nếu muốn ẩn khung)")]
    public string tenNhanVat;

    [TextArea(2, 6), Tooltip("Nội dung lời thoại (để trống nếu muốn ẩn khung)")]
    public string noiDung;

    [Tooltip("Ảnh nhân vật (cơ bản)")]
    public Sprite anhNhanVat;

    [Tooltip("Ảnh biểu cảm (đè lên ảnh nhân vật, có thể để trống)")]
    public Sprite anhBieuCam;

    [Tooltip("Ảnh nền cho câu thoại này (tùy chọn)")]
    public Sprite anhNenMoi;
}

public class LoiThoai : MonoBehaviour
{
    [Header("UI")]
    public GameObject khungThoai;
    public TMP_Text tenNhanVatText;
    public TMP_Text noiDungText;
    public Image anhNhanVatUI;
    public Image anhBieuCamUI; // <-- ảnh biểu cảm overlay
    public GameObject goiYTiepTuc; // "Bấm phím bất kỳ..."

    [Header("Background")]
    public Image anhNenHienTai;
    public float tocDoChuyenNen = 1f;

    [Header("Danh sách thoại (Inspector)")]
    public List<CauThoai> danhSachThoai = new List<CauThoai>();

    [Header("Typewriter")]
    [Range(0.005f, 0.2f)]
    public float tocDoChu = 0.03f;

    int chiSoHienTai = 0;
    Coroutine typingCoroutine = null;
    Coroutine bgCoroutine = null;
    Image bgOverlayImage = null;
    bool choPhepTiep = false;
    string currentFullText = "";

    void Start()
    {
        if (khungThoai != null)
            khungThoai.SetActive(false);

        if (goiYTiepTuc != null)
            goiYTiepTuc.SetActive(false);

        chiSoHienTai = 0;
        HienThiHienTai();
    }

    void Update()
    {
        bool anyKey = AnyKeyPressed();
        if (!anyKey) return;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
            noiDungText.text = currentFullText;
            choPhepTiep = true;
            if (goiYTiepTuc != null && khungThoai != null && khungThoai.activeSelf)
                goiYTiepTuc.SetActive(true);
            return;
        }

        if (choPhepTiep)
        {
            choPhepTiep = false;
            if (goiYTiepTuc != null)
                goiYTiepTuc.SetActive(false);

            chiSoHienTai++;
            HienThiHienTai();
        }
    }

    bool AnyKeyPressed()
    {
        bool pressed = false;
        try
        {
            if (Input.anyKeyDown)
                pressed = true;
        }
        catch (System.InvalidOperationException)
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
                pressed = true;
            if (!pressed && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                pressed = true;
            if (!pressed && Gamepad.current != null)
            {
                if (Gamepad.current.buttonSouth.wasPressedThisFrame ||
                    Gamepad.current.startButton.wasPressedThisFrame ||
                    Gamepad.current.buttonNorth.wasPressedThisFrame ||
                    Gamepad.current.buttonEast.wasPressedThisFrame ||
                    Gamepad.current.buttonWest.wasPressedThisFrame)
                    pressed = true;
            }
#endif
        }
        return pressed;
    }

    void HienThiHienTai()
    {
        if (chiSoHienTai < 0 || chiSoHienTai >= danhSachThoai.Count)
        {
            KetThuc();
            return;
        }

        CauThoai cau = danhSachThoai[chiSoHienTai];

        // Nếu cả tên và nội dung trống -> ẩn khung
        if (string.IsNullOrWhiteSpace(cau.tenNhanVat) && string.IsNullOrWhiteSpace(cau.noiDung))
        {
            if (khungThoai != null && khungThoai.activeSelf)
                khungThoai.SetActive(false);

            // Chuyển nền nếu có
            if (anhNenHienTai != null && cau.anhNenMoi != null)
                BatDauChuyenNen(cau.anhNenMoi);

            choPhepTiep = true;
            return;
        }

        // Hiện khung thoại
        if (khungThoai != null && !khungThoai.activeSelf)
            khungThoai.SetActive(true);

        // Hiện tên nhân vật
        if (tenNhanVatText != null)
            tenNhanVatText.text = cau.tenNhanVat ?? "";

        // Ảnh nhân vật chính
        if (anhNhanVatUI != null)
        {
            if (cau.anhNhanVat != null)
            {
                anhNhanVatUI.sprite = cau.anhNhanVat;
                anhNhanVatUI.enabled = true;
            }
            else
            {
                anhNhanVatUI.enabled = false;
            }
        }

        // Ảnh biểu cảm (đè lên)
        if (anhBieuCamUI != null)
        {
            if (cau.anhBieuCam != null)
            {
                anhBieuCamUI.sprite = cau.anhBieuCam;
                anhBieuCamUI.enabled = true;
            }
            else
            {
                anhBieuCamUI.enabled = false;
            }
        }

        // Chuyển nền nếu có
        if (anhNenHienTai != null && cau.anhNenMoi != null && anhNenHienTai.sprite != cau.anhNenMoi)
            BatDauChuyenNen(cau.anhNenMoi);

        // Hiện nội dung (typewriter)
        currentFullText = cau.noiDung ?? "";
        if (noiDungText != null)
            noiDungText.text = "";

        if (goiYTiepTuc != null)
            goiYTiepTuc.SetActive(false);

        choPhepTiep = false;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        typingCoroutine = StartCoroutine(Typewriter(currentFullText));
    }

    IEnumerator Typewriter(string text)
    {
        if (noiDungText != null)
            noiDungText.text = "";

        if (string.IsNullOrEmpty(text))
        {
            typingCoroutine = null;
            choPhepTiep = true;
            if (goiYTiepTuc != null)
                goiYTiepTuc.SetActive(true);
            yield break;
        }

        for (int i = 0; i < text.Length; i++)
        {
            noiDungText.text += text[i];
            yield return new WaitForSeconds(tocDoChu);
        }

        typingCoroutine = null;
        choPhepTiep = true;
        if (goiYTiepTuc != null)
            goiYTiepTuc.SetActive(true);
    }

    void BatDauChuyenNen(Sprite nenMoi)
    {
        if (bgCoroutine != null)
        {
            StopCoroutine(bgCoroutine);
            bgCoroutine = null;
        }
        bgCoroutine = StartCoroutine(FadeBackground(nenMoi));
    }

    IEnumerator FadeBackground(Sprite nenMoi)
    {
        if (anhNenHienTai == null || nenMoi == null)
            yield break;

        EnsureBgOverlay();

        bgOverlayImage.sprite = nenMoi;
        bgOverlayImage.color = new Color(1f, 1f, 1f, 0f);
        bgOverlayImage.enabled = true;

        float dur = Mathf.Max(0.0001f, tocDoChuyenNen);
        float t = 0f;
        Color baseColor = anhNenHienTai.color;

        while (t < dur)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / dur);
            anhNenHienTai.color = new Color(baseColor.r, baseColor.g, baseColor.b, 1f - p);
            bgOverlayImage.color = new Color(1f, 1f, 1f, p);
            yield return null;
        }

        anhNenHienTai.sprite = nenMoi;
        anhNenHienTai.color = baseColor;

        bgOverlayImage.enabled = false;
        bgCoroutine = null;
    }

    void EnsureBgOverlay()
    {
        if (bgOverlayImage != null)
            return;

        GameObject go = new GameObject("BG_Overlay");
        go.transform.SetParent(anhNenHienTai.transform.parent, false);
        Image img = go.AddComponent<Image>();

        RectTransform rtSrc = anhNenHienTai.rectTransform;
        RectTransform rt = img.rectTransform;

        // Giữ đúng vị trí và kích thước gốc
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.localScale = Vector3.one;

        img.preserveAspect = false; // ⚠️ Tắt preserveAspect để không bị bóp méo khi đổi sprite
        img.raycastTarget = false;
        img.enabled = false;

        bgOverlayImage = img;
    }


    void KetThuc()
    {
        if (khungThoai != null)
            khungThoai.SetActive(false);
        if (goiYTiepTuc != null)
            goiYTiepTuc.SetActive(false);
        // Kết thúc hội thoại - có thể thêm sự kiện nếu cần
    }
}

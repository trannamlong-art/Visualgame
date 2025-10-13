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
    public string tenNhanVat;
    [TextArea(2, 6)] public string noiDung;
    public Sprite anhNhanVat;
    public Sprite anhBieuCam;
    public Sprite anhNenMoi;
}

public class LoiThoai : MonoBehaviour
{
    [Header("UI")]
    public GameObject khungThoai;
    public TMP_Text tenNhanVatText;
    public TMP_Text noiDungText;
    public Image anhNhanVatUI;
    public Image anhBieuCamUI;
    public GameObject goiYTiepTuc;

    [Header("Background")]
    public Image anhNenHienTai;
    public float tocDoChuyenNen = 1f;

    [Header("Danh sách thoại")]
    public List<CauThoai> danhSachThoai = new List<CauThoai>();

    [Header("Typewriter")]
    [Range(0.005f, 0.2f)] public float tocDoChu = 0.03f;

    [Header("Lựa chọn")]
    public GameObject luaChonPanel;
    public int chiSoLuaChon = -1;

    [Header("Giới hạn nhánh thoại")]
    public int chiSoBatDau = 0;
    public int chiSoKetThuc = -1;

    int chiSoHienTai = 0;
    Coroutine typingCoroutine = null;
    Coroutine bgCoroutine = null;
    Image bgOverlayImage = null;
    bool choPhepTiep = false;
    bool dangLuaChon = false;
    string currentFullText = "";

    void Start()
    {
        if (khungThoai != null) khungThoai.SetActive(false);
        if (goiYTiepTuc != null) goiYTiepTuc.SetActive(false);
        if (luaChonPanel != null) luaChonPanel.SetActive(false);

        chiSoHienTai = chiSoBatDau;
        HienThiHienTai();
    }

    void Update()
    {
        if (dangLuaChon) return;
        if (!AnyKeyPressed()) return;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
            noiDungText.text = currentFullText;
            choPhepTiep = true;
            if (goiYTiepTuc != null && khungThoai.activeSelf)
                goiYTiepTuc.SetActive(true);
            return;
        }

        if (choPhepTiep)
        {
            choPhepTiep = false;
            if (goiYTiepTuc != null) goiYTiepTuc.SetActive(false);

            if (chiSoKetThuc != -1 && chiSoHienTai >= chiSoKetThuc)
                return;

            chiSoHienTai++;
            HienThiHienTai();
        }
    }

    bool AnyKeyPressed()
    {
        bool pressed = false;
        try
        {
            if (Input.anyKeyDown) pressed = true;
        }
        catch (System.InvalidOperationException)
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) pressed = true;
            if (!pressed && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) pressed = true;
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

        if (string.IsNullOrWhiteSpace(cau.tenNhanVat) && string.IsNullOrWhiteSpace(cau.noiDung))
        {
            if (khungThoai.activeSelf) khungThoai.SetActive(false);
            if (anhNenHienTai != null && cau.anhNenMoi != null)
                BatDauChuyenNen(cau.anhNenMoi);
            choPhepTiep = true;
            return;
        }

        if (!khungThoai.activeSelf) khungThoai.SetActive(true);
        if (tenNhanVatText != null) tenNhanVatText.text = cau.tenNhanVat ?? "";

        if (anhNhanVatUI != null)
        {
            if (cau.anhNhanVat != null)
            {
                anhNhanVatUI.sprite = cau.anhNhanVat;
                anhNhanVatUI.enabled = true;
            }
            else anhNhanVatUI.enabled = false;
        }

        if (anhBieuCamUI != null)
        {
            if (cau.anhBieuCam != null)
            {
                anhBieuCamUI.sprite = cau.anhBieuCam;
                anhBieuCamUI.enabled = true;
            }
            else anhBieuCamUI.enabled = false;
        }

        if (anhNenHienTai != null && cau.anhNenMoi != null && anhNenHienTai.sprite != cau.anhNenMoi)
            BatDauChuyenNen(cau.anhNenMoi);

        currentFullText = cau.noiDung ?? "";
        noiDungText.text = "";
        if (goiYTiepTuc != null) goiYTiepTuc.SetActive(false);
        choPhepTiep = false;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        typingCoroutine = StartCoroutine(Typewriter(currentFullText));

        if (chiSoHienTai == chiSoLuaChon && luaChonPanel != null)
            StartCoroutine(DelayLuaChon());
    }

    IEnumerator Typewriter(string text)
    {
        noiDungText.text = "";
        if (string.IsNullOrEmpty(text))
        {
            typingCoroutine = null;
            choPhepTiep = true;
            if (goiYTiepTuc != null) goiYTiepTuc.SetActive(true);
            yield break;
        }

        for (int i = 0; i < text.Length; i++)
        {
            noiDungText.text += text[i];
            yield return new WaitForSeconds(tocDoChu);
        }

        typingCoroutine = null;
        choPhepTiep = true;
        if (goiYTiepTuc != null) goiYTiepTuc.SetActive(true);
    }

    IEnumerator DelayLuaChon()
    {
        yield return new WaitForSeconds(0.5f);
        BatLuaChon();
    }

    void BatLuaChon()
    {
        dangLuaChon = true;
        if (luaChonPanel != null) luaChonPanel.SetActive(true);
        if (goiYTiepTuc != null) goiYTiepTuc.SetActive(false);
    }

    public void ChonLua(int chiSoMoi, int ketThucMoi = -1)
    {
        dangLuaChon = false;
        if (luaChonPanel != null) luaChonPanel.SetActive(false);

        chiSoHienTai = chiSoMoi;
        chiSoBatDau = chiSoMoi;
        chiSoKetThuc = ketThucMoi;
        HienThiHienTai();
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
        if (anhNenHienTai == null || nenMoi == null) yield break;
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
        if (bgOverlayImage != null) return;
        GameObject go = new GameObject("BG_Overlay");
        go.transform.SetParent(anhNenHienTai.transform.parent, false);
        Image img = go.AddComponent<Image>();
        RectTransform rtSrc = anhNenHienTai.rectTransform;
        RectTransform rt = img.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.localScale = Vector3.one;
        img.preserveAspect = false;
        img.raycastTarget = false;
        img.enabled = false;
        bgOverlayImage = img;
    }

    void KetThuc()
    {
        if (khungThoai != null) khungThoai.SetActive(false);
        if (goiYTiepTuc != null) goiYTiepTuc.SetActive(false);
    }
}

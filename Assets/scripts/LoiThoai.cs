using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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

    [Header("Lựa chọn (nếu có)")]
    public bool coLuaChon = false;
}

public class LoiThoai : MonoBehaviour
{
    [Header("UI thoại")]
    public GameObject khungThoai;
    public TMP_Text tenNhanVatText;
    public TMP_Text noiDungText;
    public Image anhNhanVatUI;
    public Image anhBieuCamUI;
    public GameObject goiYTiepTuc;

    [Header("Background")]
    public Image anhNenHienTai;
    public float tocDoFadeChopMat = 0.8f;

    [Header("Panel lựa chọn (tùy chọn)")]
    public GameObject panelLuaChon;

    [Header("Danh sách thoại")]
    public List<CauThoai> danhSachThoai = new List<CauThoai>();

    [Header("Hiệu ứng chữ")]
    [Range(0.005f, 0.2f)] public float tocDoChu = 0.03f;

    [Header("Scene khi kết thúc thoại")]
    public string sceneKetThuc; // Gán tên Scene ở Inspector

    int chiSoHienTai = 0;
    int chiSoKetThuc = -1;
    Coroutine typingCoroutine;
    Coroutine bgCoroutine;
    bool choPhepTiep = false;
    string currentFullText = "";
    Image blackOverlay;
    bool dangChuyenNen = false;

    void Start()
    {
        khungThoai?.SetActive(false);
        goiYTiepTuc?.SetActive(false);
        panelLuaChon?.SetActive(false);

        if (anhNenHienTai != null)
        {
            GameObject overlayObj = new GameObject("BlackOverlay");
            overlayObj.transform.SetParent(anhNenHienTai.transform.parent, false);
            blackOverlay = overlayObj.AddComponent<Image>();
            blackOverlay.color = new Color(0, 0, 0, 0);
            blackOverlay.rectTransform.anchorMin = Vector2.zero;
            blackOverlay.rectTransform.anchorMax = Vector2.one;
            blackOverlay.rectTransform.offsetMin = Vector2.zero;
            blackOverlay.rectTransform.offsetMax = Vector2.zero;
            blackOverlay.transform.SetSiblingIndex(anhNenHienTai.transform.GetSiblingIndex() + 1);
        }

        if (danhSachThoai.Count > 0)
            HienThiHienTai();
    }

    void Update()
    {
        if ((panelLuaChon != null && panelLuaChon.activeSelf) || dangChuyenNen)
            return;

        if (!AnyKeyPressed()) return;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
            noiDungText.text = currentFullText;
            choPhepTiep = true;
            goiYTiepTuc?.SetActive(true);
            return;
        }

        if (choPhepTiep)
        {
            choPhepTiep = false;
            goiYTiepTuc?.SetActive(false);

            chiSoHienTai++;
            if (chiSoKetThuc >= 0 && chiSoHienTai > chiSoKetThuc)
            {
                KetThuc();
                return;
            }

            HienThiHienTai();
        }
    }

    bool AnyKeyPressed()
    {
        try
        {
            if (Input.anyKeyDown) return true;
        }
        catch
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current?.anyKey.wasPressedThisFrame == true ||
                Mouse.current?.leftButton.wasPressedThisFrame == true)
                return true;
#endif
        }
        return false;
    }

    void HienThiHienTai()
    {
        if (chiSoHienTai < 0 || chiSoHienTai >= danhSachThoai.Count)
        {
            KetThuc();
            return;
        }

        CauThoai cau = danhSachThoai[chiSoHienTai];
        bool khongHienKhung = string.IsNullOrWhiteSpace(cau.tenNhanVat) && string.IsNullOrWhiteSpace(cau.noiDung);

        if (khongHienKhung)
        {
            khungThoai.SetActive(false);
            tenNhanVatText.text = "";
            noiDungText.text = "";
            choPhepTiep = true;
            return;
        }

        khungThoai.SetActive(true);
        tenNhanVatText.text = cau.tenNhanVat ?? "";

        if (anhNhanVatUI != null)
        {
            anhNhanVatUI.sprite = cau.anhNhanVat;
            anhNhanVatUI.enabled = (cau.anhNhanVat != null);
        }

        if (anhBieuCamUI != null)
        {
            anhBieuCamUI.sprite = cau.anhBieuCam;
            anhBieuCamUI.enabled = (cau.anhBieuCam != null);
        }

        if (cau.anhNenMoi != null && anhNenHienTai != null && cau.anhNenMoi != anhNenHienTai.sprite)
        {
            if (bgCoroutine != null) StopCoroutine(bgCoroutine);
            bgCoroutine = StartCoroutine(ChuyenNenFade(cau.anhNenMoi));
        }

        currentFullText = cau.noiDung ?? "";
        noiDungText.text = "";
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(Typewriter(currentFullText));

        if (cau.coLuaChon && panelLuaChon != null)
            StartCoroutine(ShowChoicePanelAfterTyping());
    }

    IEnumerator Typewriter(string text)
    {
        foreach (char c in text)
        {
            noiDungText.text += c;
            yield return new WaitForSeconds(tocDoChu);
        }

        typingCoroutine = null;
        choPhepTiep = true;
        goiYTiepTuc?.SetActive(true);
    }

    IEnumerator ShowChoicePanelAfterTyping()
    {
        while (typingCoroutine != null)
            yield return null;

        yield return new WaitForSeconds(0.3f);

        choPhepTiep = false;
        goiYTiepTuc?.SetActive(false);
        panelLuaChon.SetActive(true);
    }

    public void ChonLua(int batDau, int ketThuc)
    {
        panelLuaChon?.SetActive(false);
        chiSoHienTai = batDau;
        chiSoKetThuc = ketThuc;
        HienThiHienTai();
    }

    IEnumerator ChuyenNenFade(Sprite nenMoi)
    {
        if (blackOverlay == null || anhNenHienTai == null)
            yield break;

        dangChuyenNen = true;
        Vector3 oldScale = anhNenHienTai.rectTransform.localScale;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / tocDoFadeChopMat;
            blackOverlay.color = new Color(0, 0, 0, Mathf.Clamp01(t));
            yield return null;
        }

        anhNenHienTai.sprite = nenMoi;
        anhNenHienTai.rectTransform.localScale = oldScale;
        anhNenHienTai.color = Color.white;

        t = 1f;
        while (t > 0f)
        {
            t -= Time.deltaTime / tocDoFadeChopMat;
            blackOverlay.color = new Color(0, 0, 0, Mathf.Clamp01(t));
            yield return null;
        }

        blackOverlay.color = new Color(0, 0, 0, 0);
        dangChuyenNen = false;
    }

    public bool DangChayThoai() => khungThoai != null && khungThoai.activeSelf;

    void KetThuc()
    {
        khungThoai?.SetActive(false);
        goiYTiepTuc?.SetActive(false);
        panelLuaChon?.SetActive(false);

        // 🔹 Chuyển Scene khi hết thoại
        if (!string.IsNullOrEmpty(sceneKetThuc))
        {
            SceneManager.LoadScene(sceneKetThuc);
        }
    }
}

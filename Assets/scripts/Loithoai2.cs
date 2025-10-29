using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[System.Serializable]
public class CauThoai2
{
    public string tenNhanVat;
    [TextArea(2, 6)] public string noiDung;

    [Header("Hình ảnh thoại")]
    public Sprite anhNhanVat;   // Ảnh nhân vật (trái)
    public Sprite anhGiua;       // Ảnh giữa màn hình (mới thêm)
    public Sprite anhBieuCam;   // Ảnh biểu cảm (phải)
}

public class Loithoai2 : MonoBehaviour
{
    [Header("UI thoại")]
    public GameObject khungThoai;
    public TMP_Text tenNhanVatText;
    public TMP_Text noiDungText;
    public Image anhNhanVatUI;
    public Image anhGiuaUI;        // 🆕 Ảnh giữa màn hình
    public Image anhBieuCamUI;
    public GameObject goiYTiepTuc;

    [Header("Danh sách thoại")]
    public List<CauThoai2> danhSachThoai = new List<CauThoai2>();

    [Header("Typewriter")]
    [Range(0.005f, 0.2f)] public float tocDoChu = 0.03f;

    [Header("Giới hạn")]
    public int chiSoBatDau = 0;
    public int chiSoKetThuc = -1;

    int chiSoHienTai = 0;
    Coroutine typingCoroutine = null;
    bool choPhepTiep = false;
    bool daBatDauThoai = false;
    string currentFullText = "";

    void Start()
    {
        if (khungThoai != null) khungThoai.SetActive(false);
        if (goiYTiepTuc != null) goiYTiepTuc.SetActive(false);

        if (anhNhanVatUI != null) anhNhanVatUI.enabled = false;
        if (anhGiuaUI != null) anhGiuaUI.enabled = false;
        if (anhBieuCamUI != null) anhBieuCamUI.enabled = false;
    }

    void Update()
    {
        if (!daBatDauThoai) return;
        if (!AnyKeyPressed()) return;

        // Nếu đang gõ chữ → skip ngay
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
        Debug.Log($"[Loithoai2] Hiển thị câu {chiSoHienTai}/{chiSoKetThuc}");

        if (chiSoHienTai < 0 || chiSoHienTai >= danhSachThoai.Count ||
            (chiSoKetThuc >= 0 && chiSoHienTai > chiSoKetThuc))
        {
            KetThuc();
            return;
        }

        CauThoai2 cau = danhSachThoai[chiSoHienTai];

        bool khongHienKhung = string.IsNullOrWhiteSpace(cau.tenNhanVat) &&
                               string.IsNullOrWhiteSpace(cau.noiDung);

        if (khongHienKhung)
        {
            khungThoai.SetActive(false);
            tenNhanVatText.text = "";
            noiDungText.text = "";
            choPhepTiep = true;
            return;
        }

        // Hiển thị khung thoại
        khungThoai.SetActive(true);
        tenNhanVatText.text = cau.tenNhanVat ?? "";

        // 🧩 Cập nhật ảnh nhân vật (trái)
        if (anhNhanVatUI != null)
        {
            anhNhanVatUI.sprite = cau.anhNhanVat;
            anhNhanVatUI.enabled = (cau.anhNhanVat != null);
        }

        // 🆕 Cập nhật ảnh giữa màn hình
        if (anhGiuaUI != null)
        {
            anhGiuaUI.sprite = cau.anhGiua;
            anhGiuaUI.enabled = (cau.anhGiua != null);
        }

        // 🧩 Cập nhật ảnh biểu cảm (phải)
        if (anhBieuCamUI != null)
        {
            anhBieuCamUI.sprite = cau.anhBieuCam;
            anhBieuCamUI.enabled = (cau.anhBieuCam != null);
        }

        // Gõ chữ
        currentFullText = cau.noiDung ?? "";
        noiDungText.text = "";
        choPhepTiep = false;
        goiYTiepTuc?.SetActive(false);

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(Typewriter(currentFullText));
    }

    IEnumerator Typewriter(string text)
    {
        noiDungText.text = "";

        if (string.IsNullOrEmpty(text))
        {
            typingCoroutine = null;
            choPhepTiep = true;
            goiYTiepTuc?.SetActive(true);
            yield break;
        }

        foreach (char c in text)
        {
            noiDungText.text += c;
            yield return new WaitForSeconds(tocDoChu);
        }

        typingCoroutine = null;
        choPhepTiep = true;
        goiYTiepTuc?.SetActive(true);
    }

    void KetThuc()
    {
        Debug.Log("[Loithoai2] Kết thúc đoạn thoại");

        daBatDauThoai = false;
        khungThoai?.SetActive(false);
        goiYTiepTuc?.SetActive(false);

        if (anhNhanVatUI != null) anhNhanVatUI.enabled = false;
        if (anhGiuaUI != null) anhGiuaUI.enabled = false;
        if (anhBieuCamUI != null) anhBieuCamUI.enabled = false;

        chiSoBatDau = 0;
        chiSoKetThuc = -1;
        chiSoHienTai = 0;
    }

    public void KhoiDongThoai(int batDau, int ketThuc)
    {
        StopAllCoroutines();

        chiSoBatDau = batDau;
        chiSoKetThuc = ketThuc;
        chiSoHienTai = chiSoBatDau;
        daBatDauThoai = true;
        choPhepTiep = false;

        khungThoai?.SetActive(true);
        goiYTiepTuc?.SetActive(false);

        HienThiHienTai();
    }

    public bool DangChayThoai() => daBatDauThoai;

    public void ChonLua(int batDau, int ketThuc)
    {
        KhoiDongThoai(batDau, ketThuc);
    }

    void OnDestroy()
    {
        Debug.LogWarning($"⚠️ [Loithoai2] Object {name} bị Destroy lúc: {Time.time}");
    }
}

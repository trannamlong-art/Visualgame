using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class TuongTacManager : MonoBehaviour
{
    [Header("Panel tương tác")]
    public GameObject panelTuongTac;

    [Header("Tham chiếu script thoại")]
    public Loithoai2 loiThoai;

    [Header("Tham chiếu hệ thống thời gian / buổi")]
    public TimeManager timeManager;
    public string nextTimeState = "Chieu";

    [Header("Độ thiện cảm")]
    public int tangMoiLan = 1;

    [Header("Phạm vi hội thoại")]
    public Vector2Int[] noiChuyenDoan = new Vector2Int[5]
    {
        new Vector2Int(0, 7),
        new Vector2Int(8, 14),
        new Vector2Int(15, 21),
        new Vector2Int(22, 27),
        new Vector2Int(29, 35)
    };

    [Header("Đoạn xoa đầu")]
    public Vector2Int xoaDauDoan = new Vector2Int(36, 37);

    [Header("Hình ảnh sẽ ẩn khi thoại bắt đầu")]
    public GameObject hinhAnhBiAnKhiThoai; // Gắn trong Inspector

    private bool dangThoai = false;
    private int lastIndex = -1;
    private bool delaySpam = false;

    // Danh sách toàn bộ panel tương tác trong scene
    private static List<TuongTacManager> tatCaTuongTac = new List<TuongTacManager>();

    void Awake()
    {
        if (!tatCaTuongTac.Contains(this))
            tatCaTuongTac.Add(this);
    }

    void OnDestroy()
    {
        tatCaTuongTac.Remove(this);
    }

    void Start()
    {
        if (loiThoai != null) loiThoai.gameObject.SetActive(true);
        if (loiThoai != null && loiThoai.khungThoai != null)
            loiThoai.khungThoai.SetActive(false);

        if (panelTuongTac != null)
            panelTuongTac.SetActive(false);
    }

    void Update()
    {
        // Khi thoại kết thúc thì tự đóng khung
        if (dangThoai && loiThoai != null && !loiThoai.DangChayThoai())
            KetThucThoai();

        // Nếu panel đang mở mà click UI khác -> tự đóng
        if (panelTuongTac != null && panelTuongTac.activeSelf && Input.GetMouseButtonDown(0))
        {
            GameObject clickedObj = EventSystem.current.currentSelectedGameObject;

            if (clickedObj != null && clickedObj != panelTuongTac && !clickedObj.transform.IsChildOf(panelTuongTac.transform))
            {
                DongPanelTuongTac();
            }
            else if (clickedObj == null)
            {
                DongPanelTuongTac();
            }
        }
    }

    public void MoPanelTuongTac()
    {
        if (delaySpam) return;

        // Ẩn tất cả panel khác trước
        foreach (var tt in tatCaTuongTac)
        {
            if (tt != null && tt != this && tt.panelTuongTac != null)
                tt.panelTuongTac.SetActive(false);
        }

        if (panelTuongTac != null)
            panelTuongTac.SetActive(!panelTuongTac.activeSelf);

        StartCoroutine(ChongSpamDelay());
    }

    public void DongPanelTuongTac()
    {
        if (panelTuongTac != null)
            panelTuongTac.SetActive(false);
    }

    public void OnNoiChuyen()
    {
        if (delaySpam || dangThoai || loiThoai == null) return;
        StartCoroutine(ChongSpamDelay());
        DongPanelTuongTac();

        if (!TruNangLuongNeuCoDu()) return;

        int index;
        do { index = Random.Range(0, noiChuyenDoan.Length); }
        while (noiChuyenDoan.Length > 1 && index == lastIndex);
        lastIndex = index;

        Vector2Int doan = noiChuyenDoan[index];
        BatDauThoai(doan.x, doan.y);
        TangThienCam();
    }

    public void OnXoaDau()
    {
        if (delaySpam || dangThoai || loiThoai == null) return;
        StartCoroutine(ChongSpamDelay());
        DongPanelTuongTac();

        if (!TruNangLuongNeuCoDu()) return;

        BatDauThoai(xoaDauDoan.x, xoaDauDoan.y);
        TangThienCam();
    }

    void BatDauThoai(int batDau, int ketThuc)
    {
        dangThoai = true;

        // Ẩn hình ảnh khi bắt đầu thoại
        if (hinhAnhBiAnKhiThoai != null)
            hinhAnhBiAnKhiThoai.SetActive(false);

        loiThoai.KhoiDongThoai(batDau, ketThuc);
    }

    void KetThucThoai()
    {
        dangThoai = false;
        loiThoai.khungThoai?.SetActive(false);

        // Hiện lại hình ảnh sau khi thoại xong
        if (hinhAnhBiAnKhiThoai != null)
            hinhAnhBiAnKhiThoai.SetActive(true);

        if (timeManager != null)
            timeManager.NextTime();
    }

    void TangThienCam()
    {
        if (DoThienCamManager.instance != null)
            DoThienCamManager.instance.TangThienCam(tangMoiLan);
    }

    bool TruNangLuongNeuCoDu()
    {
        if (NangLuongManager.instance == null) return true;

        if (NangLuongManager.instance.nangLuongHienTai < 25)
        {
            Debug.Log("⚠ Không đủ năng lượng để tương tác!");
            return false;
        }

        NangLuongManager.instance.TruNangLuong(25);
        return true;
    }

    private IEnumerator ChongSpamDelay()
    {
        delaySpam = true;
        yield return new WaitForSeconds(0.5f);
        delaySpam = false;
    }
}

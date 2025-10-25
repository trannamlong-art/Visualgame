using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DiRaNgoaiManager : MonoBehaviour
{
    [Header("Panel chính")]
    public GameObject panelDiRaNgoai;

    [Header("Nền background khi đi dạo")]
    public GameObject backgroundDiDao;

    [Header("Script thoại")]
    public Loithoai2 loiThoai;

    [Header("Quản lý thời gian / buổi")]
    public TimeManager timeManager;

    [Header("Phạm vi thoại khi đi dạo (X - Y)")]
    public Vector2Int doanDiDao = new Vector2Int(0, 5);

    [Header("Tên scene khi đi mua sắm")]
    public string sceneMuaSam = "ShopScene";

    [Header("Năng lượng tiêu hao")]
    public int nangLuongTieuHao = 20;

    private bool dangThoai = false;
    private bool delaySpam = false;

    void Start()
    {
        if (panelDiRaNgoai != null)
            panelDiRaNgoai.SetActive(false);

        if (backgroundDiDao != null)
            backgroundDiDao.SetActive(false);

        if (loiThoai != null)
            loiThoai.gameObject.SetActive(true);
    }

    void Update()
    {
        // Khi thoại kết thúc, tắt background và panel
        if (dangThoai && loiThoai != null && !loiThoai.DangChayThoai())
            KetThucDiDao();
    }

    public void MoPanelDiRaNgoai()
    {
        if (delaySpam) return;
        panelDiRaNgoai.SetActive(true);
        StartCoroutine(ChongSpamDelay());
    }

    public void DongPanel()
    {
        if (panelDiRaNgoai != null)
            panelDiRaNgoai.SetActive(false);
    }

    public void OnDiDao()
    {
        if (delaySpam || dangThoai) return;
        StartCoroutine(ChongSpamDelay());

        // Kiểm tra năng lượng đủ chưa
        if (NangLuongManager.instance != null && NangLuongManager.instance.nangLuongHienTai < nangLuongTieuHao)
        {
            Debug.Log("⚠️ Không đủ năng lượng để đi dạo!");
            return;
        }

        DongPanel();

        // Trừ năng lượng
        TruNangLuong();

        // Hiện background đi dạo
        if (backgroundDiDao != null)
            backgroundDiDao.SetActive(true);

        // Bắt đầu hội thoại
        if (loiThoai != null)
        {
            dangThoai = true;
            loiThoai.KhoiDongThoai(doanDiDao.x, doanDiDao.y);
        }
        else
        {
            // Nếu không có thoại → tự chuyển buổi
            if (timeManager != null)
                timeManager.NextTime();

            if (backgroundDiDao != null)
                backgroundDiDao.SetActive(false);
        }
    }

    void KetThucDiDao()
    {
        dangThoai = false;

        if (loiThoai.khungThoai != null)
            loiThoai.khungThoai.SetActive(false);

        if (backgroundDiDao != null)
            backgroundDiDao.SetActive(false);

        if (timeManager != null)
            timeManager.NextTime();
    }

    public void OnMuaSam()
    {
        if (delaySpam) return;
        StartCoroutine(ChongSpamDelay());

        // Kiểm tra năng lượng đủ chưa
        if (NangLuongManager.instance != null && NangLuongManager.instance.nangLuongHienTai < nangLuongTieuHao)
        {
            Debug.Log("⚠️ Không đủ năng lượng để đi mua sắm!");
            return;
        }

        DongPanel();

        // Trừ năng lượng
        TruNangLuong();

        // Load sang scene mua sắm
        SceneManager.LoadScene(sceneMuaSam);
    }

    void TruNangLuong()
    {
        if (NangLuongManager.instance != null)
        {
            NangLuongManager.instance.TruNangLuong(nangLuongTieuHao);
            Debug.Log($"💧 Trừ {nangLuongTieuHao} năng lượng khi đi ra ngoài!");
        }
    }

    private IEnumerator ChongSpamDelay()
    {
        delaySpam = true;
        yield return new WaitForSeconds(0.5f);
        delaySpam = false;
    }
}

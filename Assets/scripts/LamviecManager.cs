using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;

public class LamviecManager : MonoBehaviour
{
    public GameObject lamViecPanel;
    public TMP_Text ketQuaText;
    public int nangLuongTieuHao = 25;
    public Button lamViecButton; // nút làm việc để disable khi chờ

    private bool choNhanPhim = false;
    private bool dangLam = false;

    public void OnLamViecButtonClicked()
    {
        if (dangLam) return; // tránh spam
        dangLam = true;

        if (lamViecButton) lamViecButton.interactable = false;

        if (NangLuongManager.instance.nangLuongHienTai < nangLuongTieuHao)
        {
            ketQuaText.text = "Bạn quá mệt, không thể làm việc...";
            lamViecPanel.SetActive(true);
            StartCoroutine(ChoNhanPhimVaChuyenBuoi(false));
            return;
        }

        int ketQua = Random.Range(0, 3);
        string thongBao = "";
        int tienNhan = 0;

        switch (ketQua)
        {
            case 0:
                thongBao = "Hôm nay làm việc tệ... (+200k)";
                tienNhan = 200_000;
                break;
            case 1:
                thongBao = "Hôm nay làm việc bình thường. (+330k)";
                tienNhan = 330_000;
                break;
            case 2:
                thongBao = "Hôm nay làm việc rất tốt! (+500k)";
                tienNhan = 500_000;
                break;
        }

        TienManager.instance.CongTien(tienNhan);
        NangLuongManager.instance.TruNangLuong(nangLuongTieuHao);

        ketQuaText.text = thongBao;
        lamViecPanel.SetActive(true);

        StartCoroutine(ChoNhanPhimVaChuyenBuoi(true));
        StartCoroutine(WaitRoutine());
    }

    private IEnumerator ChoNhanPhimVaChuyenBuoi(bool chuyenBuoi)
    {
        choNhanPhim = true;
        yield return new WaitForSeconds(0.3f);

        while (choNhanPhim)
        {
            bool nhanPhim = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;
            bool clickChuot = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;

            if (nhanPhim || clickChuot)
            {
                choNhanPhim = false;
                DongPanel();

                if (chuyenBuoi && TimeManager.instance != null)
                    TimeManager.instance.NextTime();

                yield return new WaitForSeconds(0.3f);
                if (lamViecButton) lamViecButton.interactable = true;
                dangLam = false;
            }

            yield return null;
        }
    }

    private System.Collections.IEnumerator WaitRoutine()
    {
        yield return new WaitForSeconds(3f);
        dangLam = false;
    }

    public void DongPanel()
    {
        lamViecPanel.SetActive(false);
    }
}

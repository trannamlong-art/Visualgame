using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;

public class NghingoiManager : MonoBehaviour
{
    public GameObject nghiNgoiPanel;
    public TMP_Text ketQuaText;
    public int nangLuongHoiPhuc = 50;
    public Button nghiNgoiButton;

    private bool choNhanPhim = false;
    private bool dangNghi = false;

    public void OnNghiNgoButtonClicked()
    {
        if (dangNghi) return; // tránh spam
        dangNghi = true;
        if (nghiNgoiButton) nghiNgoiButton.interactable = false;

        if (NangLuongManager.instance.nangLuongHienTai >= NangLuongManager.instance.nangLuongToiDa)
        {
            ketQuaText.text = "Bạn đã tràn đầy năng lượng, không cần nghỉ nữa!";
            nghiNgoiPanel.SetActive(true);
            StartCoroutine(ChoNhanPhim(false));
            return;
        }

        NangLuongManager.instance.HoiNangLuong(nangLuongHoiPhuc);

        ketQuaText.text = "Bạn cảm thấy tràn đầy sức sống! (+50 năng lượng)";
        nghiNgoiPanel.SetActive(true);

        StartCoroutine(ChoNhanPhim(true));
    }

    private IEnumerator ChoNhanPhim(bool chuyenBuoi)
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
                if (nghiNgoiButton) nghiNgoiButton.interactable = true;
                dangNghi = false;
            }

            yield return null;
        }
    }

    public void DongPanel()
    {
        nghiNgoiPanel.SetActive(false);
    }
}

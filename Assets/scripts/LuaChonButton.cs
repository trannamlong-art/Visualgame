using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LuaChonButton : MonoBehaviour
{
    [Header("Chỉ số thoại")]
    public int chiSoBatDau;
    public int chiSoKetThuc = -1;

    [Header("Hệ thống thoại liên kết")]
    public LoiThoai heThong;

    [Header("Chuyển Scene sau khi hết thoại")]
    public bool chuyenSceneSauKhiKetThuc = false;
    public string tenSceneChuyenDen = "";

    Button btn;

    void OnEnable()
    {
        if (btn == null)
            btn = GetComponent<Button>();

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (heThong == null) return;
        heThong.ChonLua(chiSoBatDau, chiSoKetThuc);

        if (chuyenSceneSauKhiKetThuc && !string.IsNullOrEmpty(tenSceneChuyenDen))
            heThong.StartCoroutine(ChoDenKhiHetThoai()); 
    }

    IEnumerator ChoDenKhiHetThoai()
    {
        while (heThong != null && heThong.DangChayThoai())
            yield return null;

        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(tenSceneChuyenDen);
    }
}

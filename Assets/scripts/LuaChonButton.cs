using UnityEngine;
using UnityEngine.UI;

public class LuaChonButton : MonoBehaviour
{
    public int chiSoBatDau;
    public int chiSoKetThuc = -1;
    public LoiThoai heThong;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (heThong != null)
            heThong.ChonLua(chiSoBatDau, chiSoKetThuc);
    }
}

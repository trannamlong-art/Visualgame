﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenu : MonoBehaviour
{
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}

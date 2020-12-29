using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] CanvasGroup mainMenu;
    static CanvasGroup m_mainMenu;
    [SerializeField] GameObject HUD;
    static GameObject m_HUD;
    public enum UIScreen
    {
        MainMenu = 0
    }

    void Awake()
    {
        m_mainMenu = mainMenu;
        m_HUD = HUD;
    }

    static public void OpenOrClose(UIScreen screen)
    {
        switch(screen)
        {
            case UIScreen.MainMenu: OpenOrClose(m_mainMenu);
                break;
        }
    }
    static public void OpenOrClose(CanvasGroup screen)
    {
        screen.alpha = screen.alpha > 0 ? 0 : 1;
        screen.blocksRaycasts = !(screen.blocksRaycasts); //!  = true ? false : true;
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        //cursor.gameObject.SetActive(cursor.gameObject.activeSelf ? false: true);
    }

    static public void ToggleHUD()
    {
        m_HUD.SetActive(!m_HUD.activeSelf);
    }
}

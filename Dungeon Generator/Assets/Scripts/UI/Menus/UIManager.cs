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
    public GraphemeDatabase graphemeDatabase;
    public AudioClip buttonEnter;
    public AudioClip buttonClick;
    public AudioClip buttonReturn;

    List<CanvasGroup> openMenus = new List<CanvasGroup>();

    void Awake()
    {
        m_mainMenu = mainMenu;
        m_HUD = HUD;
    }

    private void Start() 
    {
        m_mainMenu.GetComponent<MainMenu>().Initialize(this, GetComponent<AudioSource>(), buttonEnter);
    }
    public void OpenOrClose(UIScreen screen)
    {
        switch(screen)
        {
            case UIScreen.MainMenu: 

            OpenOrClose(m_mainMenu);
            if(m_mainMenu.alpha == 0)
            {
                EmptyMenus();
            }
            m_mainMenu.GetComponent<MainMenu>().SwitchMenu(0); break;
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

    public void AddMenu(CanvasGroup menu)
    {
        openMenus.Add(menu);
    }

    public void EmptyMenus()
    {
        for(int i = 0; i < openMenus.Count; i++)
        {
            OpenOrClose(openMenus[i]);
        }
        openMenus.Clear();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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

    List<CanvasGroup> openMenus = new List<CanvasGroup>();

    [SerializeField]Volume volume;

    static Volume m_volume;

    void Awake()
    {
        m_mainMenu = mainMenu;
        m_HUD = HUD;
        m_volume = volume;
    }

    private void Start() 
    {
        m_mainMenu.GetComponent<Menu>().Initialize(this, GetComponent<AudioSource>());
        if(m_mainMenu.alpha == 1){m_mainMenu.GetComponent<Menu>().SwitchMenu(0);}
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
            m_mainMenu.GetComponent<Menu>().SwitchMenu(0); break;
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
        ColorAdjustments colorAdjustments;
        m_volume.profile.TryGet<ColorAdjustments>(out colorAdjustments);
        colorAdjustments.active = !m_HUD.activeSelf;
        DepthOfField depthOfField;
        m_volume.profile.TryGet<DepthOfField>(out depthOfField);
        depthOfField.focusDistance.value = m_HUD.activeSelf ? 1.8f : 4.5f;
        depthOfField.focalLength.value = m_HUD.activeSelf ? 50 : 300;
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

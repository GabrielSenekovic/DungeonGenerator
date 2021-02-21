using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [System.Serializable]struct ButtonData
    {
        public string name;
        public int destination;

        public UnityEngine.CanvasGroup canvas;
    }
    [System.Serializable]struct ButtonLayout
    {
        public string name;
        public List<ButtonData> buttons;
        public int parentMenu;
    }
    [SerializeField] List<ButtonLayout> buttonLayouts;
    //0 == Main Menu
    //1 == Party
    //2 == Equipment
    //3 == Inventory
    //4 == Crafting
    //5 == Abilities
    //6 == Quests
    //7 == Map
    //8 == Bestiary
    //9 == Jukebox
    //10 == Trophies
    //11 == Tutorial
    //12 == Config

    GameObject[] buttons = new GameObject[12];
    [SerializeField]private Sprite buttonSprite;

    UIManager UI;

    public GameObject frame;

    public bool central;

    public void SwitchMenu(int i)
    {
        for(int j = 0; j < buttons.Length; j++)
        {
            if(j < buttonLayouts[i].buttons.Count)
            {
                buttons[j].GetComponent<Image>().color = Color.white;
                buttons[j].GetComponent<Image>().raycastTarget = true;
                int index_1 = i; int index_2 = j;
                buttons[j].GetComponentInChildren<SpriteText>().text = buttonLayouts[i].buttons[j].name; 
                buttons[j].GetComponentInChildren<SpriteText>().Write();
                buttons[j].GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
                if(buttonLayouts[index_1].buttons[index_2].destination >= 0)
                {
                    buttons[j].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => SwitchMenu(buttonLayouts[index_1].buttons[index_2].destination) );
                }
                else if(buttonLayouts[index_1].buttons[index_2].destination == -1)
                {
                    buttons[j].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => SceneManager.LoadScene(1));
                }
                else
                {
                    buttons[j].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {Application.Quit(); Debug.Log("Quit the game from Menu");});
                }
                buttons[j].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {UI.GetComponent<AudioSource>().clip = UI.buttonClick; UI.GetComponent<AudioSource>().Play();});
                if(buttonLayouts[i].buttons[j].canvas != null)
                {
                    UnityEngine.Events.UnityAction temp = () => UIManager.OpenOrClose(buttonLayouts[index_1].buttons[index_2].canvas);
                    buttons[j].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(temp);
                    temp = () => UI.AddMenu(buttonLayouts[index_1].buttons[index_2].canvas);
                    buttons[j].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(temp);
                }
            }
            else if(j < buttons.Length - 1 || buttonLayouts[i].parentMenu < 0)
            {
                buttons[j].GetComponent<Image>().color = Color.clear;
                buttons[j].GetComponentInChildren<SpriteText>().text = "";
                buttons[j].GetComponent<Image>().raycastTarget = false;
            }
            else if(buttonLayouts[i].parentMenu >= 0)
            {
                //Make return button
                buttons[j].GetComponent<Image>().color = Color.white;
                buttons[j].GetComponent<Image>().raycastTarget = true;
                buttons[j].GetComponentInChildren<SpriteText>().text = "Return";
                buttons[j].GetComponentInChildren<SpriteText>().Write();
                buttons[j].GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
                buttons[j].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => SwitchMenu(buttonLayouts[i].parentMenu) );
                buttons[j].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {UI.GetComponent<AudioSource>().clip = UI.buttonReturn; UI.GetComponent<AudioSource>().Play();});
                buttons[j].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => UI.EmptyMenus());
            }
        }
    }
    public void Initialize(UIManager UI_in, AudioSource audio, AudioClip buttonEnterClip)
    {
        Debug.Log("Initializing Menu");
        UI = UI_in;

        GameObject buttonTransform = new GameObject("Buttons"); buttonTransform.transform.parent = transform;

        RectTransform rect = buttonTransform.AddComponent<RectTransform>(); 
        rect.localScale = new Vector3(1,1,1);
        rect.anchorMax = new Vector2(0,1);
        rect.anchorMin = new Vector2(0,1);
        if(central)
        {
            rect.localPosition = new Vector3(-30, -80, 0);
        }
        else
        {
            rect.localPosition = new Vector3(-254, 77, 0);
        }

        GridLayoutGroup gridLayout = buttonTransform.AddComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(buttonSprite.texture.width, buttonSprite.texture.height);
        gridLayout.spacing = new Vector2(0, -13);
        gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
        gridLayout.childAlignment = TextAnchor.UpperLeft;
        gridLayout.constraint = GridLayoutGroup.Constraint.Flexible;

        for(int i = 0; i < buttons.Length; i++)
        {
            AudioSource audioSource = audio;
            buttons[i] = new GameObject("Button: " + (i+1)); buttons[i].transform.parent = buttonTransform.transform; 
            buttons[i].AddComponent<RectTransform>();
            buttons[i].GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
            buttons[i].AddComponent<UnityEngine.UI.Image>();
            buttons[i].GetComponent<UnityEngine.UI.Image>().sprite = buttonSprite;
            buttons[i].AddComponent<UnityEngine.UI.Button>();
            buttons[i].GetComponent<UnityEngine.UI.Button>().image = buttons[i].GetComponent<UnityEngine.UI.Image>();
            buttons[i].AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener( (data) => { audioSource.clip = buttonEnterClip; audioSource.Play();} );
            buttons[i].GetComponent<EventTrigger>().triggers.Add(entry);

            GameObject textObject = new GameObject("Text"); textObject.transform.parent = buttons[i].transform;
            textObject.AddComponent<SpriteText>();
            textObject.GetComponent<SpriteText>().Initialize(UI.graphemeDatabase.fonts[0], false);
            textObject.GetComponent<SpriteText>().text = "";
            textObject.GetComponent<SpriteText>().spaceSize = 8;
            textObject.AddComponent<RectTransform>();
            textObject.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
            textObject.GetComponent<RectTransform>().localPosition = new Vector3(-56, 46, 0);
        }
        if(frame)
        {
            GameObject frame_temp = Instantiate(frame, new Vector3(0,0,0), Quaternion.identity, transform);
            frame_temp.GetComponent<RectTransform>().localPosition = Vector3.zero;
        }
    }
}

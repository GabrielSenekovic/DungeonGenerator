using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class QuestSelect : MonoBehaviour
{
    public struct SeedBox
    {
        public SeedBox(int constSeed_in, int dataSeed_in, int questSeed_in)
        {
            constructionSeed = constSeed_in;
            dataSeed = dataSeed_in;
            questSeed = questSeed_in;
        }
        public int constructionSeed;
        public int dataSeed;
        public int questSeed;
    }
    List<SeedBox> seeds = new List<SeedBox>(){};

    List<LevelData> levels = new List<LevelData>(){};

    List<QuestData> quests = new List<QuestData>(){};

    bool selectedByButton = false;

    public List<Button> buttons = new List<Button>(){};
    [System.NonSerialized] public int index = 0;

    [SerializeField]Button buttonPrefab;
    [SerializeField]Transform buttonParent;
    [SerializeField]SpriteText detailText;

    [SerializeField]GraphemeDatabase graphemeDatabase; //Remove later

    BulletinBoard board = null;

    public void Initialize(Tuple<int[], int[], int[]> seeds_in, BulletinBoard board_in)
    {
        board = board_in;
        for(int i = 0; i < seeds_in.Item1.Length; i++)
        {
            seeds.Add(new SeedBox(seeds_in.Item1[i], seeds_in.Item2[i], seeds_in.Item3[i]));
            levels.Add(GetComponent<LevelDataGenerator>().Initialize(seeds[i].dataSeed));
            buttons.Add(Instantiate(buttonPrefab, buttonPrefab.transform.position, Quaternion.identity, buttonParent));
            buttons[i].GetComponent<QuestButton>().select = this;
            buttons[i].GetComponent<QuestButton>().index = i;
            quests.Add(GetComponent<QuestDataGenerator>().Initialize(seeds[i].questSeed));
        }
        detailText.Initialize(graphemeDatabase.fonts[0]);
    }

    public void OnClose()
    {
        for(int i = buttons.Count-1; i >= 0; i--)
        {
            Button temp = buttons[i];
            buttons.RemoveAt(i);
            Destroy(temp.gameObject);
        }
        board.OnClose();
        board = null;
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void OnLoadLevel()
    {
        Time.timeScale = 1;
        GameData.SetSeed(seeds[index].constructionSeed, seeds[index].dataSeed, seeds[index].questSeed);
        GameData.currentQuest = GetComponent<QuestDataGenerator>().Initialize(seeds[index].questSeed);
        Debug.Log(index);
        SceneManager.LoadSceneAsync("Level");
        OnClose();
        Time.timeScale = 1;
    }

    private void Update() 
    {
        if(GetComponent<CanvasGroup>().alpha == 0)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if(index ==0)
            {
                index = buttons.Count-1;
            }
            else
            {
                index--;
            }
            buttons[index].Select();
            RevealDetails(index);
            selectedByButton = true;
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            if(index == buttons.Count-1)
            {
                index = 0;
            }
            else
            {
                index++;
            }
            buttons[index].Select();
            RevealDetails(index);
            selectedByButton = true;
        }
        if(HasMouseMoved())
        {
            if(EventSystem.current.currentSelectedGameObject == buttons[index].gameObject && selectedByButton)
            {
                EventSystem.current.SetSelectedGameObject(null);
                selectedByButton = false;
                HideDetails();
            }
        }
    }
    bool HasMouseMoved()
    {
        return (Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0);
    }
    public void RevealDetails(int index_in)
    {
        detailText.text = "Information about the quest: \n";
        detailText.text += quests[index_in].GetQuestDescription();
        detailText.text += "\nQuestgiver: \nObjective: " + "\nDifficulty level: \nReward: \n";
        detailText.text += "\nInformation about the destination: \n";
        detailText.text += "\nThis place is a: " + levels[index_in].m_biome + ". \n";
        if(levels[index_in].m_mood[0] != levels[index_in].m_mood[1])
        {
            detailText.text += GetBiomeDescription(levels[index_in].m_mood[0], false) + " ";
            detailText.text += GetBiomeDescription(levels[index_in].m_mood[1], false) + "\n";
        }
        else
        {
            detailText.text += GetBiomeDescription(levels[index_in].m_mood[0], true) + "\n";
        }
        detailText.text += "\nSeeds: \nData seed: " + seeds[index_in].dataSeed + "\n";
        detailText.text += "Construction seed: " + seeds[index_in].constructionSeed + "\n";
        detailText.Write();
    }
    public void HideDetails()
    {
        detailText.text = "";
        detailText.Write();
    }
    public string GetBiomeDescription(Mood mood, bool same)
    {
        if(!same)
        {
            switch(mood)
            {
                case Mood.Adventurous: return "This place is perfect for the passionate adventurer.";
                case Mood.Calm: return "It is a generally calm location where people pass through to sooth their spirits.";
                case Mood.Creepy: return "Some say that this place might be haunted.";
                case Mood.Dangerous: return "This location is notorious for the many traps that loiter around. Enter at your own risk.";
                case Mood.Cursed: return "A curse has befallen this place. Be very careful.";
                case Mood.Decrepit: return "Ruins of ancient civilization scatter these lands.";
                case Mood.Fabulous: return "Fabulous sights await whoever steps food on these lands.";
                case Mood.Mysterious: return "A thick mist of magic envelops this place.";
                case Mood.Plentiful: return "A place of many riches; many a traveler has come here to loot in the past looking to make themselves a new life.";
                default: return "";
            }
        }
        else
        {
            switch(mood)
            {
                case Mood.Adventurous: return "This place is perfect for the passionate adventurer.";
                case Mood.Calm: return "Here it is so calm that you can hear little else than the gentle sounds of nature. There is a high chance to find a settlement.";
                case Mood.Creepy: return "There are no doubts that this place is haunted. Scattered across these lands are little other than the remains of what once lived.";
                case Mood.Dangerous: return "This location is notorious for the many traps that loiter around. Enter at your own risk.";
                case Mood.Cursed: return "An incredible curse plagues these lands. Even if you survive there are no guarantees that you will ever be the same.";
                case Mood.Decrepit: return "Ruins of ancient civilization scatter these lands.";
                case Mood.Fabulous: return "Fabulous sights await whoever steps food on these lands.";
                case Mood.Mysterious: return "A thick mist of magic envelops this place.";
                case Mood.Plentiful: return "A place of many riches; many a traveler has come here to loot in the past looking to make themselves a new life.";
                default: return "";
            }
        }
    }
}

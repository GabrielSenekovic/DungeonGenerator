using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
public class MusicGenerator : MonoBehaviour
{
    public enum Key
    {
        C = 0
    }
    public enum ChordType
    {
        MAJOR = 0,
        MINOR = 1,
        AUGMENTED = 2,
        DIMINISHED = 3,
        SUSPENDED_4 = 4,
        SUSPENDED_2 = 5,
        MAJOR_7 = 6,
        MINOR_7 = 7,
        MAJORMINOR_7 = 8, //The typical dominant 7th chord
        MINORMAJOR_7 = 9,
        DOM_7_FLAT_5 = 10,
        DIMINISHED_7 = 11,

        HALFDIMINISHED_7 = 12,
        DIMINISHEDMAJOR_7 = 13,
        AUGMENTEDMAJOR_7 = 14,
        AUGMENTEDMINOR_7 = 15
    }
    [System.Serializable]public struct Chord
    {
        public Chord(ChordType type_in, List<Note> notes_in, int index_in, bool sort_in, int root_in)
        {
            type = type_in;
            notes = notes_in;
            index = index_in;
            sort = sort_in;
            root = root_in;
        }
        public ChordType type;
        public List<Note> notes;
        public int index;

        public int root; //always in key
        public bool sort;
    }
    [System.Serializable]public struct Section 
    {
        public enum PlayType
        {
            AllAtOnce = 0,
            ThreeUp_Suspend = 1, //like a waltz
            ThreeDown_Suspend = 2,

            UpwardsWave = 3, //C, E, G, E
        }
        public Section(List<Chord> chords_in, PlayType playType_in, int repeats_in)
        {
            chords = chords_in;
            playType = playType_in;
            repeats = repeats_in;
        }
        public List<Chord> chords;
        public PlayType playType;
        public int repeats;
    }
    public List<Note> notes;
    public List<Section> song;

    public List<Note> keyNotes; //the notes that are in the key

    public List<AudioSource> baseChord;
    public AudioSource melody;
    public bool playing = false;
    public Section.PlayType playType;

    public Mood mood;

    private void Start()
    {
        FillKey();
        song.Add(new Section(MakeCadence(4, false), playType, 2));
    }
    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if(!playing)
        {
            StartCoroutine(Play());
        }
    }
    void FillKey()
    {
        for(int i = 0; i < 12; i++)
        {
            keyNotes.Add(notes[i]);
        }
    }
    List<Chord> MakeCadence(int length, bool sort)
    {
        List<Chord> temp = new List<Chord>(){};
        temp.Add(MakeChord(0, ChordType.MAJOR, 1));
        bool last = false;
        for(int i = 1; i < length; i++)
        {
            if(i == length -1){last = true;}
            Tuple<int, int, ChordType> temp2 = GetNewChord(new Tuple<int,ChordType>(temp[i-1].root, temp[i-1].type), last);
            temp.Add(MakeChord(temp2.Item1, temp2.Item3, temp2.Item2));
        }
        return temp;
    }

    Tuple<int,int, ChordType> GetNewChord(Tuple<int,ChordType> previousChord, bool last)
    {
        List<Tuple<int,int, ChordType>> possibleNotes = new List<Tuple<int,int, ChordType>>(){};
        switch(previousChord.Item1)
        {
            case 1: //if its the tonic
                if(previousChord.Item2 == ChordType.AUGMENTED)
                {
                    possibleNotes.Add(new Tuple<int,int, ChordType>(5,4,ChordType.MAJOR));
                    possibleNotes.Add(new Tuple<int,int, ChordType>(9,6,ChordType.MINOR));
                    break;
                }
                else if(mood != Mood.Calm)
                {
                    possibleNotes.Add(new Tuple<int,int, ChordType>(0,1, ChordType.MAJOR_7));
                    possibleNotes.Add(new Tuple<int,int, ChordType>(0,1, ChordType.AUGMENTED));
                    possibleNotes.Add(new Tuple<int,int, ChordType>(2,2, ChordType.MINOR));
                    possibleNotes.Add(new Tuple<int,int, ChordType>(4,3, ChordType.MINOR));
                    possibleNotes.Add(new Tuple<int,int, ChordType>(9,6, ChordType.MINOR));
                }
                possibleNotes.Add(new Tuple<int,int, ChordType>(5,4, ChordType.MAJOR));
                possibleNotes.Add(new Tuple<int,int, ChordType>(7,5, ChordType.MAJOR));
            //possibleNotes.Add(11); 
                break;
            case 2: //if its the parallell subdominant
                possibleNotes.Add(new Tuple<int,int, ChordType>(0,1, ChordType.MAJOR));
                possibleNotes.Add(new Tuple<int,int, ChordType>(2,2,ChordType.MINOR_7));
                possibleNotes.Add(new Tuple<int,int, ChordType>(4,3,ChordType.MINOR));
                possibleNotes.Add(new Tuple<int,int, ChordType>(7,5,ChordType.MAJOR));
                break;
            case 3: //if its the parallell dominant
                possibleNotes.Add(new Tuple<int,int,ChordType>(0,1, ChordType.MAJOR));
                possibleNotes.Add(new Tuple<int,int, ChordType>(4,3,ChordType.MINOR_7));
                possibleNotes.Add(new Tuple<int,int,ChordType>(9,6, ChordType.MINOR));
                break;
            case 4: //if its the subdominant
                if(previousChord.Item2 == ChordType.AUGMENTED)
                {
                    return new Tuple<int, int, ChordType>(2,2, ChordType.MINOR);
                }
                possibleNotes.Add(new Tuple<int,int,ChordType>(0,1, ChordType.MAJOR));
                possibleNotes.Add(new Tuple<int,int,ChordType>(7,5, ChordType.MAJOR));
                possibleNotes.Add(new Tuple<int,int,ChordType>(7,5, ChordType.MAJORMINOR_7));
                if(mood!= Mood.Calm)
                {
                    possibleNotes.Add(new Tuple<int,int, ChordType>(2,2, ChordType.MINOR));
                    possibleNotes.Add(new Tuple<int,int, ChordType>(5,4, ChordType.AUGMENTED));
                    possibleNotes.Add(new Tuple<int,int, ChordType>(5,4, ChordType.MAJOR_7));
                }
                break;
            case 5: //if its the dominant
                if(previousChord.Item2 == ChordType.AUGMENTED)
                {
                    return new Tuple<int, int, ChordType>(4,3, ChordType.MINOR);
                }
                possibleNotes.Add(new Tuple<int,int, ChordType>(0,1, ChordType.MAJOR));
                if(mood != Mood.Calm)
                {
                    possibleNotes.Add(new Tuple<int,int, ChordType>(4,3, ChordType.MINOR));
                    if(previousChord.Item2 != ChordType.MAJORMINOR_7)
                    {
                        possibleNotes.Add(new Tuple<int,int, ChordType>(7,5, ChordType.AUGMENTED));
                        possibleNotes.Add(new Tuple<int,int, ChordType>(9,6, ChordType.MINOR));
                    }
                }
                break;
            case 6: //if its the parallell tonic
                possibleNotes.Add(new Tuple<int,int, ChordType>(0,1, ChordType.MAJOR));
                possibleNotes.Add(new Tuple<int,int, ChordType>(2,2, ChordType.MINOR));
                possibleNotes.Add(new Tuple<int,int, ChordType>(4,3, ChordType.MINOR));
                possibleNotes.Add(new Tuple<int,int, ChordType>(5,4, ChordType.MAJOR));
                possibleNotes.Add(new Tuple<int,int, ChordType>(7,5, ChordType.MAJOR));
                possibleNotes.Add(new Tuple<int,int, ChordType>(7,5, ChordType.MAJORMINOR_7));
                possibleNotes.Add(new Tuple<int,int, ChordType>(9,6,ChordType.MINOR_7));
                break;
            case 7: //if its the diminished dominant
                possibleNotes.Add(new Tuple<int,int, ChordType>(0,1, ChordType.MAJOR));
                possibleNotes.Add(new Tuple<int,int, ChordType>(9,6,ChordType.MINOR));
                break;
        }
        return possibleNotes[UnityEngine.Random.Range(0, possibleNotes.Count)];
    }

    ChordType GetChordType(int chord, int previousChord)
    {
        switch(chord)
        {
            case 1: return ChordType.MAJOR;
            case 2: return ChordType.MINOR;
            case 3: return ChordType.MINOR;
            case 4: return ChordType.MAJOR;
            case 5: return ChordType.MAJOR;
            case 6: return ChordType.MINOR;
            case 7: return ChordType.DIMINISHED;
            default: return ChordType.MAJOR;
        }
    }
    Chord MakeChord(int noteIndex, ChordType type, int root)
    {
        Chord temp = new Chord(type, new List<Note>(){keyNotes[0], keyNotes[0+4], keyNotes[0+7]}, noteIndex, true, root);
        List<int> temp2 = new List<int>(){};
        switch(type)
        {
            case ChordType.MAJOR: 
            temp2 = new List<int>(){noteIndex, (noteIndex+4)%12, (noteIndex+7)%12};
            break;
            case ChordType.MINOR: 
            temp2 = new List<int>(){noteIndex, (noteIndex+3)%12, (noteIndex+7)%12};
            break;
            case ChordType.AUGMENTED:
            temp2 = new List<int>(){noteIndex, (noteIndex+4)%12, (noteIndex+8)%12};
            break;
            case ChordType.DIMINISHED:
            temp2 = new List<int>(){noteIndex, (noteIndex+3)%12, (noteIndex+6)%12};
            break;
            case ChordType.SUSPENDED_4:
            temp2 = new List<int>(){noteIndex, (noteIndex+5)%12, (noteIndex+7)%12};
            break;
            case ChordType.SUSPENDED_2:
            temp2 = new List<int>(){noteIndex, (noteIndex+2)%12, (noteIndex+7)%12};
            break;
            case ChordType.MAJOR_7:
            temp2 = new List<int>(){noteIndex, (noteIndex+4)%12, (noteIndex+11)%12};
            break;
            case ChordType.MINOR_7:
            temp2 = new List<int>(){noteIndex, (noteIndex+3)%12, (noteIndex+10)%12};
            break;
            case ChordType.MAJORMINOR_7:
            temp2 = new List<int>(){noteIndex, (noteIndex+4)%12, (noteIndex+10)%12};
            break;
            case ChordType.MINORMAJOR_7:
            case ChordType.DOM_7_FLAT_5:
            case ChordType.DIMINISHED_7:
            default: break;
        }
        if(temp.sort)
        {
            temp2.Sort();
        }
        temp = new Chord(type, new List<Note>(){keyNotes[temp2[0]], keyNotes[temp2[1]], keyNotes[temp2[2]]}, noteIndex, temp.sort, root);
        return temp;
    }
    public IEnumerator Play()
    {
        playing = true;
        for(int i = 0; i < song[0].chords.Count; i++)
        {
            //what chord of the song it is
            switch(song[0].playType)
            {
                case Section.PlayType.AllAtOnce:
                    for(int j = 0; j < song[0].chords[0].notes.Count; j++)
                    {
                        baseChord[j].Stop();
                        baseChord[j].clip = song[0].chords[i].notes[j].clip;
                        baseChord[j].Play(); 
                    }
                    yield return new WaitForSeconds(song[0].chords[i].notes[0].clip.length);
                    break;
                case Section.PlayType.ThreeUp_Suspend:
                    foreach(AudioSource source in baseChord)
                    {
                        source.Stop();
                    }
                    for(int k = 0; k < song[0].repeats; k++)
                    {
                        for(int j = 0; j < song[0].chords[0].notes.Count; j++)
                        {
                            baseChord[j].clip = song[0].chords[i].notes[j].clip;
                            baseChord[j].Play();
                            yield return new WaitForSeconds(song[0].chords[i].notes[0].clip.length/3/song[0].repeats);
                        }
                    }break;
                case Section.PlayType.UpwardsWave:
                    foreach(AudioSource source in baseChord)
                    {
                        source.Stop();
                    }
                    for(int k = 0; k < song[0].repeats; k++)
                    {
                        for(int j = 0; j < song[0].chords[0].notes.Count; j++)
                        {
                            baseChord[j].clip = song[0].chords[i].notes[j].clip;
                            baseChord[j].Play();
                            yield return new WaitForSeconds(song[0].chords[i].notes[0].clip.length/4/song[0].repeats);
                        }
                        baseChord[1].Play();
                        yield return new WaitForSeconds(song[0].chords[i].notes[0].clip.length/4/song[0].repeats);
                    }break;
                default: break;
            }
        }
        playing = false;
    }
}

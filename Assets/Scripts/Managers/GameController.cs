using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Logging;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header("PlayerCharacters")]
    [SerializeField]
    List<GameObject> UnlockedCharacters = new List<GameObject>();
    [SerializeField]
    List<GameObject> SelectedCharacters = new List<GameObject>();
    [SerializeField]
    List<GameObject> LockedCharacters = new List<GameObject>();
    [SerializeField]
    List<GameObject> AvailableCharacters = new List<GameObject>();
    [SerializeField]
    Color[] BodyColours;

    [Header("Enemies")]
    [SerializeField]
    List<GameObject> AvailableEnemies = new List<GameObject>();
    [SerializeField]
    GameObject Boss;
    [SerializeField]
    int EnemyTypeMax = 1;

    [Header("SceneControl")]
    [SerializeField]
    int Level= 1;
    [SerializeField]
    int UtilitySceneCount = 1;
    [SerializeField]
    int ActiveLevels = 1;

    [Header("Audio")]
    [SerializeField, Range(0,1)]
    float MusicVolume;
    [SerializeField, Range(0, 1)]
    float EffectsVolume;

    [Header("Gear")]
    [SerializeField]
    List<GameObject> AvailableGear = new List<GameObject>();
    [SerializeField]
    List<GameObject> UnlockedGear = new List<GameObject>();
    [SerializeField]
    List<GameObject> LockedGear = new List<GameObject>();

    [Header("Skills")]
    [SerializeField]
    List<GameObject> AvailableSkills = new List<GameObject>();
    [SerializeField]
    List<GameObject> UnlockedSkills = new List<GameObject>();
    [SerializeField]
    List<GameObject> LockedSkills = new List<GameObject>();

    [Header("PersistentPools")]
    [SerializeField]
    List<GameObject> GearPool;
    [SerializeField]
    List<GameObject> SkillPool;

    [Header("GameStats")]
    [SerializeField]
    int EnemiesSlain = 0;

    GameObject Menu;
    SaveData SaveDataComponent;
    private static int m_referenceCount = 0;

    private static GameController m_instance;

    enum SaveableType
    {
        Character,
        Gear,
        Skill
    }

    public static GameController Instance
    {
        get
        {
            return m_instance;
        }
    }
    void Awake()
    {
        m_referenceCount++;
        if (m_referenceCount > 1)
        {
            DestroyImmediate(this.gameObject);
            return;
        }
        m_instance = this;
        LoadPrefs();
        SaveDataComponent = GetComponent<SaveData>();
        SaveDataComponent.LoadFromJson();
        List<string> characters = SaveDataComponent.GetUnlockedCharacters();
        List<string> gear = SaveDataComponent.GetUnlockedGear();
        List<string> skills = SaveDataComponent.GetUnlockedSkills();
        if(characters != null && characters.Count != 0)
        {
            UnlockedCharacters = GetObjectsFromIdentifiers(characters, SaveableType.Character);
        }
        if(gear != null && gear.Count != 0)
        {
            UnlockedGear = GetObjectsFromIdentifiers(gear, SaveableType.Gear);
        }
        if(skills != null && skills.Count != 0)
        {
            UnlockedSkills = GetObjectsFromIdentifiers(skills, SaveableType.Skill);
        }
        LockedCharacters = AvailableCharacters.Except(UnlockedCharacters).ToList();
        LockedGear = AvailableGear.Except(UnlockedGear).ToList();
        LockedSkills = AvailableSkills.Except(UnlockedSkills).ToList();
        // Use this line if you need the object to persist across scenes
        DontDestroyOnLoad(this.gameObject);
    }

    void OnDestroy()
    {
        m_referenceCount--;
        if (m_referenceCount == 0)
        {
            m_instance = null;
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        int count = UnlockedCharacters.Count;
        if(count >4)
        {
            count = 4;
        }
        for(int i = 0; i < count; i++)
        {
            SelectedCharacters.Add(UnlockedCharacters[i]);
        }
        while (SelectedCharacters.Count < 4)
        {
            SelectedCharacters.Add(UnlockedCharacters[0]);
        }
        ActiveLevels = SceneManager.sceneCountInBuildSettings;
    }

    public GameObject GetAvailableCharacter(int index)
    {
        if(index < UnlockedCharacters.Count)
        {
            return UnlockedCharacters[index];
        }
        else
        {
            return UnlockedCharacters[0];
        }

    }

    public void SetCharacter(int index, GameObject character)
    {
        SelectedCharacters[index] = character;
    }

    public List<GameObject> GetSelectedCharacters()
    {
        return SelectedCharacters;
    }

    public List<GameObject> GetAvailableEnemies()
    {
        return AvailableEnemies;
    }

    public GameObject GetBoss()
    {
        return Boss;
    }

    public int GetCharacterCount()
    {
        return UnlockedCharacters.Count;
    }

    public Color GetCharacterBodyColour(int index)
    {
        return BodyColours[index];
    }

    public int GetLevel()
    {
        return Level;
    }

    public int GetEnemyTypeMax()
    {
        return EnemyTypeMax;
    }

    public void BeginGame()
    {
        for(int i = 0 ; i < SelectedCharacters.Count; i++)
        {
            List<CharacterSelecter> selecters = GetAllCharacterSelecters();
            if(SelectedCharacters[i])
            {
                string characterName = selecters[i].GetName();
                SelectedCharacters[i] = Instantiate(SelectedCharacters[i], new Vector3(1000,1000,0), Quaternion.identity);
                SelectedCharacters[i].GetComponent<PlayerCharacter>().SetName(characterName);
                ColorCharacter(SelectedCharacters[i], GetCharacterBodyColour(i), selecters[i].GetCharacterColor(), selecters[i].GetHairColor());
                DontDestroyOnLoad(SelectedCharacters[i]);
            }
        }
        EnterBattle();
    }

    void ColorCharacter(GameObject Character, Color bodyColor, Color skinColor, Color hairColor)
    {
        PlayerCharacter playerCharacter = Character.GetComponent<PlayerCharacter>();
        playerCharacter.SetBodyColour(bodyColor);
        playerCharacter.SetSkinColour(skinColor);
        playerCharacter.SetHairColour(hairColor);
    }

    List<CharacterSelecter> GetAllCharacterSelecters()
    {
        List<CharacterSelecter> list = FindObjectsByType<CharacterSelecter>(FindObjectsSortMode.None).ToList<CharacterSelecter>();
        list.Sort((x, y) => x.GetSelecterIndex().CompareTo(y.GetSelecterIndex()));
        return list;
    }

    public void EnterBattle()
    {
        Level++;
        GetComponent<MusicManager>().ChangeMute();
        if (Level >= ActiveLevels)
        {
            SceneManager.LoadScene("Win");
        }
        else
        {
            SceneManager.LoadScene(Level + UtilitySceneCount - 1);
        }

    }

    public void ExitBattle()
    {
        if (SceneManager.GetActiveScene().name == "EnemyVillage")
        {
            SceneManager.LoadScene("Win");
        }
        else
        {
            GetComponent<MusicManager>().ChangeMute();
            SceneManager.LoadScene("AfterBattle");
        }
    }

    public void LoseBattle()
    {
        SceneManager.LoadScene("Lose");
    }

    public void ReturnToMenu(bool gameCompleted)
    {
        if(LockedCharacters.Count > 0 && gameCompleted)
        {
            GameObject newCharcter = LockedCharacters[Random.Range(0, LockedCharacters.Count)];
            UnlockedCharacters.Add(newCharcter);
            LockedCharacters.Remove(newCharcter);
        }
        Reset();
        SceneManager.LoadScene("StartScene");
    }

    public void SetMenu(GameObject newMenu)
    {
        Menu = newMenu;
    }

    public GameObject GetMenu()
    {
        return Menu;
    }

    public void ChangeMusicVolume(float volume)
    {
        MusicVolume = volume;
    }   

    public void ChangeEffectsVolume(float volume)
    {
        EffectsVolume = volume;
    }

    public bool HasGearPool()
    {
        return GearPool != null && GearPool.Count > 0;
    }

    public void SetGearPool(List<GameObject> gearPool)
    {
        GearPool = gearPool;
    }

    public List<GameObject> GetGearPool()
    {
        return GearPool;
    }

    public bool HasSkillPool()
    {
        return SkillPool != null && SkillPool.Count > 0;
    }

    public void SetSkillPool(List<GameObject> skillPool)
    {
        SkillPool = skillPool;
    }

    public List<GameObject> GetSkillPool()
    {
        return SkillPool;
    }

    public void AddEnemySlain()
    {
        EnemiesSlain++;
    }

    public int GetEnemiesSlain()
    {
        return EnemiesSlain;
    } 

    private void Reset()
    {
        SkillPool = new List<GameObject>();
        GearPool = new List<GameObject>();
        foreach(GameObject character in SelectedCharacters)
        {
            Destroy(character);
        }
        SelectedCharacters = new List<GameObject>();
        SelectedCharacters.AddRange(UnlockedCharacters);
        while (SelectedCharacters.Count <4)
        {
            SelectedCharacters.Add(UnlockedCharacters[0]);
        }
        Level = 0;
        GetComponent<MusicManager>().ChangeMusic(MusicManager.MusicState.Main);
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Menu.SetActive(!Menu.activeSelf);
        }
    }

    public float GetMusicVolume()
    {
        return MusicVolume;
    }
    public float GetEffectsVolume()
    {
        return EffectsVolume;
    }

    public void SavePrefs()
    {
        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
        PlayerPrefs.SetFloat("EffectsVolume", EffectsVolume);
        PlayerPrefs.Save();
    }

    public void LoadPrefs()
    {
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.2f);
        EffectsVolume = PlayerPrefs.GetFloat("EffectsVolume", 0.4f);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OnApplicationQuit()
    {
        SaveGame();
    }

    void SaveGame()
    {
        List<string> characterIdentifiers = GetIdentifiers(UnlockedCharacters);
        List<string> gearIdentifiers = GetIdentifiers(UnlockedGear);
        List<string> skillIdentifiers = GetIdentifiers(UnlockedSkills);
        SaveDataComponent.SetCharacterData(characterIdentifiers);
        SaveDataComponent.SetGearData(gearIdentifiers);
        SaveDataComponent.SetSkillData(skillIdentifiers);
        SaveDataComponent.SaveToJson();
        SavePrefs();
    }

    List<string> GetIdentifiers(List<GameObject> identifiableObjects)
    {
        List<string> identifiers = new List<string>();
        foreach(GameObject identifiableObject in identifiableObjects)
        {
            if (identifiableObject.GetComponent<PlayerCharacter>())
            {
                string identifier = identifiableObject.GetComponent<PlayerCharacter>().GetSaveIdentifier();
                if(identifier != null && identifier != "")
                {
                    identifiers.Add(identifier);
                }
                else
                {
                    string name = identifiableObject.name;
                    Debug.LogError("Character " + name + " has no identifier");
                }
            }
            if(identifiableObject.GetComponent<Gear>())
            {
                string identifier = identifiableObject.GetComponent<Gear>().GetSaveIdentifier();
                if (identifier != null && identifier != "")
                {
                    identifiers.Add(identifier);
                }
                else
                {
                    string name = identifiableObject.name;
                    Debug.LogError("Gear " + name + " has no identifier");
                }
            }
            if(identifiableObject.GetComponent<Skill>())
            {
                string identifier = identifiableObject.GetComponent<Skill>().GetSaveIdentifier();
                if (identifier != null && identifier != "")
                {
                    identifiers.Add(identifier);
                }
                else
                {
                    string name = identifiableObject.name;
                    Debug.LogError("Skill " + name + " has no identifier");
                }
            }
        }
        return identifiers;
    }

    List<GameObject> GetObjectsFromIdentifiers(List<string> identifiers, SaveableType type)
    {
        List<GameObject> objects = new List<GameObject>();
        switch(type)
        {
            case SaveableType.Character:
                foreach(string identifier in identifiers)
                {
                    foreach(GameObject character in AvailableCharacters)
                    {
                        if(character.GetComponent<PlayerCharacter>().GetSaveIdentifier() == identifier)
                        {
                            objects.Add(character);
                        }
                    }
                }
                break;
            case SaveableType.Gear:
                foreach (string identifier in identifiers)
                {
                    foreach (GameObject gear in AvailableGear)
                    {
                        if (gear.GetComponent<Gear>().GetSaveIdentifier() == identifier)
                        {
                            objects.Add(gear);
                        }
                    }
                }
                break;
            case SaveableType.Skill:
                foreach (string identifier in identifiers)
                {
                    foreach (GameObject skill in AvailableSkills)
                    {
                        if (skill.GetComponent<Skill>().GetSaveIdentifier() == identifier)
                        {
                            objects.Add(skill);
                        }
                    }
                }
                break;
        }
        return objects;
    }
}

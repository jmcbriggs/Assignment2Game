using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField]
    List<GameObject> AvailableCharacters = new List<GameObject>();
    [SerializeField]
    List<GameObject> SelectedCharacters = new List<GameObject>();
    [SerializeField]
    List<GameObject> LockedCharacters = new List<GameObject>();
    [SerializeField]
    List<GameObject> AvailableEnemies = new List<GameObject>();
    [SerializeField]
    GameObject Boss;
    [SerializeField]
    int Level = 1;
    [SerializeField]
    int EnemyTypeMax = 1;
    [SerializeField]
    int UtilitySceneCount = 1;
    [SerializeField]
    int ActiveLevels = 1;
    [SerializeField, Range(0,1)]
    float MusicVolume;
    [SerializeField, Range(0, 1)]
    float EffectsVolume;
    GameObject Menu;
    List<GameObject> GearPool;
    List<GameObject> SkillPool;

    [Header("BalanceControl")]
    [SerializeField]
    float DropOffFactor = 0.5f;
    [SerializeField]
    float DefenceFactor = 0.5f;
  
    [SerializeField]
    Color[] BodyColours;

    private static int m_referenceCount = 0;

    private static GameController m_instance;

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
        SelectedCharacters.AddRange(AvailableCharacters);
        while (SelectedCharacters.Count < 4)
        {
            SelectedCharacters.Add(AvailableCharacters[0]);
        }
        ActiveLevels = SceneManager.sceneCountInBuildSettings;
        Menu = GameObject.Find("Menu");
    }

    public GameObject GetAvailableCharacter(int index)
    {
        if(index < AvailableCharacters.Count)
        {
            return AvailableCharacters[index];
        }
        else
        {
            return AvailableCharacters[0];
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
        return AvailableCharacters.Count;
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

    public float GetDropOffFactor()
    {
        return DropOffFactor;
    }

    public float GetDefenceFactor()
    {
        return DefenceFactor;
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

    public void ReturnToMenu(bool gameCompleted)
    {
        if(LockedCharacters.Count > 0 && gameCompleted)
        {
            GameObject newCharcter = LockedCharacters[Random.Range(0, LockedCharacters.Count)];
            AvailableCharacters.Add(newCharcter);
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

    private void Reset()
    {
        SkillPool = new List<GameObject>();
        GearPool = new List<GameObject>();
        foreach(GameObject character in SelectedCharacters)
        {
            Destroy(character);
        }
        SelectedCharacters = new List<GameObject>();
        SelectedCharacters.AddRange(AvailableCharacters);
        while (SelectedCharacters.Count <4)
        {
            SelectedCharacters.Add(AvailableCharacters[0]);
        }
        Level = 0;
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
}

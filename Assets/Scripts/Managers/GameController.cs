using System.Collections;
using System.Collections.Generic;
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
    int UtilitySceneCount = 1;
    [SerializeField]
    int ActiveLevels = 1;
  
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

    public Color GetCharacterColour(int index)
    {
        return BodyColours[index];
    }

    public int GetLevel()
    {
        return Level;
    }

    public void BeginGame()
    {
        for(int i = 0 ; i < SelectedCharacters.Count; i++)
        {
            if(SelectedCharacters[i])
            {
                SelectedCharacters[i] = Instantiate(SelectedCharacters[i], new Vector3(1000,1000,0), Quaternion.identity);
                BodyColour bodyColour = SelectedCharacters[i].GetComponentInChildren<BodyColour>();
                if (bodyColour != null)
                {
                    bodyColour.SetColour(GetCharacterColour(i));
                }
                DontDestroyOnLoad(SelectedCharacters[i]);
            }
        }
        EnterBattle();
    }

    public void EnterBattle()
    {
        Level++;
        if(Level >= ActiveLevels)
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
            SceneManager.LoadScene("AfterBattle");
        }
    }

    public void ReturnToMenu()
    {
        if(LockedCharacters.Count > 0)
        {
            GameObject newCharcter = LockedCharacters[Random.Range(0, LockedCharacters.Count)];
            AvailableCharacters.Add(newCharcter);
            LockedCharacters.Remove(newCharcter);
        }
        Reset();
        SceneManager.LoadScene("StartScene");
    }

    private void Reset()
    {
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
        Level = 1;
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}

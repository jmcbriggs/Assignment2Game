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
    List<GameObject> AvailableEnemies = new List<GameObject>();
    [SerializeField]
    int Level = 1;

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
        SelectedCharacters.Add(AvailableCharacters[0]);
        SelectedCharacters.Add(AvailableCharacters[1]);
        SelectedCharacters.Add(AvailableCharacters[2]);
        SelectedCharacters.Add(AvailableCharacters[3]);
    }

    public GameObject GetAvailableCharacter(int index)
    {
        return AvailableCharacters[index];
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
                SelectedCharacters[i] = Instantiate(SelectedCharacters[i]);
                BodyColour bodyColour = SelectedCharacters[i].GetComponentInChildren<BodyColour>();
                if (bodyColour != null)
                {
                    bodyColour.SetColour(GetCharacterColour(i));
                }
                DontDestroyOnLoad(SelectedCharacters[i]);
            }
        }
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
       SceneManager.LoadScene(nextSceneIndex);
    }

    public void EnterBattle()
    {
        Level++;
        SceneManager.LoadScene("SampleScene");
    }

    public void ExitBattle()
    {
        SceneManager.LoadScene("AfterBattle");
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("StartScene");
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreenManager : MonoBehaviour
{
    [SerializeField]
    GameObject button;
    [SerializeField]
    List<Transform> _characterPositions = new List<Transform>();

    [SerializeField]
    TMPro.TextMeshProUGUI _enemiesSlain;
    [SerializeField]
    TMPro.TextMeshProUGUI _damageDone;
    [SerializeField]
    TMPro.TextMeshProUGUI _damageTaken;
    [SerializeField]
    TMPro.TextMeshProUGUI _healingDone;
    [SerializeField]
    TMPro.TextMeshProUGUI _characterDefeats;
    [SerializeField]
    TMPro.TextMeshProUGUI _mvp;
    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(RevealButton), 5);
        if(GameController.Instance != null)
        {
            int damageDoneValue = 0;
            int damageTakenValue = 0;
            int healingDoneValue = 0;
            int enemiesSlainValue = GameController.Instance.GetEnemiesSlain();
            int characterDefeatsValue = 0;
            string mvpValue = null;
            int mvpNumber = 0;  
           for(int i = 0; i < GameController.Instance.GetSelectedCharacters().Count; i++)
           {
                GameObject character = GameController.Instance.GetSelectedCharacters()[i];
                ChangeChildLayers(character.transform, 5);
                character.transform.position = _characterPositions[i].position;
                damageDoneValue += character.GetComponent<PlayerCharacter>().GetDamageDone();
                damageTakenValue += character.GetComponent<PlayerCharacter>().GetDamageTaken();
                healingDoneValue += character.GetComponent<PlayerCharacter>().GetHealingDone();
                characterDefeatsValue += character.GetComponent<PlayerCharacter>().GetDefeats();
                if(healingDoneValue + damageDoneValue > mvpNumber)
                {
                    mvpNumber = healingDoneValue + damageDoneValue;
                    mvpValue = character.GetComponent<PlayerCharacter>().GetName();
                }
                if(SceneManager.GetActiveScene().name == "Win")
                {
                    character.GetComponent<Animator>().SetBool("Celebrate", true);
                    GameController.Instance.GetComponent<MusicManager>().ChangeMusic(MusicManager.MusicState.Win);
                }
                else
                {
                    character.GetComponent<Animator>().SetBool("Celebrate", false);
                }
                
            }
           _damageDone.text =  damageDoneValue.ToString();
            _damageTaken.text = damageTakenValue.ToString();
            _healingDone.text = healingDoneValue.ToString();
            _enemiesSlain.text = enemiesSlainValue.ToString();
            _characterDefeats.text = characterDefeatsValue.ToString();
            _mvp.text = mvpValue;
        }
    }

    void RevealButton()
    {
        button.SetActive(true);
    }

    public void ReturnToMenu()
    {
        if(GameController.Instance != null)
        {
            GameController.Instance.ReturnToMenu(true);
        }
    }

    public void ChangeChildLayers(Transform parent, int layer)
    {
        parent.gameObject.layer = layer;
        foreach (Transform child in parent)
        {
            if (child.gameObject.name != "HealthBar")
            {
                ChangeChildLayers(child, layer);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class GearAssigner : MonoBehaviour
{
    // Start is called before the first frame update
    

    [SerializeField]
    List<GearBox> _gearBox = new List<GearBox>();
    List<GameObject> _gearPool = new List<GameObject>();
    GameObject _currentCharacter;
    GameObject _characterPrefab;
    AssignerControl _assignerControl;

    [Header("Character Info")]
    [SerializeField]
    TextMeshProUGUI _characterHealth;
    [SerializeField]
    TextMeshProUGUI _characterMovement;
    [SerializeField]
    TextMeshProUGUI _characterAttack;
    [SerializeField]
    TextMeshProUGUI _characterDefence;
    [SerializeField]
    TextMeshProUGUI _characterMagic;
    public void Initialise(GameObject character, GameObject characterPrefab)
    {
        if (_gearBox == null || _gearBox.Count == 0)
        {
            _gearBox = new List<GearBox>(GetComponentsInChildren<GearBox>());
        }
        _currentCharacter = character;
        _characterPrefab = characterPrefab;
        _assignerControl = GetComponent<AssignerControl>();
        UpdateUI();
        //Invoke("SetCharacterGear", 0.1f);
    }

    public void SetGearPool(List<GameObject> gearPool)
    {
        _gearPool = gearPool;
    }

    public void UpdateUI()
    {
        if (_currentCharacter != null)
        {
            PlayerCharacter character = _currentCharacter.GetComponent<PlayerCharacter>();
            if (character != null)
            {
                for (int i = 0; i < _gearBox.Count; i++)
                {
                    List<GameObject> gear = character.GetGear();
                    GameObject selectedGear =  gear.Find(x => x.GetComponent<Gear>().GetGearType() == _gearBox[i].GetGearType());
                    if (selectedGear != null)
                    {
                        string gearName = selectedGear.name;
                        if (gearName != null && gearName.Contains("(Clone)"))
                        {
                            gearName = gearName.Replace("(Clone)", "");
                        }
                        GameObject gearPiece = _gearPool.Find(x => x.name == gearName);
                        if(gearPiece != null)
                        {
                            _gearBox[i].SetGear(gearPiece);
                        }
                        else
                        {
                            Debug.LogError("Gear not found in pool: " + gearName);
                        }
                    }
                    else
                    {
                        _gearBox[i].SetGear(null);
                    }
                }
                UpdateStats(character);               
            }
        }
    }

    private void UpdateStats(PlayerCharacter playerCharacter)
    {
        _characterHealth.text = "Max Health: " + playerCharacter.GetMaxHealth();
        _characterMovement.text = "Movement: " + playerCharacter._movement;
        _characterAttack.text = "Attack: " + playerCharacter.GetAttack();
        _characterDefence.text = "Defence: " + playerCharacter.GetDefence();
        _characterMagic.text = "Magic: " + playerCharacter._magic;
    }

    private void UpdatePrefab(GameObject character) { 

        if (character != null)
        {
            PlayerCharacter playerCharacter = character.GetComponent<PlayerCharacter>();
            if (playerCharacter != null)
            {
                for (int i = 0; i < _gearBox.Count; i++)
                {
                    GameObject gearPiece = _gearBox[i].GetGear();
                    if (gearPiece != null)
                    {
                        playerCharacter.OnEquip(gearPiece, false);
                    }
                    else
                    {
                        playerCharacter.OnUnequip(_gearBox[i].GetGearType());
                    }

                }
            }
        }
    }

    public void SetCharacterGear()
    {
        UpdatePrefab(_currentCharacter);
        UpdatePrefab(_characterPrefab);
        _assignerControl.ChangeChildLayers(_characterPrefab.transform, 5);
        UpdateStats(_currentCharacter.GetComponent<PlayerCharacter>());
    }

}

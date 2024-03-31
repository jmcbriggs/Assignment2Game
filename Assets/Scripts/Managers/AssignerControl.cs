using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignerControl : MonoBehaviour
{
    [SerializeField]
    GameObject SkillUI;
    [SerializeField]
    GameObject GearUI;
    SkillAssigner SkillAssign;
    GearAssigner GearAssign;
    [SerializeField]
    GameObject CurrentCharacter;
    GameObject _characterPrefab;

    [SerializeField]
    int SelecterIndex = 0;
    [SerializeField]
    float CharacterScale = 1.0f;

    [SerializeField]
    private Transform _prefabPosition;

    [SerializeField]
    TMPro.TextMeshProUGUI _characterName;
    // Start is called before the first frame update
    void Start()
    {
        if (GameController.Instance != null)
        {
            CurrentCharacter = GameController.Instance.GetSelectedCharacters()[SelecterIndex];

        }
        else
        {
            CurrentCharacter = Instantiate(CurrentCharacter, new Vector3(1000, 1000, 1000), Quaternion.identity);
        }
        if(SkillAssign == null)
        {
            SkillAssign = GetComponentInChildren<SkillAssigner>();
        }
        if(GearAssign == null)
        {
            GearAssign = GetComponentInChildren<GearAssigner>();
        }
        CreateInitialPrefab();
        SkillAssign.Initialise(CurrentCharacter);
        GearAssign.Initialise(CurrentCharacter, _characterPrefab);
        _characterName.text = CurrentCharacter.GetComponent<PlayerCharacter>().GetName();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateInitialPrefab()
    {
        if (_characterPrefab != null)
        {
            Destroy(_characterPrefab);
        }
        _characterPrefab = Instantiate(CurrentCharacter, _prefabPosition.position, _prefabPosition.rotation);
        _characterPrefab.transform.SetParent(_prefabPosition);
        _characterPrefab.transform.localScale = new Vector3(CharacterScale, CharacterScale, CharacterScale);
        ChangeChildLayers(_characterPrefab.transform, 5);
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

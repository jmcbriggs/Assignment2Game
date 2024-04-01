using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    [SerializeField]
    private CharacterData _characterData;
    [SerializeField]
    private GearData _gearData;
    [SerializeField]
    private SkillData _skillData;

    public void SaveToJson()
    {
        string characterJson = JsonUtility.ToJson(_characterData);
        string gearJson = JsonUtility.ToJson(_gearData);
        string skillJson = JsonUtility.ToJson(_skillData);

        System.IO.File.WriteAllText(Application.persistentDataPath + "/characterData.json", characterJson);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/gearData.json", gearJson);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/skillData.json", skillJson);
    }

    public void UpdateCharacterData(string character)
    {
        _characterData.unlockedCharacters.Add(character);
    }

    public void SetCharacterData(List<string> characters)
    {
        _characterData.unlockedCharacters = characters;
    }

    public void UpdateGearData(string gear)
    {
        _gearData.unlockedGear.Add(gear);
    }

    public void SetGearData(List<string> gear)
    {
        _gearData.unlockedGear = gear;
    }

    public void UpdateSkillData(string skill)
    {
        _skillData.unlockedSkills.Add(skill);
    }

    public void SetSkillData(List<string> skills)
    {
        _skillData.unlockedSkills = skills;
    }

    public void LoadFromJson()
    {
        string characterFilePath = Application.persistentDataPath + "/characterData.json";
        string gearFilePath = Application.persistentDataPath + "/gearData.json";
        string skillFilePath = Application.persistentDataPath + "/skillData.json";
        if(System.IO.File.Exists(characterFilePath))
        {
            string characterJson = System.IO.File.ReadAllText(characterFilePath);
            _characterData = JsonUtility.FromJson<CharacterData>(characterJson);
        }
        if (System.IO.File.Exists(gearFilePath))
        {
            string gearJson = System.IO.File.ReadAllText(gearFilePath);
            _gearData = JsonUtility.FromJson<GearData>(gearJson);
        }
        if (System.IO.File.Exists(skillFilePath))
        {
            string skillJson = System.IO.File.ReadAllText(skillFilePath);
            _skillData = JsonUtility.FromJson<SkillData>(skillJson);
        }
    }

    public List<string> GetUnlockedCharacters()
    {
        return _characterData.unlockedCharacters;
    }

    public List<string> GetUnlockedSkills()
    {
        return _skillData.unlockedSkills;
    }

    public List<string> GetUnlockedGear()
    {
        return _gearData.unlockedGear;
    }

}

[System.Serializable]
public class CharacterData
{
    public List<string> unlockedCharacters;
}

[System.Serializable]
public class GearData
{
    public List<string> unlockedGear;
}

[System.Serializable]
public class SkillData
{
    public List<string> unlockedSkills;
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GearDescriptionBox : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI _gearName;
    [SerializeField]
    TextMeshProUGUI _gearDescription;
    [SerializeField]
    TextMeshProUGUI _gearType;
    [SerializeField]
    TextMeshProUGUI _gearStats;
  
    // Start is called before the first frame update
    public void Set(Gear gear)
    {
        _gearName.text = "Gear Name: " + gear.GetGearName();
        _gearDescription.text = "Description: " + gear.GetGearDescription();
        _gearType.text = "Type: " + gear.GetGearType().ToString();
        Dictionary<Gear.StatType, int> stats = gear.GetStats();
        string statsString = "Stats: ";
        foreach (KeyValuePair<Gear.StatType, int> stat in stats)
        {
            if(statsString.Length > 10)
            {
                statsString += "\n";
            }
            statsString += stat.Key.ToString() + ": " + stat.Value + " ";
        }
        _gearStats.text = statsString;
    }

    public void Clear()
    {
        _gearName.text = "Gear Name: ";
        _gearDescription.text = "Description: ";
        _gearType.text = "Type: ";
        _gearStats.text = "Stats: ";
;
    }
}

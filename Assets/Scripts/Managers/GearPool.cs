using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GearPool : MonoBehaviour
{
    [SerializeField]
    List<GearBox> _gearBoxes = new List<GearBox>();
    [SerializeField]
    List<GameObject> _gearPool = new List<GameObject>();

    void Start()
    {
        if (_gearBoxes == null || _gearBoxes.Count == 0)
        {
            _gearBoxes = new List<GearBox>(GetComponentsInChildren<GearBox>());
        }
    }

    private void UpdateUI()
    {

        for (int i = 0; i < _gearBoxes.Count; i++)
        {
            if (i < _gearPool.Count)
            {
                _gearBoxes[i].SetGear(_gearPool[i]);
            }
            else
            {
                _gearBoxes[i].SetGear(null);
            }
        }
    }

    public void SetGearPool(List<GameObject> gearPool)
    {
        _gearPool = gearPool;
        UpdateUI();
    }


}

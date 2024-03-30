using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField]
    private VolumeType volumeType;
    enum VolumeType
    {
        Music,
        SFX
    }
    // Start is called before the first frame update
    void Start()
    {
        if(GameController.Instance == null)
        {
            return;
        }
        Slider slider = GetComponent<Slider>();
        slider.onValueChanged.RemoveAllListeners();
        if (volumeType == VolumeType.SFX)
        {
            slider.onValueChanged.AddListener(GameController.Instance.ChangeEffectsVolume);
        }

        else
        {
            slider.onValueChanged.AddListener(GameController.Instance.ChangeMusicVolume);
        }

    }

    private void OnEnable()
    {
        if(volumeType == VolumeType.SFX)
        {
            GetComponent<Slider>().value = GameController.Instance.GetEffectsVolume();
        }
        else
        {
            GetComponent<Slider>().value = GameController.Instance.GetMusicVolume();
        }
    }

}

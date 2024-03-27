using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMusic : MonoBehaviour
{

    [SerializeField] MusicManager.MusicState state;
    // Start is called before the first frame update
    void Start()
    {
        if(GameController.Instance != null)
        {
            GameController.Instance.GetComponent<MusicManager>().ChangeMusic(state);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

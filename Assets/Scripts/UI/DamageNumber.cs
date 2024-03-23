using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    public float _speed;
    public float _lifeTime;
    public float _counter;   

    private Color _colour;
    // Start is called before the first frame update
    void Start()
    {
        _counter = 0;
        StartCoroutine(Animate());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetColour(Color colour)
    {
        _colour = colour;
    }

    IEnumerator Animate()
    {
        while (_counter < _lifeTime)
        {
            _counter += Time.deltaTime;
            transform.position += new Vector3(0, _speed * Time.deltaTime, 0);
            GetComponent<TextMeshPro>().color = new Color(_colour.r, _colour.g, _colour.b, 1 - (_counter / _lifeTime));
            yield return null;
        }
        Destroy(gameObject);
    }
}

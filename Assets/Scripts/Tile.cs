using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    GameObject innerSquare;
    [SerializeField]
    CombatManager _combatManager;
    [SerializeField]
    GameObject _occupant;
    SpriteRenderer spriteRenderer;

    public int _x;
    public int _y;
    public float _priority;
    public bool _isWalkable;
    public GameObject _parent;

    void Start()
    {
        spriteRenderer = innerSquare.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnMouseEnter()
    {
        if (_combatManager != null)
        {
            _combatManager.OverTile(gameObject);
        }
    }

    private void OnMouseDown()
    {
        if (_combatManager != null)
        {
            _combatManager.TileClick(gameObject);
        }
    }

    public void ColourChange(Color color)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }

    }

    public void SetCombatManager(CombatManager combatManager)
    {
        _combatManager = combatManager;
    }

    public void SetOccupant(GameObject occupant)
    {
        _occupant = occupant;
        _isWalkable = false;
    }   

    public void RemoveOccupant()
    {
        _occupant = null;
        _isWalkable = true;
    }

    public GameObject GetOccupant()
    {
        return _occupant;
    }

}

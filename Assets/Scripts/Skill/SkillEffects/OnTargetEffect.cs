using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OnTargetEffect : SkillEffect
{
    [SerializeField] private GameObject _hitEffect;
    [SerializeField] private float _scale;
    // Start is called before the first frame update
    void Start()
    {
    }

    public override void TriggerEffect(Transform user, Transform singleTarget, List<Transform> targets)
    {
        //Creates projectile and sets its direction and speed, when it reaches the target, it triggers the hit effect
        Vector3 pos = singleTarget.position;
        if(singleTarget.GetComponent<Tile>() != null)
        {
            GameObject occupant = singleTarget.GetComponent<Tile>().GetOccupant();
            if (occupant != null)
            {
                pos = occupant.transform.position;
            }
        }
        GameObject proj = Instantiate(_hitEffect, pos, Quaternion.identity);
        List<Transform> hitParticleChildren = proj.GetComponentsInChildren<Transform>().ToList();
        if (hitParticleChildren.Count > 0)
        {
            foreach (Transform child in hitParticleChildren)
            {
                child.localScale = new Vector3(_scale, _scale, _scale);
            }
        }
        Destroy(proj, 5f);
        user.GetComponent<Character>().FinishAttack();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

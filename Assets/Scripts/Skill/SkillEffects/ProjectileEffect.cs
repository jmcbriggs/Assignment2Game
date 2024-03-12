using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileEffect : SkillEffect
{
    [SerializeField] private float _speed;
    [SerializeField] private GameObject _projectile;
    [SerializeField] private GameObject _hitEffect;
    [SerializeField] private float _scale;
    [SerializeField] private float _explodeScale;
    // Start is called before the first frame update
    void Start()
    {
    }

    public override void TriggerEffect(Transform user, Transform singleTarget, List<Transform> targets)
    {
        //Creates projectile and sets its direction and speed, when it reaches the target, it triggers the hit effect
        GameObject proj = Instantiate(_projectile, user.position, Quaternion.identity);
        Vector3 target = singleTarget.position;
        if(singleTarget.GetComponent<Tile>() != null)
        {
            target = target + new Vector3(0, 0.5f, 0);
        }
        proj.transform.localScale = new Vector3(_scale, _scale, _scale);
        proj.GetComponent<Rigidbody>().velocity = (target - user.position).normalized * _speed;
        StartCoroutine(TriggerHitEffect(proj, target, user));
    }

    private IEnumerator TriggerHitEffect(GameObject proj, Vector3 target, Transform user)
    {
        //Waits until the projectile reaches the target and triggers the hit effect
        while (Vector3.Distance(proj.transform.position, target) > 0.1f)
        {
            yield return null;
        }
        GameObject hitParticle =  Instantiate(_hitEffect, target, Quaternion.identity);
        hitParticle.transform.localScale = new Vector3(_explodeScale, _explodeScale, _explodeScale);
        List<Transform> hitParticleChildren = hitParticle.GetComponentsInChildren<Transform>().ToList();
        if(hitParticleChildren.Count > 0)
        {
            foreach (Transform child in hitParticleChildren)
            {
                child.localScale = new Vector3(_explodeScale, _explodeScale, _explodeScale);
            }
        }
        Destroy(hitParticle, 5f);
        Destroy(proj);
        user.GetComponent<Character>().FinishAttack();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

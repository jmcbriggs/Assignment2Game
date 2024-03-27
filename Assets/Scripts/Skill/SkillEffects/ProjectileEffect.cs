using FMODUnity;
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
    [SerializeField] private EventReference _fmodCast;
    [SerializeField] private EventReference _fmodHit;
    FMOD.Studio.EventInstance _castInstance;
    FMOD.Studio.EventInstance _hitInstance;
    StudioEventEmitter _fmodEmitter;
    // Start is called before the first frame update
    void Start()
    {
        _fmodEmitter = GetComponent<StudioEventEmitter>();
        if (!_fmodCast.IsNull)
        {
            _castInstance = RuntimeManager.CreateInstance(_fmodCast);

        }
        else
        {
            Debug.LogError("No cast sound assigned to " + gameObject.name);
        }
        if (!_fmodHit.IsNull)
        {
            _hitInstance = RuntimeManager.CreateInstance(_fmodHit);
        }
        else
        {
            Debug.LogError("No hit sound assigned to " + gameObject.name);
        }
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

    public override void TriggerCastSound()
    {
        if (_castInstance.isValid())
        {
            _castInstance.start();
        }
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
        if (_castInstance.isValid())
        {
            _castInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            _castInstance.setTimelinePosition(0);
        }
        if (_hitInstance.isValid())
        {
            _hitInstance.start();
        }
        Destroy(hitParticle, 5f);
        Destroy(proj);
        user.GetComponent<Character>().FinishAttack();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameController.Instance != null)
        {
            _castInstance.setParameterByName("Volume", GameController.Instance.GetEffectsVolume());
            _hitInstance.setParameterByName("Volume", GameController.Instance.GetEffectsVolume());
        }
    }
}

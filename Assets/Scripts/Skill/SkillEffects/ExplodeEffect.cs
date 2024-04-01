using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExplodeEffect : SkillEffect
{
    [SerializeField] private GameObject _projectile;
    [SerializeField] private GameObject _hitEffect;
    [SerializeField] private float _scale;
    [SerializeField] private float _explodeScale;
    [SerializeField] private EventReference _fmodCast;
    FMOD.Studio.EventInstance _castInstance;
    // Start is called before the first frame update
    void Start()
    {
        if (!_fmodCast.IsNull)
        {
            _castInstance = RuntimeManager.CreateInstance(_fmodCast);

        }
        else
        {
            Debug.LogError("No cast sound assigned to " + gameObject.name);
        }
    }

    public override void TriggerEffect(Transform user, Transform singleTarget, List<Transform> targets)
    {
        //Creates projectile and sets its direction and speed, when it reaches the target, it triggers the hit effect
        GameObject proj = Instantiate(_projectile, user.position, Quaternion.identity);
        proj.transform.localScale = new Vector3(_scale, _scale, _scale);
        List<Transform> particleChildren = proj.GetComponentsInChildren<Transform>().ToList();
        if (particleChildren.Count > 0)
        {
            foreach (Transform child in particleChildren)
            {
                child.localScale = new Vector3(_scale, _scale, _scale);
            }
        }
        TriggerHitEffect(proj, targets, user);
    }

    private void TriggerHitEffect(GameObject proj, List<Transform> targets, Transform user)
    {
        List<GameObject> hitParticles = new List<GameObject>();
        if (_hitEffect != null)
        {
            foreach (Transform target in targets)
            {
                hitParticles.Add(Instantiate(_hitEffect, target.position, Quaternion.identity));
                hitParticles.Last().transform.localScale = new Vector3(_explodeScale, _explodeScale, _explodeScale);
                List<Transform> hitParticleChildren = hitParticles.Last().GetComponentsInChildren<Transform>().ToList();
                if (hitParticleChildren.Count > 0)
                {
                    foreach (Transform child in hitParticleChildren)
                    {
                        child.localScale = new Vector3(_explodeScale, _explodeScale, _explodeScale);
                    }
                }
                Destroy(hitParticles.Last(), 5f);
            }
        }
        Destroy(proj, 5f);
        user.GetComponent<Character>().FinishAttack(1);
    }

    public override void TriggerCastSound()
    {
        if (_castInstance.isValid())
        {
            _castInstance.start();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.Instance != null)
        {
            _castInstance.setParameterByName("Volume", GameController.Instance.GetEffectsVolume());
        }
    }

}

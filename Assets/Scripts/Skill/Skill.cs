using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Skill : MonoBehaviour
{
    [SerializeField]
    string _skillName;
    [SerializeField]
    int _power;
    [SerializeField]
    int _range;
    [SerializeField]
    int _splash;
    [SerializeField]
    int _cooldown;
    [SerializeField]
    int _cooldownCounter;
    [SerializeField]
    string _skillDescription;
    [SerializeField]
    int _level;
    [SerializeField]
    AnimationType _animationType;
    
    public enum SkillType
    {
        LINE,
        CROSS,
        AREA
    }

    public enum SkillHitType
    {
        POINT,
        DIRECTIONAL,
        AREA
    }

    public enum SkillTarget
    {
        ENEMY,
        FRIENDLY
    }

    public enum AnimationType
    {
        Attack,
        OffensiveSpell,
        DefensiveSpell,
    }

    [SerializeField]
    SkillType _skillType;
    [SerializeField]
    SkillTarget _skillTarget;
    [SerializeField]
    SkillHitType _skillHitType;
    [SerializeField]
    bool _isMagic;
    [SerializeField]
    bool _extraEffect;
    // Start is called before the first frame update
    void Start()
    {
        if(_skillType == SkillType.AREA && _skillHitType == SkillHitType.DIRECTIONAL)
        {
            Debug.LogError("Area skills cannot be directional");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetSkillName()
    {
        return _skillName;
    }

    public SkillType GetSkillType()
    {
        return _skillType;
    }

    public int GetRange()
    {
        return _range;
    }

    public int GetPower()
    {
        return _power;
    }


    public Skill.SkillTarget GetSkillTarget()
    {
        return _skillTarget;
    }

    public int GetSplash()
    {
        return _splash;
    }   

    public int GetLevel()
    {
        return _level;
    }

    public SkillHitType GetSkillHitType()
    {
        return _skillHitType;
    }

    public bool IsMagic()
    {
        return _isMagic;
    }
    public bool HasExtraEffect()
    {
        return _extraEffect;
    }
    public int GetCooldown()
    {
        return _cooldown;
    }
    public int GetCooldownCounter()
    {
        return _cooldownCounter;
    }
    public bool OnCooldown()
    {
        return _cooldownCounter > 0;
    }
    public void StartCooldown()
    {
        _cooldownCounter = _cooldown;
    }
    public void DecrementCooldown()
    {
        _cooldownCounter--;
    }
    public void ResetCooldown()
    {
        _cooldownCounter = 0;
    }

    public string GetSkillDescription()
    {
        return _skillDescription;
    }

    public AnimationType GetAnimationType()
    {
        return _animationType;
    }

    public void TriggerExtraEffect(Transform user, Transform target)
    {
        if(_extraEffect)
        {
            SkillEffect effect = GetComponent<SkillEffect>();
            effect.TriggerEffect(user, target, null);
        }
        else { 
            Debug.LogError("This skill has no extra effect");
        }
    }

    public  void TriggerExtraEffect(Transform user, Transform singleTarget, List<GameObject> targets)
    {
        if (_extraEffect)
        {
            SkillEffect effect = GetComponent<SkillEffect>();
            List<Transform> targetsTransforms = new List<Transform>();
            foreach (GameObject target in targets)
            {
                targetsTransforms.Add(target.transform);
            }
            effect.TriggerEffect(user, singleTarget, targetsTransforms);
        }
        else
        {
            Debug.LogError("This skill has no extra effect");
        }
    }
}

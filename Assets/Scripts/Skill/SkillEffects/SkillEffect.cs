using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillEffect : MonoBehaviour
{
    public abstract void TriggerEffect(Transform user, Transform singleTarget, List<Transform> targets);
    public abstract void TriggerCastSound();
}

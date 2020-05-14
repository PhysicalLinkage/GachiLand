using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SlowPulse : PulseBase
{
    public float Damage = 5f;
    protected override ManaSphereFactory ManaSphereFactory => FactoryManager.BlueSphereFactory;

    protected override Action<GameObject, Collision> CollisionAction => (gameObject, collision) =>
    {
        var target = collision.gameObject.GetComponent<Character>();
        if (target != null)
        {
            m_Character.Attack(target, Damage);
        }
    };
}

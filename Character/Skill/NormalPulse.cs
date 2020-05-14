using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NormalPulse : PulseBase
{
    public float Damage = 10;

    protected override ManaSphereFactory ManaSphereFactory => FactoryManager.WhiteSphereFactory;

    protected override Action<GameObject, Collision> CollisionAction => (gameObject, collision) => 
    {
        var character = collision.gameObject.GetComponent<Character>();
        m_Character.Attack(character, Damage);

        gameObject.SetActive(false);
    };

    public void PulseEnd()
    {
        Resume();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;

public class CollisionActioner : MonoBehaviour
{
    public Action<GameObject, Collision> Action { get; set; }

    private void Awake()
    {
        this.OnCollisionEnterAsObservable()
            .Subscribe(collision => Action(gameObject, collision));
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class WorldManager : MonoBehaviour
{
    public GameObject GameObject = null;

    private void Start()
    {
        this.OnCollisionEnterAsObservable()
            .Where(collision => collision.collider.CompareTag("Player"))
            .Subscribe(_ => GameObject.SetActive(true));
    }
}

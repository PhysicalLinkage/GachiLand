using UnityEngine;
using System.Linq;
using System;
using UniRx;

[RequireComponent(typeof(Rigidbody))]
public class ManaSphere : MonoBehaviour
{
    public Rigidbody Rigidbody { get; private set; }

    public float LifeTime { get; set; } = 0f;

    public float Speed { get; set; } = 8f;

    public Vector3 Direction { get; set; } = Vector3.zero;

    public Action OnDeath { get; set; } = () => { };

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        Observable.Timer(TimeSpan.FromSeconds(LifeTime))
            .Do(_ => OnDeath())
            .Do(_ => OnDeath = () => { })
            .Subscribe(_ => gameObject.SetActive(false))
            .AddTo(this);

        Rigidbody.velocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        var velocity = Speed * Direction;
        var acceleration = (velocity - Rigidbody.velocity) / Time.fixedDeltaTime;
        var force = Rigidbody.mass * acceleration;
        Rigidbody.AddForce(force);
    }
}

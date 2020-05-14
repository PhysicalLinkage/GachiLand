using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Linq;
using System;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Character))]
public class Move : MonoBehaviour
{
    public Vector3 targetPoint { get; private set; }

    Character character;

    private float FirstSpeed => character.Speed.Value * character.Speed.Value * 4f;

    void Awake()
    {
        targetPoint = transform.position;

        character = GetComponent<Character>();

        character.IsAlive
            .Where(isAlive => !isAlive)
            .Subscribe(_ => Stop());

        this.OnEnableAsObservable()
            .Subscribe(_ => targetPoint = transform.position);

        Stop();

        character.Rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

        character.Animator.applyRootMotion = false;
    }

    private void Start()
    {
        targetPoint = transform.position;
    }

    void FixedUpdate()
    {
        if (!character.CanAct.Value)
        {
            return;
        }

        // 動いていなかったら何もしない
        if (character.Animator.GetFloat("MoveSpeed") <= 0f)
        {
            return;
        }

        // 自分の位置から目的の位置の方向
        var direction = targetPoint - character.Rigidbody.position;

        // 目的地までの距離 と このフレームで進む距離 を比較
        if (direction.magnitude < FirstSpeed * Time.fixedDeltaTime)
        {
            // 目的地までの距離がこのフレームで進む距離より短いなら
            // 自分の位置を目的地まで移動、
            // 止まる
            character.Rigidbody.position = targetPoint;
            Stop();
        }
        else
        {
            // 目的地までの距離がこのフレームで進む距離より長いなら
            // 目的地に向かってスピードに応じて力を加える
            //
            // 力のは以下の公式から求める
            // v = v0 + at  <=> a = (v0 - v) / t
            // f = ma
            transform.LookAt(targetPoint);
            var velocity = FirstSpeed * direction.normalized;
            var acceleration = (velocity - character.Rigidbody.velocity) / Time.fixedDeltaTime;
            var force = character.Rigidbody.mass * acceleration;
            character.Rigidbody.AddForce(force);
        }
    }

    public void SetTarget(Vector3 point)
    {
        // 目的地の高さを自分のいる座標の高さに合わせる
        point.y = character.Rigidbody.position.y;


        // 目的地までの距離を求める
        var distance = Vector3.Distance(point, character.Rigidbody.position);

        // 目的地までの距離が自分の半径の半分より短いなら何もしない
        if (distance < (character.Collider.bounds.size.z * transform.localScale.z / 4f))
        {
            return;
        }

        // 目的地を向く
        transform.LookAt(point);

        // 目的地までの距離とこのフレームで進む距離と比較
        if (distance > FirstSpeed * Time.fixedDeltaTime)
        {
            // 目的地までの距離のが長い時
            // 目的地を設定
            // 歩くアニメーション
            targetPoint = point;
            character.Animator.SetFloat("MoveSpeed", character.Speed.Value);
        }
        else
        {
            // このフレームで進む距離のが長いなら
            // 自分の座標を指定された座標に設定
            // 止まる
            character.Rigidbody.position = point;
            Stop();
        }
    }

    public void Stop()
    {
        targetPoint = character.Rigidbody.position;
        character.Rigidbody.velocity = Vector3.zero;
        character.Animator.SetFloat("MoveSpeed", 0f);
    }
}

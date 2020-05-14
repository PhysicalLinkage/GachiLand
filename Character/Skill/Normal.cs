using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;

public class Normal : MonoBehaviour
{
    private Move move;
    private Character character;

    private ReactiveProperty<Character> Target { get; } = new ReactiveProperty<Character>(null);

    [SerializeField] private float NormalAttackRange = 4f;

    private IDisposable Disposable { get; set; }

    private void Awake()
    {
        move = gameObject.AddComponent<Move>();
        character = GetComponent<Character>();

        Target
            .Where(target => target != null)
            .Select(target => target.IsAlive
                .Where(isAlive => !isAlive)
                .Do(_ => Target.Value = null)
                .Where(_ => character.Animator.GetCurrentAnimatorStateInfo(0).IsName("NormalAttack"))
                .Subscribe(_ => character.Animator.SetTrigger("Normal")))
            .Subscribe(disposable => Disposable = disposable);

        Target
            .Where(target => target == null)
            .Subscribe(_ => Disposable?.Dispose());

        this.UpdateAsObservable()
            .Where(_ => character.CanAct.Value)
            .Where(_ => character.Animator.GetCurrentAnimatorStateInfo(0).IsName("Normal"))
            .Where(_ => Target.Value != null && Target.Value != character)
            .Subscribe(_ =>
            {
                if (InNormalAttackRange(Target.Value.transform))
                {
                    transform.LookAt(Target.Value.transform);
                    move.SetTarget(transform.position);
                    move.Stop();
                    character.Animator.SetTrigger("NormalAttack");
                }
                else
                {
                    move.SetTarget(Target.Value.transform.position);
                }

            });
    }

    public void Action() => character.CommandAction = CommandSkill;

    public void CommandSkill(Command command)
    {

        if (!character.CanAct.Value)
        {
            return;
        }

        if (command.Target == null)
        {
            move.SetTarget(command.Point);
            Target.Value = null;
            if (!character.Animator.GetCurrentAnimatorStateInfo(0).IsName("Normal"))
            {
                character.Animator.SetTrigger("Normal");
            }
        }
        else
        {
            var newTarget = command.Target.GetComponent<Character>();

            if (newTarget == Target.Value)
            {
                if (character.Animator.GetCurrentAnimatorStateInfo(0).IsName("Normal"))
                {
                    if (InNormalAttackRange(Target.Value.transform))
                    {
                        move.Stop();
                    }
                    else
                    {
                        move.SetTarget(command.Target.transform.position);
                    }
                }
            }
            else if (newTarget != this)
            {
                Target.Value = newTarget;
                if (InNormalAttackRange(Target.Value.transform))
                {
                    if (!character.Animator.GetCurrentAnimatorStateInfo(0).IsName("Normal"))
                        character.Animator.SetTrigger("Normal");
                    move.Stop();
                }
                else
                {
                    move.SetTarget(command.Target.transform.position);
                }
            }
        }
    }

    private bool InNormalAttackRange(Transform target)
    {
        return Vector3.Distance(target.position, transform.position) <= NormalAttackRange;
    }

    public void OnBeginNormalAttack()
    {
        if (Target.Value != null)
        {
            transform.LookAt(Target.Value.transform);
        }
        else
        {
            character.Animator.SetTrigger("Normal");
        }
    }

    public void OnNormalAttack()
    {
        if (Target.Value != null)
        {
            character.Attack(Target.Value, 4f);
        }
        else
        {
            character.Animator.SetTrigger("Normal");
        }
    }

    public void OnEndNormalAttack()
    {
        if (Target.Value != null)
        {
            if (!InNormalAttackRange(Target.Value.transform))
            {
                character.Animator.SetTrigger("Normal");
            }
        }
        else
        {
            character.Animator.SetTrigger("Normal");
        }
    }
}

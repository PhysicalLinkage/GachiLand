using UnityEngine;
using System.Linq;
using System;
using UniRx;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Character))]
[RequireComponent(typeof(Normal))]
[RequireComponent(typeof(Move))]
public abstract class PulseBase : MonoBehaviour
{
    [SerializeField] float LifeTime = 4f;
    [SerializeField] float Power = 5f;
    protected abstract ManaSphereFactory ManaSphereFactory { get; }
    protected abstract Action<GameObject, Collision> CollisionAction { get; }

    Subject<Unit> m_AnimatorSubject;
    
    Animator m_Animator;
    Rigidbody m_Rigidbody;
    Collider m_Collider;
    public Character m_Character;
    Normal m_Normal;
    Move m_Move;

    Vector3 targetPoint;

    ManaSphere ManaSphere = null;

    Vector3 direction;

    public void Skill()
    {
        if (!m_Character.CanAct.Value)
        {
            return;
        }

        m_Character.CommandAction = command =>
        {
            Release(command.Point);
            m_Character.CommandAction = (nextCommand) => { Releasing(nextCommand.Point); };
        };
    }

    void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<Collider>();
        m_Character = GetComponent<Character>();
        m_Normal = GetComponent<Normal>();
        m_Move = GetComponent<Move>();
    }

    public void Release(Vector3 point)
    {

        targetPoint = m_Move.targetPoint;

        m_Move.Stop();

        m_Rigidbody.transform.LookAt(point);

        ManaSphere = ManaSphereFactory.Generate(LifeTime);

        float offset = m_Collider.bounds.size.z * transform.localScale.z;
        Vector3 position = transform.position + offset * transform.forward.normalized;
        ManaSphere.transform.position = position + Vector3.up * m_Collider.bounds.size.y * transform.localScale.y / 2f;

        ManaSphere.GetComponent<CollisionActioner>().Action = CollisionAction;


        m_Animator.SetTrigger("Attack");
        ManaSphere.Speed = Power;
        ManaSphere.Direction = transform.forward.normalized;
        ManaSphere.OnDeath = () => { ManaSphere = null; };
    }

    public void Releasing(Vector3 point)
    {
        targetPoint = point;
    }

    public void Resume()
    {
        m_Move.SetTarget(targetPoint);
        m_Character.CommandAction = m_Normal.CommandSkill;
    }
}

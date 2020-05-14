using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Latte;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider))]
public class Character : MonoBehaviour
{
    public static Dictionary<UInt32, Character> Instances = new Dictionary<UInt32, Character>();

    bool isInitialized = false;
    Actor Actor { get; set; }

    public FloatReactiveProperty HP { get; } = new FloatReactiveProperty(100f);
    public FloatReactiveProperty Speed { get; } = new FloatReactiveProperty(0.5f);
    public BoolReactiveProperty CanAct { get; } = new BoolReactiveProperty(true);
    public BoolReactiveProperty IsAlive { get; } = new BoolReactiveProperty(true);
    public Action<Command> CommandAction { get; set; }

    public UInt32 ID { get; private set; }
    public Animator Animator { get; private set; }
    public Collider Collider { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    public ActionManager ActionManager { get; private set; }

    StatusCanvas statusCanvas;

    public Character Initialize<T>(UInt32 id, string name_) where T : Actor, new()
    {
        if (!isInitialized)
        {
            isInitialized   = true;
            name            = name_;
            tag             = "Character";
            Actor           = new T();
            ID              = id;
            Instances[ID]   = this;
            Animator        = GetComponent<Animator>();
            Collider        = GetComponent<Collider>();
            Rigidbody       = gameObject.AddComponent<Rigidbody>();
            ActionManager   = gameObject.AddComponent<ActionManager>();

            ActionManager.Action(ActionType.Normal);

            statusCanvas = Instantiate(ObjectManager.StatusCanvas).GetComponent<StatusCanvas>().Initialize(this);

            HP.Pairwise().Subscribe(hp =>
            {
                if (hp.Previous <= 0 && hp.Current > 0)
                {
                    IsAlive.Value = true;
                }

                if (hp.Previous > 0 && hp.Current <= 0)
                {
                    IsAlive.Value = false;
                }
            });

            IsAlive.Subscribe(isAlive =>
            {
                if (isAlive)
                {
                    Animator.SetTrigger("Normal");
                }
                else
                {
                    Animator.SetTrigger("Death");
                }

                Collider.enabled = CanAct.Value = isAlive;
            });
        }

        return this;
    }

    public void Chat(string message)
    {

    }

    public void Destroy()
    {
        Destroy(statusCanvas.gameObject);
        Destroy(gameObject);
    }

    public void Attack(Character target, float amount)
    {
        if (amount > 0)
        {
            Actor.Action(target.Actor, () =>
            {
                RoomTurn.Request($"Character Attack {target.ID} {amount}");
                target.HP.Value -= amount;
            });
        }
    }

    public static void SubscribeAttacked()
    {
        RoomTurn.AsObservable.Subscribe(turn =>
        {
            var messages = turn.Message.Split(' ');

            if (messages.Length == 4 && messages[0] == "Character" && messages[1] == "Attack")
            {
                UInt32 id;
                float amount;
                if (UInt32.TryParse(messages[2], out id) && float.TryParse(messages[3], out amount))
                {
                    Character character;
                    if (Instances.TryGetValue(id, out character))
                    {
                        character.HP.Value -= amount;
                    }
                }
            }
        });
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using UniRx.Triggers;
using Latte;
using System.Linq;

public class AI : MonoBehaviour
{
    public static Dictionary<UInt32, AI> Instances = new Dictionary<UInt32, AI>();

    public UInt32 id;
    public float TargetRange = 6f;
    public Vector3 sponePosition { get; set; }

    public Character Character { get; private set; }
    public ActionManager ActionManager { get; private set; }

    private void Start()
    {
        Character = gameObject.AddComponent<Character>();
        Character.Initialize<EnemyActor>(id, name);

        Instances[id] = this;

        sponePosition = transform.position;

        Character.Speed.Value = 0.4f;
        
        this.UpdateAsObservable().Subscribe(_ =>
        {
            var targets = new List<Character>(4);

            targets.Add(Host.Character);
            targets.AddRange(Client.Instances.Values.Select(client => client.Character));

            targets = targets
                .OrderBy(target => Vector3.Distance(target.transform.position, transform.position))
                .Where(target => Vector3.Distance(target.transform.position, transform.position) <= TargetRange && target.IsAlive.Value)
                .ToList();

            foreach (var target in targets)
            {
                Character.CommandAction(new Command(target.gameObject));
                return;
            }

            Character.CommandAction(new Command(sponePosition));
        });
        
    }
}

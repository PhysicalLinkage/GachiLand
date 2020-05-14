using System.Collections.Generic;
using UnityEngine;

public interface IInitializable<T>
{
    void Initialize(T element);
}


public abstract class FactoryBase<T, E> : MonoBehaviour
    where T : MonoBehaviour, IInitializable<E>
{
    protected abstract GameObject Prefab { get; }

    List<T> cashables = new List<T>();

    public void Cash(int capacity)
    {
        if (capacity > cashables.Capacity)
        {
            cashables.Capacity = capacity;
        }

        for (int i = cashables.Count; i < capacity; i++)
        {
            T cashable = Instantiate(Prefab).GetComponent<T>();
            cashable.gameObject.SetActive(false);
            cashables.Add(cashable);
        }
    }

    public T Generate(E element)
    {
        foreach (var cashable in cashables)
        {
            if (!cashable.gameObject.activeSelf)
            {
                cashable.Initialize(element);
                cashable.gameObject.SetActive(true);
                return cashable;
            }
        }

        T newCashable = Instantiate(Prefab).GetComponent<T>();
        newCashable.Initialize(element);
        cashables.Add(newCashable);
        return newCashable;
    }
}

public abstract class ManaSphereFactory : MonoBehaviour
{
    protected abstract GameObject ManaSphere { get; }

    List<ManaSphere> m_ManaSpheres = new List<ManaSphere>(64);

    private void Start()
    {
        Cash(12);
    }

    public void Cash(int capacity)
    {
        for (int i = 0; i < capacity; i++)
        {
            var manaObject = Instantiate(ManaSphere).GetComponent<ManaSphere>();
            manaObject.gameObject.SetActive(false);
            m_ManaSpheres.Add(manaObject);
        }
    }

    public ManaSphere Generate(float lifeTime)
    {
        foreach (var manaSphere in m_ManaSpheres)
        {
            if (!manaSphere.gameObject.activeSelf)
            {
                manaSphere.LifeTime = lifeTime;
                manaSphere.gameObject.SetActive(true);
                return manaSphere;
            }
        }

        ManaSphere newManaSphere = Instantiate(ManaSphere).GetComponent<ManaSphere>();
        newManaSphere.LifeTime = lifeTime;
        m_ManaSpheres.Add(newManaSphere);
        return newManaSphere;
    }
}

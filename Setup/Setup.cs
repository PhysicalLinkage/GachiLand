using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using UniRx.Triggers;
using System.Linq;
using UnityEngine.SceneManagement;

public class Setup : MonoBehaviour
{

    [SerializeField] GameObject NameSetter = null;
    [SerializeField] GameObject ModelSetter = null;
    [SerializeField] GameObject Man = null;
    [SerializeField] GameObject Woman = null;
    [SerializeField] GameObject ManSpotLight = null;
    [SerializeField] GameObject WomanSpotLight = null;

    private void Awake()
    {
        Woman.tag = Man.tag = "Character";

        this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonUp(0))
            .Select(_ => Input.mousePosition)
            .Select(point => Camera.main.ScreenPointToRay(point))
            .Select(ray => Physics.RaycastAll(ray, 100f))
            .Select(hits => hits.Where(hit => hit.collider.tag.Split('/').Any(tag => tag == "Character")))
            .Where(hits => hits.Any())
            .Select(hits => hits.First().collider.GetComponent<Animator>())
            .Do(animator =>
            {
                if (animator.gameObject.name == "Man")
                {
                    WomanSpotLight.SetActive(false);
                    ManSpotLight.SetActive(true);
                }
                else if (animator.gameObject.name == "Woman")
                {
                    ManSpotLight.SetActive(false);
                    WomanSpotLight.SetActive(true);
                }
            })
            .Select(animator => animator.avatar.name)
            .Subscribe(name => Model = name);
    }

    private void Start()
    {
        ModelSetter.SetActive(false);
        Man.SetActive(false);
        Woman.SetActive(false);
        ManSpotLight.SetActive(false);
        WomanSpotLight.SetActive(false);
    }

    public void Save()
    {
        if (Model != null && Name != null)
        {
            Host.Setup(Model, Name);
            SceneManager.LoadScene("Login");
        }
    }

    public string Name { get; private set; } = null;

    public void SetName(string name)
    {
        Name = name;
        SetNameEnd();
    }

    public void SetNameEnd()
    {
        NameSetter.SetActive(false);
        ModelSetter.SetActive(true);
        Man.SetActive(true);
        Woman.SetActive(true);
    }


    public string Model { get; private set; } = null;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UniRx;
using UniRx.Triggers;
using System.Linq;

public class StatusCanvas : MonoBehaviour
{
    Character m_character;
    RectTransform m_rect;
    Vector3 m_offset;

    [SerializeField] Text m_name;
    [SerializeField] Image m_hp;

    private void LateUpdate()
    {
        transform.position = m_character.transform.position + m_offset;
        transform.rotation = Camera.main.transform.rotation;
    }

    private Vector3 Offset()
    {
        var rect_h = m_rect.rect.height * m_rect.localScale.y;
        var character_h = m_character.Collider.bounds.extents.y;
        return (character_h + rect_h) * Vector3.up;
    }

    public StatusCanvas Initialize(Character character)
    {
        m_name.text = character.name;
        m_character = character;
        m_rect = GetComponent<RectTransform>();
        m_offset = Offset();

        m_character.HP.Subscribe(value => m_hp.fillAmount = value / 100f);
        m_character.IsAlive.Subscribe(isAlive => gameObject.SetActive(isAlive));

        return this;
    }
}

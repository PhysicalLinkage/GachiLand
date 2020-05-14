using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UniRx.Triggers;
using System.Linq;

public class FollowPerson : MonoBehaviour
{
    [SerializeField] float m_HeightRatio = 1f;
    [SerializeField] float m_RadiusRatio = 1f;
    [SerializeField] float m_Length = 6f;
    [SerializeField] float m_Angle;
    Vector3 m_Offset = new Vector3();

    public GameObject Person => Host.Character?.gameObject;

    private Collider charaCollider;

    private Subject<Vector3> drag { get; } = new Subject<Vector3>();

    private Vector3 DownPoint = new Vector3();

    float length { get { return m_Length; } set { m_Length = Mathf.Min(Mathf.Max(2f, value), 8f); } }

    public void UpdateOffset()
    {
        m_Offset.Set(Mathf.Sin(m_Angle) * m_Length * m_RadiusRatio, m_Length * m_HeightRatio, Mathf.Cos(m_Angle) * m_Length * m_RadiusRatio);
        transform.position = Person.transform.position + m_Offset;
        var chara = Host.Character;
        var height = chara.Collider.bounds.size.y * chara.transform.localScale.y / 2f;
        var point = new Vector3(Person.transform.position.x, height, Person.transform.position.z);
        transform.LookAt(point);
    }

    private void Start()
    {
        this.LateUpdateAsObservable()
            .Subscribe(_ => transform.position = Person.transform.position + m_Offset);
        /*
        this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonDown(0))
            .Select(_ => Input.mousePosition)
            .Select(point => new Vector3(point.x -= Screen.width / 2f, point.y -= Screen.height / 2f))
            .Subscribe(point => DownPoint = point);

        this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButton(0))
            .Select(_ => Input.mousePosition)
            .Select(point => new Vector3(point.x -= Screen.width / 2f, point.y -= Screen.height / 2f))
            .Where(point => Vector3.Distance(point, DownPoint) > 24f)
            .Subscribe(point =>
            {
                var lsl = DownPoint.normalized;
                var lel = point.normalized;
                var sin = Vector3.Cross(lsl, lel).z;
                var cos = Vector3.Dot(lel, lsl);
                m_Angle += Mathf.Atan(sin / cos);
                UpdateOffset();

                DownPoint = point;
            });
            */

        this.UpdateAsObservable()
            .Select(_ => Input.GetAxis("Mouse ScrollWheel"))
            .Where(scroll => scroll > 0)
            .Do(_ => length--)
            .Subscribe(_ => UpdateOffset());

        this.UpdateAsObservable()
            .Select(_ => Input.GetAxis("Mouse ScrollWheel"))
            .Where(scroll => scroll < 0)
            .Do(_ => length++)
            .Subscribe(_ => UpdateOffset());

        UpdateOffset();
    }
}

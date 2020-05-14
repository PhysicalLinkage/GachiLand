using UnityEngine;
using UniRx;
using UnityEngine.EventSystems;

public class PassTap : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        Host.TapDownPoint = Input.mousePosition;
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        Host.TapUpAsObserver.OnNext(Input.mousePosition);
    }
}

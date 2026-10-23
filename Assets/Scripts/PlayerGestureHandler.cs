
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class PlayerGestureHandler : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerExitHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [Header("Gesture Settings")]
    [SerializeField, Min(0.1f)]
    private float longPressDuration = 0.5f;

    [SerializeField, Min(0.1f)]
    private float doubleTapInterval = 0.3f;

    [SerializeField, Min(1f)]
    private float dragThreshold = 15f;

    private Action onLongPress;
    private Action onDoubleTap;
    private bool allowDoubleTap;

    private bool pointerDown;
    private bool holdTriggered;
    private bool dragging;
    private bool pointerExited;
    private int activePointerId;
    private float pointerDownTime;
    private Vector2 pointerDownPosition;

    private bool waitingForSecondTap;
    private float firstTapTime;

    public void Configure(
        Action longPressAction,
        Action doubleTapAction = null,
        bool enableDoubleTap = false)
    {
        onLongPress = longPressAction;
        onDoubleTap = doubleTapAction;
        allowDoubleTap = enableDoubleTap;
    }

    private void Update()
    {
        if (!pointerDown || holdTriggered || dragging || pointerExited)
            return;

        if (Time.unscaledTime - pointerDownTime >= longPressDuration)
        {
            holdTriggered = true;
            waitingForSecondTap = false;
            onLongPress?.Invoke();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (pointerDown)
            return;

        pointerDown = true;
        holdTriggered = false;
        dragging = false;
        pointerExited = false;

        activePointerId = eventData.pointerId;
        pointerDownTime = Time.unscaledTime;
        pointerDownPosition = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!pointerDown || eventData.pointerId != activePointerId)
            return;

        bool validTap =
            !holdTriggered &&
            !dragging &&
            !pointerExited &&
            Vector2.Distance(pointerDownPosition, eventData.position)
                <= dragThreshold;

        pointerDown = false;

        if (!validTap)
            return;

        if (!allowDoubleTap || onDoubleTap == null)
            return;

        float now = Time.unscaledTime;

        if (waitingForSecondTap &&
            now - firstTapTime <= doubleTapInterval)
        {
            waitingForSecondTap = false;
            onDoubleTap.Invoke();
        }
        else
        {
            waitingForSecondTap = true;
            firstTapTime = now;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!pointerDown || eventData.pointerId != activePointerId)
            return;

        pointerExited = true;
        pointerDown = false;
        waitingForSecondTap = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != activePointerId)
            return;

        dragging = true;
        pointerDown = false;
        waitingForSecondTap = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != activePointerId)
            return;

        if (Vector2.Distance(pointerDownPosition, eventData.position)
            > dragThreshold)
        {
            dragging = true;
            pointerDown = false;
            waitingForSecondTap = false;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != activePointerId)
            return;

        pointerDown = false;
        dragging = false;
    }

    private void OnDisable()
    {
        pointerDown = false;
        holdTriggered = false;
        dragging = false;
        pointerExited = false;
        waitingForSecondTap = false;
    }
}

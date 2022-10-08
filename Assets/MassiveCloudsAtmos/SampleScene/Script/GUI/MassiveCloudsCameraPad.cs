using UnityEngine;
using UnityEngine.EventSystems;

public class MassiveCloudsCameraPad : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform target;
    [Range(0.01f, 1f)]
    public float sensitivity = 1f;

    [SerializeField] private float pitch = -16f;
    [SerializeField] private float yaw   = 90f;

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void Start()
    {
        var angles = target.rotation.eulerAngles;
        pitch = angles.x > 180f ? angles.x - 360f : angles.x;
        yaw = angles.y;
        target.rotation = Quaternion.Euler(pitch, yaw, 0);
    }

    public void OnDrag(PointerEventData eventData)
    {
        var diff = eventData.delta;

        pitch = Mathf.Clamp (pitch - diff.y * sensitivity, -85f, 85f);
        yaw   = Mathf.Repeat(yaw + diff.x * sensitivity, 360f);

        target.rotation = Quaternion.Euler(pitch, yaw, 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }
}
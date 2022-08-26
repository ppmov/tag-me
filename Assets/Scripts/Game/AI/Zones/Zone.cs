using UnityEngine;
using UnityEngine.Events;

public class Zone : MonoBehaviour
{
    private enum Standing { None, Top, Down }

    [SerializeField]
    private Standing standing;
    [SerializeField]
    private Zone[] mostRemoteZones;

    public bool IsTop => standing == Standing.Top;
    public bool IsDown => standing == Standing.Down;
    public Vector2 Position => transform.position;
    public Vector2 RandomPositionInCircle => (Vector2)transform.position + new Vector2(Random.Range(0f, circle.radius * transform.lossyScale.x), Random.Range(0f, circle.radius * transform.lossyScale.y));
    public Zone[] MostRemoteZones => mostRemoteZones;

    public UnityEvent<Unit, Zone> OnUnitEnter;
    public UnityEvent<Unit, Zone> OnUnitExit;

    private CircleCollider2D circle;

    public bool IsZoneFarthest(Zone other)
    {
        foreach (Zone zone in MostRemoteZones)
            if (zone == other)
                return true;

        return false;
    }

    private void Awake()
    {
        circle = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Unit"))
            OnUnitEnter.Invoke(collision.gameObject.GetComponent<Unit>(), this);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Unit"))
            OnUnitExit.Invoke(collision.gameObject.GetComponent<Unit>(), this);
    }
}

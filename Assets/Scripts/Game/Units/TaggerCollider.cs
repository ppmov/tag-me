using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TaggerCollider : MonoBehaviour
{
    public UnityEvent OnStateChange;

    public Unit.Role State
    {
        get => (Unit.Role)gameObject.layer;
        set
        {
            gameObject.layer = (int)value;
            OnStateChange.Invoke();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (State != Unit.Role.Tagger)
            return;

        if (collision.gameObject.layer != (int)Unit.Role.Runner)
            return;

        TaggerCollider other = collision.gameObject.GetComponent<TaggerCollider>();

        if (other == null)
            return;

        other.State = Unit.Role.Tagger;
        StartCoroutine(AfterTagFlickering());
    }

    private IEnumerator AfterTagFlickering()
    {
        State = Unit.Role.Untagable;
        yield return new WaitForSeconds(Settings.FlickeringTime);
        
        if (State == Unit.Role.Untagable)
            State = Unit.Role.Runner;
    }
}

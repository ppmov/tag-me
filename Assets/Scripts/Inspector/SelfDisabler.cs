using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDisabler : MonoBehaviour
{
    [SerializeField]
    private int lifetime;

    private void OnEnable()
    {
        StartCoroutine(Wait4Disable());
    }

    private IEnumerator Wait4Disable()
    {
        yield return new WaitForSeconds(lifetime);
        gameObject.SetActive(false);
    }
}

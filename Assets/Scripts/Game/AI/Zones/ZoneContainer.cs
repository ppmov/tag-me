using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneContainer : MonoBehaviour
{
    public static RoundedZoneManager Rounded { get; private set; }
    public static TopDownZoneManager TopDown { get; private set; }

    [SerializeField]
    private GameObject roundedZoneContainer;
    [SerializeField]
    private GameObject topDownZoneContainer;

    private void Awake()
    {
        Rounded = new RoundedZoneManager(roundedZoneContainer.GetComponentsInChildren<Zone>());
        TopDown = new TopDownZoneManager(topDownZoneContainer.GetComponentsInChildren<Zone>());
    }

}

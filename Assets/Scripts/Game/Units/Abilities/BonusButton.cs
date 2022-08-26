using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusButton : MonoBehaviour
{
    [SerializeField]
    private Unit.Bonus bonus;
    private Button button;

    public Unit.Bonus Bonus { get => bonus; }
    public bool Interactable { get => button.interactable; set => button.interactable = value; }

    public void AddListener(UnityEngine.Events.UnityAction call) => button.onClick.AddListener(call);

    private void Awake() => button = GetComponent<Button>();
}

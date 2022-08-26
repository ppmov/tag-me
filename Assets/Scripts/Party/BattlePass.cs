using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Battle Pass")]
public class BattlePass : ScriptableObject
{
    public string Title => title.ToUpper();
    public Sprite Background => background;
    public int Cost => cost;
    public int Prize => prize;
    public int Bonus => bonus;

    public float BoostMultiplier => boostMultiplier;
    public float DashMultiplier => dashMultiplier;
    public float GrowthMultiplier => growthMultiplier;

    [SerializeField]
    private string title;
    [SerializeField]
    private Sprite background;
    [SerializeField]
    private int cost;
    [SerializeField]
    private int prize;
    [SerializeField]
    private int bonus;

    [SerializeField]
    private float boostMultiplier;
    [SerializeField]
    private float dashMultiplier;
    [SerializeField]
    private float growthMultiplier;
}

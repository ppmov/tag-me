using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Options")]
public class Settings : ScriptableObject
{
    public static Sprite PlayerSkin => Instance.playerSkin;
    public static bool IsBonusesEnabled => Instance.isBonusesEnabled;
    public static bool HasAIOnly => Instance.hasAIOnly;
    public static float UnitSize => Instance.unitSize;
    public static float MoveSpeed => Instance.moveSpeed;
    public static float FlickeringTime => Instance.flickeringTime;
    public static GameObject EliminateFx => Instance.eliminatedFx;

    public static float AbilityDuration => Instance.abilityDuration;
    public static float AbilityCooldown => Instance.abilityCooldown;
    public static float BoostSpeed => Instance.boostSpeed;
    public static GameObject BoostFx => Instance.boostFx;
    public static float GrowthScale => Instance.growthScale;
    public static GameObject GrowthFx => Instance.growthFx;
    public static float DashForce => Instance.dashForce;
    public static float DashFinishForce => Instance.dashFinishForce;
    public static GameObject DashFx => Instance.dashFx;
    public static Material InvisibleMaterial => Instance.invisibleMaterial;
    public static GameObject InvisibleFx => Instance.invisibleFx;

    public static AISettings[] AIs => Instance.ais;

    [SerializeField]
    private Sprite playerSkin;

    [Header("Common")]
    [SerializeField]
    private bool isBonusesEnabled = true;
    [SerializeField]
    private bool hasAIOnly = false;
    [SerializeField]
    private float unitSize = 1f;
    [SerializeField]
    private float moveSpeed = 50f;
    [SerializeField]
    private float flickeringTime = 3f;
    [SerializeField]
    private GameObject eliminatedFx;

    [Header("Abilities")]
    [SerializeField]
    private float abilityDuration = 3f;
    [SerializeField]
    private float abilityCooldown = 5f;
    [SerializeField]
    private float boostSpeed = 30f;
    [SerializeField]
    private GameObject boostFx;
    [SerializeField]
    private float growthScale = 1.5f;
    [SerializeField]
    private GameObject growthFx;
    [SerializeField]
    private float dashForce = 200f;
    [SerializeField]
    private float dashFinishForce = 50f;
    [SerializeField]
    private GameObject dashFx;
    [SerializeField]
    private Material invisibleMaterial;
    [SerializeField]
    private GameObject invisibleFx;

    [Space]
    [SerializeField]
    private AISettings[] ais;

    private static Settings Instance
    {
        get
        {
            if (_instance == null)
                _instance = Resources.Load<Settings>("Setup/Options");
            return _instance;
        }
    }

    private static Settings _instance;
}

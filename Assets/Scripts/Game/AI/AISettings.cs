using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/AI")]
public class AISettings : ScriptableObject
{
    public enum Distance { Critical, Dangerous, Attention, Safe }
    public Sprite Skin => skin;
    public float PredictionRange => distantPredictionRange.Random;
    public float FloorPredictionRange => closePredictionRange.Random;
    public float CeilPredictionRange => distantPredictionRange.maxValue;
    public int RaycastAccuracy => raycastAccuracy;
    public int FixedUpdateCount => (int)fixedUpdateCount.Random;
    public float OvercrowdDistance => overcrowdDistance;
    public int OvercrowdCount => overcrowdCount;
    public float StopChangingTargetDistance => stopChangingTargetDistance.Random;
    public float TaggerInterceptionAngle => interceptionAngle.Random;
    public float BoostDistance(Unit.Role role) => role == Unit.Role.Tagger ? boostDistance4tag : boostDistance4run;
    public float DashDistance(Unit.Role role) => role == Unit.Role.Tagger ? dashDistance4tag : dashDistance4run;
    public float GrowthDistance => growthDistance4tag;
    public bool HasToStopSuddenly => Random.Range(0f, 1f) < unexpectedStopChance;
    public float SuddenStopDuration => unexpectedStopDuration.Random;
    public bool HasToMessUpAngle => Random.Range(0f, 1f) < messingMoveDirectionChance;
    public float MessingAngle => messingMoveAngle.Random;
    public bool HasToUseRandomAbility => Random.Range(0f, 1f) < chanceOfUsingRandomBonus;
    public float SmoothSpeed => 2f - smoothingScale;
    public float JerkAvoidanceFactor => (1f - jerkAvoidanceFactor) * 2f;
    public int LastChanceInvisibilityTime => lastChanceInvisibilityTime;
    public float BecomeTaggerDelay => becomeTaggerDelay.Random;
    public float InterceptionDelay => interceptionDelay.Random;
    public RangedFloat SharpMoveAngles => sharpRunAngles;
    public RangedFloat BypassAngles => bypassAngles;
    public float SafeSleepChance => safeSleepChance.Random;
    public float SharpDecisionDelay => decisionCooldown.Random;
    public float ReversalCooldown => reversalCooldown.Random;
    public float SwitchStageTime => switchStageTime;
    public float BypassDistance => bypassDistance;
    public RangedFloat ZoneVisitedDelayRange => bypassDelay;
    public bool CanBypassAtDangerousDistance => Random.Range(0f, 1f) < bypassChance;
    public float StraightRunDistance => straightDangerousDistance;
    public float SharpRunDistance => sharpDangerousDistance;
    public float ComparingDistancesScale => 10f;//comparingDistancesScale;
    public float ReversalWindowWidth => reversalWindowWidth + FloorPredictionRange;
    public float ReversalMaxAngleToTarget => reversalMaxAngleToTarget;
    public float ReversalStartError => reversalStartError;
    public float ReversalAngleError => reversalAngleError;
    public float CornerTaggerDistance => cornerDistanceToTagger;
    public float CornerWallsDistance => cornerDistanceToWalls;
    public float UnlockPredictionDistance => switchPredictionDistance;

    public Distance GetDistanceType(float distance)
    {
        if (distance < criticalDistance)
            return Distance.Critical;

        if (distance < Mathf.Max(straightDangerousDistance, sharpDangerousDistance))
            return Distance.Dangerous;

        if (distance < attentionDistance)
            return Distance.Attention;

        return Distance.Safe;
    }

    public bool CanMoveSharply(float distance)
    {
        if (distance < criticalDistance)
            return false;

        if (distance < straightDangerousDistance)
            return false;

        if (distance < sharpDangerousDistance)
            return true;

        return false;
    }


    [SerializeField]
    private Sprite skin;

    [Header("Common")]
    [SerializeField]
    [MinMaxRange(1, 25)]
    private RangedFloat fixedUpdateCount = new RangedFloat(6, 10);
    [SerializeField]
    [Range(0f, 1.99f)]
    private float smoothingScale = 1f;
    [SerializeField]
    [MinMaxRange(5, 100)]
    private RangedFloat closePredictionRange = new RangedFloat(5, 15);
    [SerializeField]
    [MinMaxRange(5, 100)]
    private RangedFloat distantPredictionRange = new RangedFloat(25, 50);
    [SerializeField]
    [Range(0, 125)]
    private float switchPredictionDistance = 20f;

    [Space]
    [SerializeField]
    [Range(0, 50)]
    private float overcrowdDistance = 25f;
    [SerializeField]
    [Range(2, 5)]
    private int overcrowdCount = 2;

    [Header("Tagger")]
    [SerializeField]
    [MinMaxRange(0, 125)]
    private RangedFloat stopChangingTargetDistance = new RangedFloat(30, 70);
    [SerializeField]
    [MinMaxRange(0, 90)]
    private RangedFloat interceptionAngle = new RangedFloat(25, 65);
    [SerializeField]
    [MinMaxRange(0, 1)]
    private RangedFloat interceptionDelay = new RangedFloat(0.6f, 0.8f);
    [SerializeField]
    [MinMaxRange(0, 1)]
    private RangedFloat becomeTaggerDelay = new RangedFloat(0f, 0.2f);

    [Header("Runner")]
    [SerializeField]
    [Range(4, 180)]
    private int raycastAccuracy = 100;
    [SerializeField]
    [Range(0f, 1f)]
    private float jerkAvoidanceFactor = 0.2f;
    [SerializeField]
    [Range(0, 30)]
    private float switchStageTime = 10;
    [SerializeField]
    [MinMaxRange(0, 1)]
    private RangedFloat safeSleepChance = new RangedFloat(0f, 0.4f);

    [Space]
    [SerializeField]
    [Range(0, 125)]
    private float criticalDistance = 12f;
    [SerializeField]
    [Range(0, 125)]
    private float straightDangerousDistance = 18f;
    [SerializeField]
    [Range(0, 125)]
    private float sharpDangerousDistance = 65f;
    [SerializeField]
    [Range(0, 125)]
    private float attentionDistance = 90f;

    [Space]
    [SerializeField]
    [MinMaxRange(0.2f, 1f)]
    private RangedFloat decisionCooldown = new RangedFloat(0.4f, 0.6f);
    [SerializeField]
    [MinMaxRange(0, 120)]
    private RangedFloat sharpRunAngles = new RangedFloat(35, 120);

    [Space]
    [SerializeField]
    [Range(0, 60)]
    private float cornerDistanceToTagger = 30f;
    [SerializeField]
    [Range(0, 60)]
    private float cornerDistanceToWalls = 42f;

    [Space]
    [SerializeField]
    [Range(0f, 1f)]
    private float bypassChance = 0.5f;
    [SerializeField]
    [Range(0, 125)]
    private float bypassDistance = 15f;
    [SerializeField]
    [MinMaxRange(0, 180)]
    private RangedFloat bypassAngles = new RangedFloat(20, 100);
    [SerializeField]
    [MinMaxRange(0f, 3f)]
    private RangedFloat bypassDelay = new RangedFloat(0f, 0.4f);

    [Space]
    [SerializeField]
    [MinMaxRange(0f, 5f)]
    private RangedFloat reversalCooldown = new RangedFloat(0.5f, 1f);
    [SerializeField]
    [Range(1, 20)]
    private float reversalWindowWidth = 8;
    [SerializeField]
    [Range(90, 180)]
    private float reversalMaxAngleToTarget = 150;
    [SerializeField]
    [Range(0, 1)]
    private float reversalStartError = 0.1f;
    [SerializeField]
    [Range(1, 45)]
    private float reversalAngleError = 25;

    [Header("Errors")]
    [SerializeField]
    [Range(0f, 0.1f)]
    private float unexpectedStopChance = 0f;
    [SerializeField]
    [MinMaxRange(0f, 10f)]
    private RangedFloat unexpectedStopDuration = new RangedFloat();
    [SerializeField]
    [Range(0f, 0.5f)]
    private float messingMoveDirectionChance = 0.2f;
    [SerializeField]
    [MinMaxRange(0f, 180f)]
    private RangedFloat messingMoveAngle = new RangedFloat(5f, 15f);
    [SerializeField]
    [Range(0f, 1f)]
    private float chanceOfUsingRandomBonus = 0f;

    [Header("Abilities")]
    [SerializeField]
    [Range(0, 125)]
    private float boostDistance4tag = 35f;
    [SerializeField]
    [Range(0, 125)]
    private float growthDistance4tag = 15f;
    [SerializeField]
    [Range(0, 125)]
    private float dashDistance4tag = 35f;
    [SerializeField]
    [Range(0, 125)]
    private float boostDistance4run = 20f;
    [SerializeField]
    [Range(0, 125)]
    private float dashDistance4run = 20f;
    [SerializeField]
    [Range(1, 10)]
    private int lastChanceInvisibilityTime = 5;
}

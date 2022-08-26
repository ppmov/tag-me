using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public enum Role { Tagger = 6, Runner = 7, Untagable = 8 }
    public enum Bonus { Boost, Dash, Growth, Invisible }

    [SerializeField]
    private SpriteRenderer skin;
    [SerializeField]
    private GameObject playerMarker;
    [SerializeField]
    private GameObject taggerMarker;

    private TaggerCollider tagger;
    private CircleCollider2D circleCollider;

    public Vector2 Position { get => transform.position; }
    public float Radius { get => circleCollider.radius * transform.localScale.x; }
    public Rigidbody2D Rigidbody { get; private set; }
    public Dictionary<Bonus, Ability> Abilities { get; private set; }
    public Vector2 LastDirection { get; private set; }
    public float SpeedVariable { get; set; }
    public Role State { get => tagger.State; set => tagger.State = value; }

    public BattlePass BattlePass { get => Game.Party[this].BattlePass; }

    public void Construct()
    {
        SetReferences();

        playerMarker.SetActive(Game.Party.Myself?.Unit == this);
        skin.sprite = Game.Party[this].Skin;
    }

    public void MoveTo(Vector2 direction)
    {
        if (Game.Counter.IsFreezed)
            return;

        if (Rigidbody.velocity != Vector2.zero)
        {
            if (Rigidbody.velocity.magnitude >= Settings.DashFinishForce)
                return;
            else
                Rigidbody.velocity = Vector2.zero;
        }

        if (direction.magnitude > 1f)
            direction = direction.normalized;

        //
        skin.color = new Color(direction.magnitude < 0.95f ? 0 : 1, direction.magnitude < 0.95f ? 0 : 1, direction.magnitude < 0.95f ? 0 : 1, skin.color.a);
        //

        transform.position += (Settings.MoveSpeed + SpeedVariable) * Time.fixedDeltaTime * (Vector3)direction;
        LastDirection = direction;
    }

    private void SetReferences()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        tagger = GetComponentInChildren<TaggerCollider>();
        tagger.OnStateChange.AddListener(UpdateLook);
        UpdateLook();

        Rigidbody = GetComponent<Rigidbody2D>();
        Rigidbody.freezeRotation = true;

        Abilities = new Dictionary<Bonus, Ability>
        {
            { Bonus.Boost, new BoostAbility(this) },
            { Bonus.Dash, new DashAbility(this) },
            { Bonus.Growth, new GrowthAbility(this) },
            { Bonus.Invisible, new InvisibleAbility(this) }
        };

        transform.localScale = new Vector3(Settings.UnitSize, Settings.UnitSize, 1f);
    }

    private void UpdateLook()
    {
        taggerMarker.SetActive(State == Role.Tagger);
        skin.color = new Color(skin.color.r, skin.color.g, skin.color.b, State == Role.Untagable ? 0.5f : 1f);
    }

    private bool hasQuit = false;

    private void OnApplicationQuit() => hasQuit = true;

    private void OnDestroy()
    {
        if (!hasQuit)
            if (Settings.EliminateFx != null)
                Instantiate(Settings.EliminateFx, transform.position, Quaternion.identity);
    }
}

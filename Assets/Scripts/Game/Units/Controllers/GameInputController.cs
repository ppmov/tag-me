using System.Collections;
using UnityEngine;


public class GameInputController : MonoBehaviour
{
    [SerializeField]
    private Joystick joystick;
    [SerializeField]
    private BonusButton[] buttons;

    private Unit PlayerUnit { get => Game.Party?.Myself?.Unit; }

    public void UseBonus(Unit.Bonus bonus)
    {
        if (!PlayerUnit.Abilities[bonus].TryUse())
            return;

        foreach (BonusButton button in buttons)
            if (button.Bonus == bonus)
            {
                button.Interactable = false;
                StartCoroutine(WaitUntilCooldownEnds(button));
                return;
            }
    }

    private IEnumerator WaitUntilCooldownEnds(BonusButton button)
    {
        yield return new WaitUntil(() => !PlayerUnit.Abilities[button.Bonus].IsCooling);
        button.Interactable = true;
    }

    private void Start()
    {
        foreach (BonusButton button in buttons)
            if (Settings.IsBonusesEnabled)
                button.AddListener(delegate { UseBonus(button.Bonus); });
            else
                button.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    { 
        if (PlayerUnit == null)
            return;

        if (joystick == null)
            return;

        if (Input.GetKeyDown(KeyCode.Q))
            UseBonus(Unit.Bonus.Boost);

        if (Input.GetKeyDown(KeyCode.W))
            UseBonus(Unit.Bonus.Dash);

        if (Input.GetKeyDown(KeyCode.E))
            UseBonus(Unit.Bonus.Growth);

        if (Input.GetKeyDown(KeyCode.R))
            UseBonus(Unit.Bonus.Invisible);

        if (joystick.DragDirection != Vector2.zero)
            PlayerUnit.MoveTo(joystick.DragDirection);
    }
}

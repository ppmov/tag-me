using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ExecuteAlways]
public class BattlePassElement : MonoBehaviour
{
    [SerializeField]
    private BattlePass setup;

    [Space]
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private Text costText;
    [SerializeField]
    private Text prizeText;
    [SerializeField]
    private Text bonusText;

    private Image backImage;
    private Button button;

    public string Title { get => setup.Title; }
    public bool Interactable { get => button.interactable; set => button.interactable = value; }
    public UnityEvent<BattlePass> OnSelect;

    public void Select() => OnSelect.Invoke(setup);

    private void Awake()
    {
        backImage = GetComponent<Image>();
        button = GetComponent<Button>();
        button.onClick.AddListener(Select);
    }

    private void Start()
    {
        if (setup == null)
            return;

        nameText.text = setup.Title;
        backImage.sprite = setup.Background;
        costText.text = setup.Cost == 0 ? "FREE" : setup.Cost.ToString();
        prizeText.text = setup.Prize.ToString();
        bonusText.text = setup.Bonus.ToString() + "/sec";
    }
}

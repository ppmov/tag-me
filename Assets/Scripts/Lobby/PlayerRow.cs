using UnityEngine;
using UnityEngine.UI;

public class PlayerRow : MonoBehaviour
{
    [SerializeField]
    private Text nickname;
    [SerializeField]
    private Image skin;

    public Player Player { get; set; }

    private void FixedUpdate()
    {
        if (Player == null)
            return;

        skin.sprite = Player.Skin;
        nickname.text = Player.Name;
        nickname.color = Player.IsReady ? Color.green : Color.white;
    }
}

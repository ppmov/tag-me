using UnityEngine;

public class EndGamePanel : MonoBehaviour
{
    [SerializeField]
    private GameObject youWinBanner;
    [SerializeField]
    private GameObject winnerBanner;

    [SerializeField]
    private PlayerRow playerRow;

    public void DisplayWinner(Player winner)
    {
        gameObject.SetActive(true);

        if (winner == Game.Party.Myself)
            youWinBanner.SetActive(true);
        else
        {
            winnerBanner.SetActive(true);
            playerRow.Player = winner;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerList : MonoBehaviour
{
    [SerializeField]
    private GameObject rowPrefab;
    [SerializeField]
    private float margin;

    private List<PlayerRow> list = new List<PlayerRow>();

    private void FixedUpdate()
    {
        for (int i = 0; i < 5; i++)
        {
            Player player = Game.Party[i];

            if (player == null)
            {
                if (list.Count <= i)
                    break;
                else
                    RemoveRow(i);
            }
            else
            {
                if (list.Count <= i)
                    AddRow(player);
                else
                if (player != list[i].Player)
                    list[i].Player = player;
                else
                    continue;
            }
        }
    }

    private void AddRow(Player player)
    {
        PlayerRow row = Instantiate(rowPrefab, transform).GetComponent<PlayerRow>();
        list.Add(row);
        row.Player = player;
        Align();
    }

    private void RemoveRow(int index)
    {
        PlayerRow row = list[index];
        list.RemoveAt(index);
        Destroy(row);
        Align();
    }

    private void Align()
    {
        for (int i = 0; i < list.Count; i++)
            list[i].transform.localPosition = new Vector3(0f, i * -margin, 0f);
    }
}

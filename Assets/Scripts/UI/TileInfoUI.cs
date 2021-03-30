using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileInfoUI : MonoBehaviour
{
    [SerializeField]
    private Text tileName;
    [SerializeField]
    private Text status1Number;
    [SerializeField]
    private Text status2Number;

    public void OpenTileInfoUI(Tile tile)
    {
        tileName.text = tile.type.ToString();
        status1Number.text = tile.avoidBonus.ToString();
        status2Number.text = tile.defResBonus.ToString();
    }
}

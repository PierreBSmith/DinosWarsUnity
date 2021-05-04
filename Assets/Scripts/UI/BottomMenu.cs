using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomMenu : MonoBehaviour
{
    private RulesEngine _rulesEngine;

    void Start()
    {
        _rulesEngine = GameObject.Find("RulesEngine").GetComponent<RulesEngine>();
    }
    public void EndTurn()
    {
        _rulesEngine.EndPlayerTurn();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WorldMap : MonoBehaviour
{
    private List<GameObject> mapButtons = new List<GameObject>();
    private GameManager gameManager;

    void Start()
    {
        foreach(Transform child in transform)
        {
            mapButtons.Add(child.gameObject);
        }
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        DisplaySelectableLevels(gameManager.currentLevel);
    }

    private void DisplaySelectableLevels(int currentLevel)
    {
        foreach(GameObject mapIcon in mapButtons)
        {
            if ((int)mapIcon.GetComponent<LevelButton>().level <= (currentLevel - 1))
            {
                mapIcon.SetActive(true);
            }
            else
            {
                mapIcon.SetActive(false);
            }
        }
    }

    public void LoadLevel(Button button)
    {
        gameManager.inLevel = true;
        SceneManager.LoadScene(button.gameObject.GetComponent<LevelButton>().level.ToString(), LoadSceneMode.Single);
    }
}

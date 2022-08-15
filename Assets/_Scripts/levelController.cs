using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelController : MonoBehaviour
{
    [SerializeField] public unitMatcher unitMatcher;
    [SerializeField] public enemyFieldController enemyFieldController;
    [SerializeField] public playerController playerController;
    [SerializeField] public manaSpawner manaSpawner;


    public GameObject tapToStart;
    public GameObject playerWinScreen;
    public GameObject playerLoseScreen;

    void Update()
    {
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            changeStatusOfScripts(true);
            tapToStart.SetActive(false);
            this.enabled = false;
        }
    }

    private void changeStatusOfScripts(bool value)
    {
        unitMatcher.enabled = value;
        enemyFieldController.enabled = value;
        playerController.enabled = value;
        manaSpawner.enabled = value;
        this.enabled = false;
    }

    public void endGame(bool playerWin)
    {
        changeStatusOfScripts(false);
        if (playerWin)
            playerWinScreen.SetActive(true);
        else
            playerLoseScreen.SetActive(true);
            
        
    }
}

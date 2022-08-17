using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class levelController : MonoBehaviour
{
    [SerializeField] public unitMatcher unitMatcher;
    [SerializeField] public enemyController enemyController;
    [SerializeField] public playerController playerController;
    [SerializeField] public manaSpawner manaSpawner;
    [SerializeField] public Animator cinemachineAnimator;

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
        enemyController.enabled = value;
        playerController.enabled = value;
        manaSpawner.enabled = value;
        this.enabled = false;
    }

    public async void endGame(bool playerWin)
    {
        changeStatusOfScripts(false);
        cinemachineAnimator.Play("ending");
        await Task.Delay(System.TimeSpan.FromSeconds(1f));

        foreach (GameObject unit in GameObject.FindGameObjectsWithTag("enemyUnit"))
            Destroy(unit);
        foreach (GameObject unit in GameObject.FindGameObjectsWithTag("playerUnit"))
            Destroy(unit);


        if (playerWin)
            playerWinScreen.SetActive(true);
        else
            playerLoseScreen.SetActive(true);
            
        
    }
}

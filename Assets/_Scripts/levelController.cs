using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

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
    public GameObject changeSceneButton;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("level"))
            PlayerPrefs.SetInt("level", 1);
    }

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
        if(enemyController)
            enemyController.enabled = value;
        playerController.enabled = value;
        manaSpawner.enabled = value;
        this.enabled = false;
    }

    public async void endGame(bool playerWin)
    {
        changeStatusOfScripts(false);
        

        foreach (unitController unitController in FindObjectsOfType<unitController>())
        {
            unitController.cancelAttack = true;
        }

        if (playerWin)
        {
            /*foreach (GameObject unit in GameObject.FindGameObjectsWithTag("playerUnit"))
                unit.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);*/
        }
        await Task.Delay(System.TimeSpan.FromSeconds(1.5f));
        destroyAllUnits();

        if (playerWin)
        {
            playerWinScreen.SetActive(true);
            destroyCastle();
            PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level") + 1);
        }
        else
        {
            PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level") - 1);
            playerLoseScreen.SetActive(true);
            
        }

        
        changeSceneButton.SetActive(true);
            

    }

    private void destroyAllUnits()
    {
        foreach (GameObject unit in GameObject.FindGameObjectsWithTag("enemyUnit"))
            Destroy(unit);

        foreach (GameObject unit in GameObject.FindGameObjectsWithTag("playerUnit"))
            Destroy(unit);
    }

    private void destroyCastle()
    {
        Transform enemyCastleTransform = FindObjectOfType<unitMatcher>().enemyCastle.transform.GetChild(1);

        foreach(Transform pieceTransform in enemyCastleTransform)
        {
            Rigidbody rb = pieceTransform.gameObject.AddComponent<Rigidbody>();
            pieceTransform.gameObject.AddComponent<BoxCollider>();
            rb.AddExplosionForce(3, enemyCastleTransform.position, 10);
            
        }

    }

    public void changeScene()
    {
        int level = SceneManager.GetActiveScene().buildIndex;
        level++;
        level %= SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(level);
    }
    private bool isCameraAtFinalPosition = false;
    public void moveCameraAtFinalPosition()
    {
        if (isCameraAtFinalPosition)
            return;

        isCameraAtFinalPosition = true;
        

        unitMatcher.enemyUnitsList.Clear();

        foreach (GameObject unit in GameObject.FindGameObjectsWithTag("enemyUnit"))
            Destroy(unit);
        foreach (Transform baby in FindObjectOfType<enemyController>().enemyBabyUnits)
        {
            DOTween.Kill(baby);
            Destroy(baby.gameObject);
        }

        foreach (GameObject unit in GameObject.FindGameObjectsWithTag("playerUnit"))
            unit.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        Destroy(GameObject.FindGameObjectWithTag("Enemy"));
        cinemachineAnimator.Play("ending");
    }
}

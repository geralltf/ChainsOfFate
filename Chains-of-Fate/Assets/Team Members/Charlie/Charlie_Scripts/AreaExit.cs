using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class AreaExit : MonoBehaviour
{

    public string areaToLoad;

    public string areaTransitionName;


    //public AreaEntrance theEntrance;

    // Start is called before the first frame update
    void Start()
    {
        //theEntrance.transitionName = areaTransitionName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ChainsOfFate.Gerallt.GameManager gameManager = ChainsOfFate.Gerallt.GameManager.Instance;
        if (other.tag == "Player")
        {
            
            RunFade(gameManager);
        }
    }

    void RunFade(ChainsOfFate.Gerallt.GameManager gm)
    {
        float alpha = 0;
        gm.transform.GetChild(0).gameObject.SetActive(true);
        Image image = gm.GetComponentInChildren<Image>();

        image.color = new Color(0,0,0,0);
        StartCoroutine(LerpColour(0, alpha, 1, image, gm.loadingAnimationSpeed));
    }

    IEnumerator LerpColour(int i, float currentAlpha, float targetAlpha, Image image, float delay)
    {
        float alpha = Mathf.Lerp(currentAlpha, targetAlpha, i / 10f);
        image.color = new Color(0, 0, 0, alpha);
        yield return new WaitForSeconds(delay / 20f);
        if (i++ < 10)
        {
            StartCoroutine(LerpColour(i, alpha, 1, image, delay));
        }
        else if (i++ < 20)
        {
            StartCoroutine(LerpColour(i, alpha, 0, image, delay));
        }
        if (i == 10)
        {
            LoadLevel();
        }
    }

    void LoadLevel()
    {
        SceneManager.LoadScene(areaToLoad);
        PlayerController.instance.areaTransitionName = areaTransitionName;
    }
}

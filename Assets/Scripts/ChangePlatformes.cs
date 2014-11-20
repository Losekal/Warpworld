using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ChangePlatformes : SingletonGeneric<ChangePlatformes> {

    private int numberPlatformVisible; //Numéro de plate forme visible
    public bool isPlayerJumping; //bool permetant de savoir si le joueur saute ou non
    public int numberOfPlatforms; //nombre de type de plateforme
    public float timeBeforeSwitchPlateforms; //petit temps entre le saut et la disparition


	// Use this for initialization
	void Start () {
        numberPlatformVisible = 2;
        isPlayerJumping = false;

	}
	
	// Update is called once per frame
	void FixedUpdate () {
          
            isPlayerJumping = false;
	}

    IEnumerator displayOnlyPlateformOfNumber(int numPlateformVisible,float timeBeforeSwitch)
    {
        while (timeBeforeSwitch > 0)
        {
            yield return new WaitForSeconds(0.1f);
            timeBeforeSwitch -= 0.1f;
        }
 
         GameObject[] tabOfPlatforms = GameObject.FindGameObjectsWithTag("Plateform");
        foreach (GameObject platform in tabOfPlatforms)
        {
            int numPlatforme = platform.GetComponent<Number_Platform>().numberTypePlatform;
            if (numPlatforme != numberPlatformVisible)
            {
                platform.collider2D.enabled = false;
                platform.GetComponent<MeshRenderer>().enabled = false;

            }
            else
            {
                platform.collider2D.enabled = true;
                platform.GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }

    public void rotatePlateFormsIfPlayerJumping(object sender, EventArgs e) //Fonction qui écoute le joueur pour savoir lorsqu'il saute
    {
        InfoFromPlayerToPlateformManagerArgs info = e as InfoFromPlayerToPlateformManagerArgs;
        numberPlatformVisible++;
        numberPlatformVisible = numberPlatformVisible % numberOfPlatforms + 1; //Permet d'ajouter 1 au nb caractérisant la position dans le cycle des plateformes
        StartCoroutine(displayOnlyPlateformOfNumber(numberPlatformVisible, timeBeforeSwitchPlateforms)); //Fct affichant uniquement les plateformes portant le numero voulu, et desaffiche les autres.
    }
    

}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MikrosClient;
using MikrosClient.Analytics;

public class Play : MonoBehaviour
{
    public GameObject test2;
    NewBehaviourScript gameControl;

    void Start()
    {
        gameControl = test2.GetComponent<NewBehaviourScript>();
    }

    public void OnButtonPress()
    {   
        if (!gameControl.gameRan)
        {
            gameControl.game = true;
            gameControl.menu = false;
        }
    }
}

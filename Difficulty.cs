using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Difficulty : MonoBehaviour
{
    public GameObject test2;
    NewBehaviourScript gameControl;

    void Start()
    {
        gameControl = test2.GetComponent<NewBehaviourScript>();
    }

    void Update()
    {
        this.GetComponent<Text>().text = "Difficulty: " + gameControl.n;
    }
}

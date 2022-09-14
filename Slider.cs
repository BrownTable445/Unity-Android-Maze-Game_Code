using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slider : MonoBehaviour
{
    public GameObject test2;
    NewBehaviourScript gameControl;

    void Start()
    {
        gameControl = test2.GetComponent<NewBehaviourScript>();
    }

    public void OnSliderMoved()
    {
        gameControl.n = (int)GameObject.Find("Slider").GetComponent<UnityEngine.UI.Slider>().value;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "Sphere") {
            Debug.Log("hi");
            GameObject.Find("Spawner").GetComponent<NewBehaviourScript>().finished = true;
        }
    }
}

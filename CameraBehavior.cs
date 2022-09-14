using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class CameraBehavior : MonoBehaviour
{
    bool[] states = new bool[6]; //which plane the sphere is on, 0 -> arrParent; 1 -> arrbParent; 2 -> arrlParent; 3 ->  arrRParent; 4 -> arrfParent; 5 ->arrbackParent
    bool topRotateFront = false, frontRotateLeft = false, leftRotateBack = false, backRotateRight = false, rightRotateBottom = false; //probably should make another bool array which is for storing the states of the rotations
    List<int> previousStates;


    void Start()
    {

    }

    public void SetFalse()
    {
        for (int i = 0; i < 6; i++)
            states[i] = false;
    }

    void Update()
    {
        var sphere = GameObject.Find("Sphere");

        if (GameObject.Find("top").gameObject.GetComponent<Collider>().bounds.Contains(sphere.transform.position))
        {
            bool[] temp = new bool[6];
            temp[0] = true;
            if (!Enumerable.SequenceEqual(states, temp))
            {
                SetFalse();
            }
            states[0] = true;
        } 
        else if (GameObject.Find("topb").gameObject.GetComponent<Collider>().bounds.Contains(sphere.transform.position))
        {
            bool[] temp = new bool[6];
            temp[1] = true;
            if (!Enumerable.SequenceEqual(states, temp))
            {
                if (states[3] == true)
                {
                    rightRotateBottom = true;
                    SetFalse();
                }
            }
            states[1] = true;
        } 
        else if (GameObject.Find("topl").gameObject.GetComponent<Collider>().bounds.Contains(sphere.transform.position))
        {
            bool[] temp = new bool[6];
            temp[2] = true;
            if (!Enumerable.SequenceEqual(states, temp))
            {
                if (states[4] == true)
                {
                    frontRotateLeft = true;
                    SetFalse();
                }
            }
            states[2] = true;
        }
        else if (GameObject.Find("topR").gameObject.GetComponent<Collider>().bounds.Contains(sphere.transform.position))
        {
            bool[] temp = new bool[6];
            temp[3] = true;
            if (!Enumerable.SequenceEqual(states, temp))
            {
                if (states[5] == true)
                {
                    backRotateRight = true;
                    SetFalse();
                }
            }
            states[3] = true;
        }
        else if (GameObject.Find("topf").gameObject.GetComponent<Collider>().bounds.Contains(sphere.transform.position))
        {
            bool[] temp = new bool[6];
            temp[4] = true;
            if (!Enumerable.SequenceEqual(states, temp))
            {
                if (states[0] == true)
                {
                    topRotateFront = true;
                    SetFalse();
                }
            }
            states[4] = true;
        }
        else if (GameObject.Find("topback").gameObject.GetComponent<Collider>().bounds.Contains(sphere.transform.position))
        {
            bool[] temp = new bool[6];
            temp[5] = true;
            if (!Enumerable.SequenceEqual(states, temp))
            {
                if (states[2] == true)
                {
                    leftRotateBack = true;
                    SetFalse();
                }
            }
            states[5] = true;
        }

        if (topRotateFront)
        {
            var temp = GameObject.Find("MasterCube");
            temp.transform.rotation = Quaternion.RotateTowards(temp.transform.rotation, Quaternion.Euler(0, 0, 90), 100 * Time.deltaTime);
            if (temp.transform.rotation == Quaternion.Euler(0, 0, 90))
                topRotateFront = false;
            var spawner = GameObject.Find("Spawner");
            spawner.GetComponent<NewBehaviourScript>().rotatedTopFront = true;
        }
        else if (frontRotateLeft)
        {
            var temp = GameObject.Find("MasterCube");
            temp.transform.rotation = Quaternion.RotateTowards(temp.transform.rotation, Quaternion.Euler(90, 0, 90), 100 * Time.deltaTime);
            if (temp.transform.rotation == Quaternion.Euler(90, 0, 90))
                frontRotateLeft = false;
            var spawner = GameObject.Find("Spawner");
            spawner.GetComponent<NewBehaviourScript>().rotatedFrontLeft = true;
        }
        else if (leftRotateBack)
        {
            var temp = GameObject.Find("MasterCube");
            temp.transform.rotation = Quaternion.RotateTowards(temp.transform.rotation, Quaternion.Euler(180, 0, 90), 100 * Time.deltaTime);
            if (temp.transform.rotation == Quaternion.Euler(180, 0, 90))
                leftRotateBack = false;
            var spawner = GameObject.Find("Spawner");
            spawner.GetComponent<NewBehaviourScript>().rotatedLeftBack = true;
        }
        else if (backRotateRight)
        {
            var temp = GameObject.Find("MasterCube");
            temp.transform.rotation = Quaternion.RotateTowards(temp.transform.rotation, Quaternion.Euler(270, 0, 90), 100 * Time.deltaTime); //maybe I could just write quaternion.Euler(temp.tranform.x + 90...)
            if (temp.transform.rotation == Quaternion.Euler(270, 0, 90))
                backRotateRight = false;
            var spawner = GameObject.Find("Spawner");
            spawner.GetComponent<NewBehaviourScript>().rotatedBackRight = true;
        }
        else if (rightRotateBottom)
        {
            var temp = GameObject.Find("MasterCube");
            temp.transform.rotation = Quaternion.RotateTowards(temp.transform.rotation, Quaternion.Euler(0, 0, 180), 100 * Time.deltaTime); //maybe I could just write quaternion.Euler(temp.tranform.x + 90...)
            if (temp.transform.rotation == Quaternion.Euler(0, 0, 180))
                rightRotateBottom = false;
            var spawner = GameObject.Find("Spawner");
            spawner.GetComponent<NewBehaviourScript>().rotatedRightBottom = true;
        }
    }
}

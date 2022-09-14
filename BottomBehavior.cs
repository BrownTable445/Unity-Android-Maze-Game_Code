using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BottomBehavior : MonoBehaviour
{
    bool[] states = new bool[6]; //which plane the sphere is on, 0 -> arrParent; 1 -> arrbParent; 2 -> arrlParent; 3 ->  arrRParent; 4 -> arrfParent; 5 ->arrbackParent
    bool topRotateFront = false, frontRotateLeft = false, leftRotateBack = false, backRotateRight = false, rightRotateBottom = false; //probably should make another bool array which is for storing the states of the rotations
    List<int> previousStates;

    void Start()
    {
        this.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    public void SetFalse()
    {
        for (int i = 0; i < 6; i++)
            states[i] = false;
    }

    /*void OnCollisionEnter(Collision col) //what I should do is add a Bounds component to see if the sphere is within it
    {
        if (col.transform.parent.gameObject.name == "arrParent" && col.gameObject.name == "Cube")
        {
            bool[] temp = new bool[6];
            temp[0] = true;
            if (!Enumerable.SequenceEqual(states, temp))
            {
                SetFalse();
            }
            states[0] = true;
        }
        else if (col.transform.parent.gameObject.name == "arrbParent" && col.gameObject.name == "Cube")
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
        else if (col.transform.parent.gameObject.name == "arrlParent" && col.gameObject.name == "Cube")
        {
            bool[] temp = new bool[6];
            temp[2] = true;
            if (!Enumerable.SequenceEqual(states, temp))
            {
                //Debug.Log("hi");
                if (states[4] == true)
                {
                    frontRotateLeft = true;
                    SetFalse();
                }
            }
            states[2] = true;
        }
        else if (col.transform.parent.gameObject.name == "arrRParent" && col.gameObject.name == "Cube")
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
        else if (col.transform.parent.gameObject.name == "arrfParent" && col.gameObject.name == "Cube")
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
        else if (col.transform.parent.gameObject.name == "arrbackParent" && col.gameObject.name == "Cube")
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
    }*/

    /*void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.name == "arr")
        {
            bool[] temp = new bool[6];
            temp[0] = true;
            if (!Enumerable.SequenceEqual(states, temp))
            {
                SetFalse();
            }
            states[0] = true;
        }
        else if (col.gameObject.name == "topb")
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
        else if (col.gameObject.name == "topl")
        {
            bool[] temp = new bool[6];
            temp[2] = true;
            if (!Enumerable.SequenceEqual(states, temp))
            {
                //Debug.Log("hi");
                if (states[4] == true)
                {
                    frontRotateLeft = true;
                    SetFalse();
                }
            }
            states[2] = true;
        }
        else if (col.gameObject.name == "topR")
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
        else if (col.gameObject.name == "topf")
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
        else if (col.gameObject.name == "topback")
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
    }*/

    void Update()
    {
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

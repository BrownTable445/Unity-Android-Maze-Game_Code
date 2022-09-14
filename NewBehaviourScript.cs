using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MikrosClient;
using MikrosClient.Analytics;
using MikrosClient.Authentication;
using System.Globalization;
using System;
using Random = UnityEngine.Random;

public class NewBehaviourScript : MonoBehaviour
{
    public int n = 20;
    public GameObject myCanvas;
    public bool menu = true, game = false, gameRan = false, menuRan = false, rotatedTopFront = false, rotatedFrontLeft = false, rotatedLeftBack = false, rotatedBackRight = false, rotatedRightBottom = false, finished = false;
    //public bool[5] states = false; //which plane the sphere is on, 0 -> arrParent; 1 -> arrbParent; 2 -> arrlParent

    int exitX, exitY, counti = 0, countj = 0;

    bool[,] met;
    bool gamne = false;

    float cartesianX, cartesianY, gameStartSeconds, timer = 0; 

    GameObject[,] array, array2;
    GameObject sphere, congradulations, myText, play, bottom, finishLine, cameraPivot, invisibleColliderTopFront, invisibleColliderFrontLeft, invisibleColliderLeftBack, invisibleColliderBackRight, invisibleColliderRightBottom;

    Vector3 inputVector;

    Material newMat;

    void Start()
    {
        menu = true;
        Screen.orientation = ScreenOrientation.LandscapeRight;
        newMat = Resources.Load("Default", typeof(Material)) as Material;

        cameraPivot = new GameObject("cameraPivot");
        Camera.main.transform.SetParent(cameraPivot.transform);

        MikrosManager.Instance.AuthenticationController.LaunchSignin();

        MikrosManager.Instance.AnalyticsController.LogEvent("Game Launched", (Hashtable customEventWholeData) =>
        {
            // handle success
            Debug.Log("Success");
        },
        onFailure =>
        {
            // handle failure
            Debug.Log("Failure");
        });

        //MikrosManager.Instance.AnalyticsController.FlushEvents();
    }

    bool Traversed(bool[,] met)
    {
        bool result = true;
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                if (met[i, j] == false)
                    result = false;
        return result;
    }

    bool Possible(bool[,] met, int x, int y)
    {
        bool result = true;

        if (x == 0 && y == 0) //top left
        {
            if (met[0, 1] == true && met[1, 0] == true)
                return false;
        }
        else if (x == n - 1 && y == 0) //top right
        {
            if (met[n - 2, 0] == true && met[n - 1, 1] == true)
                return false;
        }
        else if (x == 0 && y == n - 1) //bottom left
        {
            if (met[1, n - 1] == true && met[0, n - 2] == true)
                return false;
        }
        else if (x == n - 1 && y == n - 1) //bottom right
        {
            if (met[n - 2, n - 1] == true && met[n - 1, n - 2] == true)
                return false;
        }
        else if (x == 0) //left
        {
            if (met[0, y - 1] == true && met[0, y + 1] == true && met[1, y] == true)
                return false;
        }
        else if (x == n - 1) //right
        {
            if (met[x, y - 1] == true && met[x, y + 1] == true && met[x - 1, y] == true)
                return false;
        }
        else if (y == 0) //top
        {
            if (met[x - 1, 0] == true && met[x + 1, 0] == true && met[x, 1] == true)
                return false;
        }
        else if (y == n - 1) //bottom
        {
            if (met[x - 1, y] == true && met[x + 1, y] == true && met[x, y - 1] == true)
                return false;
        }
        else
        {
            if (met[x - 1, y] == true && met[x + 1, y] == true && met[x, y - 1] == true && met[x, y + 1] == true)
                return false;
        }
        return result;
    }

    void GenerateMaze(GameObject[,] arr, GameObject[,] arr2, GameObject arrParent)
    {
        float n2 = n;
        bool[,] met = new bool[n, n];
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                met[i, j] = false;

        for (float i = -n2 / 2; i <= n2 / 2; i++)
        {
            for (float j = n2 / 2 - 0.5f; j >= -n2 / 2; j--)
            {
                arr[counti, countj] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                arr[counti, countj].GetComponent<Renderer>().material = newMat;
                arr[counti, countj].transform.position = new Vector3(i, (float)n / 2, j);
                arr[counti, countj].transform.localScale = new Vector3(0.1f, 1, 1);
                arr[counti, countj].transform.SetParent(arrParent.transform);
                countj++;
            }
            countj = 0;
            counti++;
        }

        countj = 0;
        counti = 0;

        for (float i = n2 / 2; i >= -n2 / 2; i--)
        {
            for (float j = -n2 / 2 + 0.5f; j <= n2 / 2; j++)
            {
                arr2[counti, countj] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                arr2[counti, countj].GetComponent<Renderer>().material = newMat;
                arr2[counti, countj].transform.position = new Vector3(j, (float)n / 2, i);
                arr2[counti, countj].transform.localScale = new Vector3(1, 1, 0.1f);
                arr2[counti, countj].transform.SetParent(arrParent.transform);
                countj++;
            }
            countj = 0;
            counti++;
        }

        counti = 0;
        countj = 0;

        array = arr;
        array2 = arr2;

        int x = Random.Range(0, n);
        int y = Random.Range(0, n);
        met[x, y] = true;
        List<List<int>> previousMoves = new List<List<int>>();
        while (!Traversed(met))
        {
            met[x, y] = true;
            if (Possible(met, x, y))
            {
                if (x == 0 && y == 0)//top left corner
                {
                    int temp = Random.Range(0, 2); //what if it is solvable? im checking if the next moves are NOT solvable, which means I will never get out of the for loop
                    if (temp == 0 && met[1, 0] == false)
                    {
                        previousMoves.Add(new List<int> { x, y });
                        Destroy(arr[1, 0]);
                        x++;
                    }
                    else if (met[0, 1] == false)
                    {
                        previousMoves.Add(new List<int> { x, y });
                        Destroy(arr2[1, 0]);
                        y++;
                    }
                }
                else if (x == n - 1 && y == 0) //top right corner
                {
                    int temp = Random.Range(0, 2);
                    if (temp == 0 && met[x - 1, 0] == false)
                    {
                        previousMoves.Add(new List<int> { x, y });
                        Destroy(arr[n - 1, 0]);
                        x--;
                    }
                    else if (met[x, 1] == false)
                    {
                        previousMoves.Add(new List<int> { x, y });
                        Destroy(arr2[1, x]);
                        y++;
                    }
                }
                else if (x == 0 && y == n - 1) //bottom left corner
                {

                    int temp = Random.Range(0, 2);
                    if (temp == 0 && met[0, y - 1] == false)
                    {
                        previousMoves.Add(new List<int> { x, y });
                        Destroy(arr2[n - 1, 0]);
                        y--;
                    }
                    else if (met[1, y] == false)
                    {
                        previousMoves.Add(new List<int> { x, y });
                        Destroy(arr[1, n - 1]);
                        x++;
                    }
                }
                else if (x == n - 1 && y == n - 1) //bottom right corner
                {
                    int temp = Random.Range(0, 2);
                    if (temp == 0 && met[x, y - 1] == false)
                    {
                        previousMoves.Add(new List<int> { x, y });
                        Destroy(arr2[n - 1, n - 1]);
                        y--;
                    }
                    else if (met[x - 1, y] == false)
                    {
                        previousMoves.Add(new List<int> { x, y });
                        Destroy(arr[n - 1, n - 1]);
                        x--;
                    }
                }
                else if (y == 0) //top row
                {
                    int temp = Random.Range(0, 3);
                    switch (temp)
                    {
                        case 0:
                            if (met[x, 1] == false)
                            {
                                previousMoves.Add(new List<int> { x, y });
                                Destroy(arr2[1, x]);
                                y++;
                            }
                            break;
                        case 1:
                            if (met[x - 1, 0] == false)
                            {
                                previousMoves.Add(new List<int> { x, y });
                                Destroy(arr[x, 0]);
                                x--;
                            }
                            break;
                        case 2:
                            if (met[x + 1, 0] == false)
                            {
                                previousMoves.Add(new List<int> { x, y });
                                Destroy(arr[x + 1, 0]);
                                x++;
                            }
                            break;
                    }
                }
                else if (y == n - 1) //bottom row
                {
                    int temp = Random.Range(0, 3);
                    switch (temp)
                    {
                        case 0:
                            if (met[x, y - 1] == false)
                            {
                                previousMoves.Add(new List<int> { x, y });
                                Destroy(arr2[y, x]);
                                y--;
                            }
                            break;
                        case 1:
                            if (met[x - 1, y] == false)
                            {
                                previousMoves.Add(new List<int> { x, y });
                                Destroy(arr[x, y]);
                                x--;
                            }
                            break;
                        case 2:
                            if (met[x + 1, y] == false)
                            {
                                previousMoves.Add(new List<int> { x, y });
                                Destroy(arr[x + 1, y]);
                                x++;
                            }
                            break;
                    }
                }
                else if (x == 0) //left column
                {
                    int temp = Random.Range(0, 3);
                    switch (temp)
                    {
                        case 0:
                            if (met[0, y - 1] == false)
                            {
                                previousMoves.Add(new List<int> { x, y });
                                Destroy(arr2[y, 0]);
                                y--;
                            }
                            break;
                        case 1:
                            if (met[0, y + 1] == false)
                            {
                                previousMoves.Add(new List<int> { x, y });
                                Destroy(arr2[y + 1, 0]);
                                y++;
                            }
                            break;
                        case 2:
                            if (met[1, y] == false)
                            {
                                previousMoves.Add(new List<int> { x, y });
                                Destroy(arr[1, y]);
                                x++;
                            }
                            break;
                    }
                }
                else if (x == n - 1) //right column
                {
                    int temp = Random.Range(0, 3);
                    switch (temp)
                    {
                        case 0:
                            if (met[x, y - 1] == false)
                            {
                                previousMoves.Add(new List<int> { x, y });
                                Destroy(arr2[y, x]);
                                y--;
                            }
                            break;
                        case 1:
                            if (met[x, y + 1] == false)
                            {
                                previousMoves.Add(new List<int> { x, y });
                                Destroy(arr2[y + 1, x]);
                                y++;
                            }
                            break;
                        case 2:
                            if (met[x - 1, y] == false)
                            {
                                previousMoves.Add(new List<int> { x, y });
                                Destroy(arr[x, y]);
                                x--;
                            }
                            break;
                    }
                }
                else
                {
                    int temp = Random.Range(0, 4);
                    switch (temp)
                    {
                        case 0:
                            if (met[x, y - 1] == false)
                            {
                                previousMoves.Add(new List<int> { x, y });
                                Destroy(arr2[y, x]);
                                y--;
                            }
                            break;
                        case 1:
                            if (met[x, y + 1] == false)
                            {
                                previousMoves.Add(new List<int> { x, y });
                                Destroy(arr2[y + 1, x]);
                                y++;
                            }
                            break;
                        case 2:
                            if (met[x - 1, y] == false)
                            {
                                previousMoves.Add(new List<int> { x, y });
                                Destroy(arr[x, y]);
                                x--;
                            }
                            break;
                        case 3:
                            if (met[x + 1, y] == false)
                            {
                                previousMoves.Add(new List<int> { x, y });
                                Destroy(arr[x + 1, y]);
                                x++;
                            }
                            break;
                    }
                }
            }
            else
            {
                x = previousMoves[previousMoves.Count - 1][0];
                y = previousMoves[previousMoves.Count - 1][1];
                previousMoves.RemoveAt(previousMoves.Count - 1);
            }
        }

        for (int i2 = 0; i2 < n + 1; i2++) //giving them a boxcollider component
            for (int j2 = 0; j2 < n; j2++)
            {
                if (arr[i2, j2] != null)
                    arr[i2, j2].AddComponent<BoxCollider>();
                if (arr2[i2, j2] != null)
                    arr2[i2, j2].AddComponent<BoxCollider>();
            }
    }

    // Update is called once per frame
    void Update() //error could be that i need to create menuRan
    {
        //Debug.Log(rotatedTopFront);
        if (game) //WHAT IF I MADE ALL THE ARRAY PARENTS UNDER A SINGLE PARENT AND ROTATING THE PARENT INSTEAD OF THE CAMERA OMG OMG OMG
        {
            if (!finished)
                timer += Time.deltaTime;

            if (invisibleColliderTopFront != null)
                invisibleColliderTopFront.SetActive(rotatedTopFront);
            if (invisibleColliderFrontLeft != null)
                invisibleColliderFrontLeft.SetActive(rotatedFrontLeft);
            if (invisibleColliderLeftBack != null)
                invisibleColliderLeftBack.SetActive(rotatedLeftBack);
            if (invisibleColliderBackRight != null)
                invisibleColliderBackRight.SetActive(rotatedBackRight);
            if (invisibleColliderRightBottom != null)
                invisibleColliderRightBottom.SetActive(rotatedRightBottom);

            if (!gameRan)
                StartGame();

            inputVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            //bool temp = true;
            if (finished)
            {
                menu = true;
                game = false;

                Destroy(GameObject.Find("MasterCube"));

                menuRan = false;
                gameRan = false;
                finished = false;

                this.gameObject.GetComponent<CameraBehavior>().SetFalse();

                Hashtable paramdata2 = new Hashtable();
                paramdata2.Add("Difficulty", n);
                paramdata2.Add("Duration", timer);

                MikrosManager.Instance.AnalyticsController.LogEvent("EventName", paramdata2, (Hashtable customEventWholeData) =>
                {
                    // handle success
                    Debug.Log("Success");
                },
                onFailure =>
                {
                    Debug.Log("Failure");
                    // handle failure
                });

                if (PlayerPrefs.HasKey(n + ""))
                {
                    if (PlayerPrefs.GetFloat(n + "") > timer)
                    {
                        /*TrackUnlockAchievementRequest.Builder()
                        .AchievementId(n + "b")
                        .AchievementName("You solved it faster!")
                        .Create(
                        trackUnlockAchievementRequest => MikrosManager.Instance.AnalyticsController.LogEvent(trackUnlockAchievementRequest),
                        onFailure =>
                        {
                            // handle failure
                        });*/
                        PlayerPrefs.SetFloat(n + "", timer);

                        Hashtable paramdata = new Hashtable();
                        paramdata.Add("You_solved_it_faster!", timer);

                        MikrosManager.Instance.AnalyticsController.LogEvent("EventName", paramdata, (Hashtable customEventWholeData) =>
                        {
                            // handle success
                            Debug.Log("Success");
                        },
                        onFailure =>
                        {
                            Debug.Log("Failure");
                            // handle failure
                        });
                    }
                }
                else
                {
                    PlayerPrefs.SetFloat(n + "", timer);
                    /*TrackUnlockAchievementRequest.Builder()
                    .AchievementId(n + "a")
                    .AchievementName("You solved it for the first time!")
                    .Create(
                    trackUnlockAchievementRequest => MikrosManager.Instance.AnalyticsController.LogEvent(trackUnlockAchievementRequest),
                    onFailure =>
                    {
                        // handle failure
                    });*/

                    Hashtable paramdata = new Hashtable();
                    paramdata.Add("Fastest Time", timer);

                    MikrosManager.Instance.AnalyticsController.LogEvent("Fastest Time", paramdata, (Hashtable customEventWholeData) =>
                    {
                        // handle success
                        Debug.Log("Success");
                    },
                    onFailure =>
                    {
                        Debug.Log("Failure");
                        // handle failure
                    });
                }

                /*TrackLevelEndRequest.Builder()
                .Level(n)
                .LevelName(n + "")
                .Description("maze difficulty")
                .CompleteDuration(timer + "")
                .Success("success")
                .Create(
                trackLevelEndRequest => MikrosManager.Instance.AnalyticsController.LogEvent(trackLevelEndRequest),
                onFailure =>
                {
                    // handle failure
                });

                MikrosManager.Instance.AnalyticsController.LogEvent("maze difficulty", "level", n, (Hashtable customEventWholeData) =>
                {
                    // handle success
                },
                onFailure =>
                {
                    // handle failure
                });*/

                MikrosManager.Instance.AnalyticsController.FlushEvents();

                timer = 0;
            }
            //gameRan = temp;
        }
       // Canvas myCanvas = GetComponent<Canvas>();
        myCanvas.SetActive(menu);
        //GetComponent<Canvas>().SetActive(menu);
    }

    void StartGame()
    {
        if (!gameRan)
        {
            gameStartSeconds = Time.time; //will use this to get difference between now and when the game ends, which i will use MIKROS to upload to

            Camera.main.transform.position = new Vector3(0, n + 10, 0);
            Camera.main.transform.rotation = Quaternion.Euler(90, 0, 0);

            GameObject arrParent = new GameObject("arrParent");
            GameObject arrbParent = new GameObject("arrbParent");
            GameObject arrlParent = new GameObject("arrlParent");
            GameObject arrRParent = new GameObject("arrRParent");
            GameObject arrfParent = new GameObject("arrfParent");
            GameObject arrbackParent = new GameObject("arrbackParent");

            GameObject[,] arr = new GameObject[n + 1, n]; //vertical, top
            GameObject[,] arrb = new GameObject[n + 1, n]; //bottom of cube
            GameObject[,] arrl = new GameObject[n + 1, n]; //left side
            GameObject[,] arrR = new GameObject[n + 1, n]; //right
            GameObject[,] arrf = new GameObject[n + 1, n]; //front
            GameObject[,] arrback = new GameObject[n + 1, n]; //back

            GameObject[,] arr2 = new GameObject[n + 1, n]; //vertical, top
            GameObject[,] arr2b = new GameObject[n + 1, n]; //bottom of cube
            GameObject[,] arr2l = new GameObject[n + 1, n]; //left side
            GameObject[,] arr2R = new GameObject[n + 1, n]; //right
            GameObject[,] arr2f = new GameObject[n + 1, n]; //front
            GameObject[,] arr2back = new GameObject[n + 1, n]; //back

            GenerateMaze(arr, arr2, arrParent);
            GenerateMaze(arrb, arr2b, arrbParent);
            GenerateMaze(arrl, arr2l, arrlParent);
            GenerateMaze(arrR, arr2R, arrRParent);
            GenerateMaze(arrf, arr2f, arrfParent);
            GenerateMaze(arrback, arr2back, arrbackParent);

            GameObject cube = new GameObject("MasterCube");
            arrParent.transform.SetParent(cube.transform);
            arrbParent.transform.SetParent(cube.transform);
            arrlParent.transform.SetParent(cube.transform);
            arrRParent.transform.SetParent(cube.transform);
            arrfParent.transform.SetParent(cube.transform);
            arrbackParent.transform.SetParent(cube.transform);

            //top
            GameObject bottom = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bottom.name = "bottom";
            bottom.transform.localScale = new Vector3(n, 1, n);
            bottom.transform.position = new Vector3(0, -1 + (float)n / 2, 0);
            bottom.AddComponent<BoxCollider>();
            var bottomRenderer = bottom.GetComponent<Renderer>();
            bottomRenderer.material.SetColor("_Color", Color.gray);
            bottom.transform.SetParent(arrParent.transform);

            GameObject top = GameObject.CreatePrimitive(PrimitiveType.Cube);
            top.name = "top";
            top.transform.localScale = new Vector3(n, 1, n);
            top.transform.position = new Vector3(0, (float)n / 2, 0);
            top.GetComponent<Collider>().isTrigger = true;
            top.GetComponent<Renderer>().GetComponent<MeshRenderer>().enabled = false;
            top.transform.SetParent(arrParent.transform);

            //bottom
            GameObject bottomb = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bottomb.name = "bottomb";
            bottomb.transform.localScale = new Vector3(n, 1, n);
            bottomb.transform.position = new Vector3(0, -1 + (float)n / 2, 0);
            bottomb.AddComponent<BoxCollider>();
            var bottombRenderer = bottomb.GetComponent<Renderer>();
            bottombRenderer.material.SetColor("_Color", Color.gray);
            bottomb.transform.SetParent(arrbParent.transform);

            GameObject topb = GameObject.CreatePrimitive(PrimitiveType.Cube);
            topb.name = "topb";
            topb.transform.localScale = new Vector3(n, 1, n);
            topb.transform.position = new Vector3(0, (float)n / 2, 0);
            topb.GetComponent<Collider>().isTrigger = true;
            topb.GetComponent<Renderer>().GetComponent<MeshRenderer>().enabled = false;
            topb.transform.SetParent(arrbParent.transform);

            //left
            GameObject bottoml = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bottoml.name = "bottoml";
            bottoml.transform.localScale = new Vector3(n - 0.1f, 1, n - 0.1f);
            bottoml.transform.position = new Vector3(0, -1 + (float)n / 2, 0);
            bottoml.AddComponent<BoxCollider>();
            var bottomlRenderer = bottoml.GetComponent<Renderer>();
            bottomlRenderer.material.SetColor("_Color", Color.gray);
            bottoml.transform.SetParent(arrlParent.transform);

            GameObject topl = GameObject.CreatePrimitive(PrimitiveType.Cube);
            topl.name = "topl";
            topl.transform.localScale = new Vector3(n, 1, n);
            topl.transform.position = new Vector3(0, (float)n / 2, 0);
            topl.GetComponent<Collider>().isTrigger = true;
            topl.GetComponent<Renderer>().GetComponent<MeshRenderer>().enabled = false;
            topl.transform.SetParent(arrlParent.transform);

            //right
            GameObject bottomR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bottomR.name = "bottomR";
            bottomR.transform.localScale = new Vector3(n - 0.1f, 1, n - 0.1f);
            bottomR.transform.position = new Vector3(0, -1 + (float)n / 2, 0);
            bottomR.AddComponent<BoxCollider>();
            var bottomRRenderer = bottomR.GetComponent<Renderer>();
            bottomRRenderer.material.SetColor("_Color", Color.gray);
            bottomR.transform.SetParent(arrRParent.transform);

            GameObject topR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            topR.name = "topR";
            topR.transform.localScale = new Vector3(n, 1, n);
            topR.transform.position = new Vector3(0, (float)n / 2, 0);
            topR.GetComponent<Collider>().isTrigger = true;
            topR.GetComponent<Renderer>().GetComponent<MeshRenderer>().enabled = false;
            topR.transform.SetParent(arrRParent.transform);

            //front
            GameObject bottomf = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bottomf.name = "bottomf";
            bottomf.transform.localScale = new Vector3(n - 0.1f, 1, n - 0.1f);
            bottomf.transform.position = new Vector3(0, -1 + (float)n / 2, 0);
            bottomf.AddComponent<BoxCollider>();
            var bottomfRenderer = bottomf.GetComponent<Renderer>();
            bottomfRenderer.material.SetColor("_Color", Color.gray);
            bottomf.transform.SetParent(arrfParent.transform);

            GameObject topf = GameObject.CreatePrimitive(PrimitiveType.Cube);
            topf.name = "topf";
            topf.transform.localScale = new Vector3(n, 1, n);
            topf.transform.position = new Vector3(0, (float)n / 2, 0);
            topf.GetComponent<Collider>().isTrigger = true;
            topf.GetComponent<Renderer>().GetComponent<MeshRenderer>().enabled = false;
            topf.transform.SetParent(arrfParent.transform);
            //back
            GameObject bottomback = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bottomback.name = "bottomback";
            bottomback.transform.localScale = new Vector3(n - 0.1f, 1, n - 0.1f);
            bottomback.transform.position = new Vector3(0, -1 + (float)n / 2, 0);
            bottomback.AddComponent<BoxCollider>();
            var bottombackRenderer = bottomback.GetComponent<Renderer>();
            bottombackRenderer.material.SetColor("_Color", Color.gray);
            bottomback.transform.SetParent(arrbackParent.transform);

            GameObject topback = GameObject.CreatePrimitive(PrimitiveType.Cube);
            topback.name = "topback";
            topback.transform.localScale = new Vector3(n, 1, n);
            topback.transform.position = new Vector3(0, (float)n / 2, 0);
            topback.GetComponent<Collider>().isTrigger = true;
            topback.GetComponent<Renderer>().GetComponent<MeshRenderer>().enabled = false;
            topback.transform.SetParent(arrbackParent.transform);

            //finishLine
            float finishX = Random.Range(0, n);
            float finishY = Random.Range(0, n);

            if (n % 2 == 0)
            {
                if (finishX < n / 2)
                    finishX = -n / 2 + 0.5f;
                else
                    finishX = n / 2 - 0.5f;

                if (finishY < n / 2)
                    finishY = -n / 2 + 0.5f;
                else
                    finishY = n / 2 - 0.5f;
            }
            else
            {
                if (finishX < n / 2)
                {
                    finishX = -n / 2;
                }
                else if (finishX == n / 2)
                {
                    finishX = 0;
                }
                else
                {
                    finishX = n / 2;
                }

                if (finishY < n / 2)
                {
                    finishY = -n / 2;
                }
                else if (finishY == n / 2)
                {
                    finishY = 0;
                }
                else
                {
                    finishY = n / 2;
                }
            }

            finishLine = GameObject.CreatePrimitive(PrimitiveType.Cube);
            finishLine.transform.position = new Vector3(finishX, -0.5f + n / 2.0f + 0.01f, finishY);
            finishLine.transform.localScale = new Vector3(0.05f, 1, 1);
            finishLine.transform.rotation = Quaternion.Euler(0, 0, 90);
            finishLine.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
            finishLine.AddComponent<Finish>();
            finishLine.AddComponent<Rigidbody>();
            finishLine.transform.SetParent(bottomb.transform);


            //rotate the parent
            arrbParent.transform.rotation = Quaternion.Euler(180, 0, 0);
            arrbParent.transform.position = new Vector3(arrbParent.transform.position.x, arrbParent.transform.position.y - 0.5f, arrbParent.transform.position.z);
            arrParent.transform.position = new Vector3(arrParent.transform.position.x, arrParent.transform.position.y + 0.5f, arrParent.transform.position.z);
            arrlParent.transform.rotation = Quaternion.Euler(-90, 0, 0);
            arrlParent.transform.position = new Vector3(arrlParent.transform.position.x, arrlParent.transform.position.y, arrlParent.transform.position.z - 0.5f);
            arrRParent.transform.rotation = Quaternion.Euler(90, 0, 0);
            arrRParent.transform.position = new Vector3(arrRParent.transform.position.x, arrRParent.transform.position.y, arrRParent.transform.position.z + 0.5f);
            arrfParent.transform.rotation = Quaternion.Euler(0, 0, -90);
            arrfParent.transform.position = new Vector3(arrfParent.transform.position.x + 0.5f, arrfParent.transform.position.y, arrfParent.transform.position.z);
            arrbackParent.transform.rotation = Quaternion.Euler(0, 0, 90);
            arrbackParent.transform.position = new Vector3(arrbackParent.transform.position.x - 0.5f, arrbackParent.transform.position.y, arrbackParent.transform.position.z);

            top.transform.SetParent(cube.transform);
            topb.transform.SetParent(cube.transform);
            topl.transform.SetParent(cube.transform);
            topR.transform.SetParent(cube.transform);
            topf.transform.SetParent(cube.transform);
            topback.transform.SetParent(cube.transform);

            //adding invisible colliders
            GameObject invisibleColliderTop = GameObject.CreatePrimitive(PrimitiveType.Cube);
            invisibleColliderTop.name = "invisibleColliderTop";
            invisibleColliderTop.transform.localScale = new Vector3(n + 2, 1, n + 2);
            invisibleColliderTop.transform.position = new Vector3(0, 1.5f + (float)n / 2, 0);
            invisibleColliderTop.AddComponent<BoxCollider>();
            var invisibleColliderTopRenderer = invisibleColliderTop.GetComponent<Renderer>();
            invisibleColliderTopRenderer.material.SetColor("_Color", Color.gray);
            invisibleColliderTopRenderer.GetComponent<MeshRenderer>().enabled = false;
            invisibleColliderTop.transform.SetParent(cube.transform);

            GameObject invisibleColliderBottom = GameObject.CreatePrimitive(PrimitiveType.Cube);
            invisibleColliderBottom.name = "invisibleColliderBottom";
            invisibleColliderBottom.transform.localScale = new Vector3(n + 2, 1, n + 2);
            invisibleColliderBottom.transform.position = new Vector3(0, -(float)n / 2 - 1.5f, 0);
            invisibleColliderBottom.AddComponent<BoxCollider>();
            var invisibleColliderBottomRenderer = invisibleColliderBottom.GetComponent<Renderer>();
            invisibleColliderBottomRenderer.material.SetColor("_Color", Color.gray);
            invisibleColliderBottomRenderer.GetComponent<MeshRenderer>().enabled = false;
            invisibleColliderBottomRenderer.transform.SetParent(cube.transform);

            GameObject invisibleColliderLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
            invisibleColliderLeft.name = "invisibleColliderLeft";
            invisibleColliderLeft.transform.localScale = new Vector3(n + 2, 1, n + 2);
            invisibleColliderLeft.transform.position = new Vector3(0, 0, n / 2 + 1.5f);
            invisibleColliderLeft.transform.rotation = Quaternion.Euler(90, 0, 0);
            invisibleColliderLeft.AddComponent<BoxCollider>();
            var invisibleColliderLeftRenderer = invisibleColliderLeft.GetComponent<Renderer>();
            invisibleColliderLeftRenderer.material.SetColor("_Color", Color.gray);
            invisibleColliderLeftRenderer.GetComponent<MeshRenderer>().enabled = false;
            invisibleColliderLeftRenderer.transform.SetParent(cube.transform);

            GameObject invisibleColliderRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
            invisibleColliderRight.name = "invisibleColliderRight";
            invisibleColliderRight.transform.localScale = new Vector3(n + 2, 1, n + 2);
            invisibleColliderRight.transform.position = new Vector3(0, 0, -n / 2 - 1.5f);
            invisibleColliderRight.transform.rotation = Quaternion.Euler(90, 0, 0);
            invisibleColliderRight.AddComponent<BoxCollider>();
            var invisibleColliderRightRenderer = invisibleColliderRight.GetComponent<Renderer>();
            invisibleColliderRightRenderer.material.SetColor("_Color", Color.gray);
            invisibleColliderRightRenderer.GetComponent<MeshRenderer>().enabled = false;
            invisibleColliderRightRenderer.transform.SetParent(cube.transform);

            GameObject invisibleColliderFront = GameObject.CreatePrimitive(PrimitiveType.Cube);
            invisibleColliderFront.name = "invisibleColliderFront";
            invisibleColliderFront.transform.localScale = new Vector3(n + 2, 1, n + 2);
            invisibleColliderFront.transform.position = new Vector3(-n / 2 - 1.5f, 0, 0);
            invisibleColliderFront.transform.rotation = Quaternion.Euler(0, 0, 90);
            invisibleColliderFront.AddComponent<BoxCollider>();
            var invisibleColliderFrontRenderer = invisibleColliderFront.GetComponent<Renderer>();
            invisibleColliderFrontRenderer.material.SetColor("_Color", Color.gray);
            invisibleColliderFrontRenderer.GetComponent<MeshRenderer>().enabled = false;
            invisibleColliderFrontRenderer.transform.SetParent(cube.transform);

            GameObject invisibleColliderBack = GameObject.CreatePrimitive(PrimitiveType.Cube);
            invisibleColliderBack.name = "invisibleColliderBack";
            invisibleColliderBack.transform.localScale = new Vector3(n + 2, 1, n + 2);
            invisibleColliderBack.transform.position = new Vector3(n / 2 + 1.5f, 0, 0);
            invisibleColliderBack.transform.rotation = Quaternion.Euler(0, 0, 90);
            invisibleColliderBack.AddComponent<BoxCollider>();
            var invisibleColliderBackRenderer = invisibleColliderBack.GetComponent<Renderer>();
            invisibleColliderBackRenderer.material.SetColor("_Color", Color.gray);
            invisibleColliderBackRenderer.GetComponent<MeshRenderer>().enabled = false;
            invisibleColliderBackRenderer.transform.SetParent(cube.transform);

            invisibleColliderTopFront = GameObject.CreatePrimitive(PrimitiveType.Cube);
            invisibleColliderTopFront.name = "invisibleColliderTopFront";
            invisibleColliderTopFront.transform.localScale = new Vector3(1, 1, n);
            invisibleColliderTopFront.transform.position = new Vector3(n / 2.0f + 0.5f, n / 2.0f + 0.5f, 0);
            invisibleColliderBack.AddComponent<BoxCollider>();
            var invisibleColliderTopFrontRenderer = invisibleColliderTopFront.GetComponent<Renderer>();
            invisibleColliderTopFrontRenderer.material.SetColor("_Color", Color.gray);
            invisibleColliderTopFrontRenderer.GetComponent<MeshRenderer>().enabled = false;
            invisibleColliderTopFrontRenderer.transform.SetParent(cube.transform);

            invisibleColliderFrontLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
            invisibleColliderFrontLeft.name = "invisibleColliderFrontLeft"; //should remove names later once everything seems to be working, names are only there for debugging purposes
            invisibleColliderFrontLeft.transform.localScale = new Vector3(1, n, 1);
            invisibleColliderFrontLeft.transform.position = new Vector3(n / 2.0f + 0.5f, 0, -n / 2.0f - 0.5f); //perhaps i need to replace all n/2 with n/2.0f because it won't work well if it's odd
            invisibleColliderFrontLeft.AddComponent<BoxCollider>();
            var invisibleColliderFrontLeftRenderer = invisibleColliderFrontLeft.GetComponent<Renderer>();
            invisibleColliderFrontLeftRenderer.material.SetColor("_Color", Color.gray);
            invisibleColliderFrontLeftRenderer.GetComponent<MeshRenderer>().enabled = false;
            invisibleColliderFrontLeftRenderer.transform.SetParent(cube.transform);

            invisibleColliderLeftBack = GameObject.CreatePrimitive(PrimitiveType.Cube);
            invisibleColliderLeftBack.name = "invisibleColliderLeftBack";
            invisibleColliderLeftBack.transform.localScale = new Vector3(1, n, 1);
            invisibleColliderLeftBack.transform.position = new Vector3(-n / 2 - 0.5f, 0, -n / 2 - 0.5f);
            invisibleColliderLeftBack.AddComponent<BoxCollider>();
            var invisibleColliderLeftBackRenderer = invisibleColliderLeftBack.GetComponent<Renderer>();
            invisibleColliderLeftBackRenderer.material.SetColor("_Color", Color.gray);
            invisibleColliderLeftBackRenderer.GetComponent<MeshRenderer>().enabled = false;
            invisibleColliderLeftBackRenderer.transform.SetParent(cube.transform);

            invisibleColliderBackRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
            invisibleColliderBackRight.name = "invisibleColliderBackRight";
            invisibleColliderBackRight.transform.localScale = new Vector3(1, n, 1);
            invisibleColliderBackRight.transform.position = new Vector3(-n / 2 - 0.5f, 0, n / 2 + 0.5f);
            invisibleColliderBackRight.AddComponent<BoxCollider>();
            var invisibleColliderBackRightRenderer = invisibleColliderBackRight.GetComponent<Renderer>();
            invisibleColliderBackRightRenderer.material.SetColor("_Color", Color.gray);
            invisibleColliderBackRightRenderer.GetComponent<MeshRenderer>().enabled = false;
            invisibleColliderBackRightRenderer.transform.SetParent(cube.transform);

            invisibleColliderRightBottom = GameObject.CreatePrimitive(PrimitiveType.Cube);
            invisibleColliderRightBottom.name = "invisibleColliderBackRight";
            invisibleColliderRightBottom.transform.localScale = new Vector3(n, 1, 1);
            invisibleColliderRightBottom.transform.position = new Vector3(0, -n / 2.0f - 0.5f, n / 2.0f + 0.5f);
            invisibleColliderRightBottom.AddComponent<BoxCollider>();
            var invisibleColliderRightBottomRenderer = invisibleColliderRightBottom.GetComponent<Renderer>();
            invisibleColliderRightBottomRenderer.material.SetColor("_Color", Color.gray);
            invisibleColliderRightBottomRenderer.GetComponent<MeshRenderer>().enabled = false;
            invisibleColliderRightBottomRenderer.transform.SetParent(cube.transform);

            int[] arr10 = new int[n];
            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            sphere.AddComponent<Rigidbody>();
            float increment = 0;

            int firstDestroyed = Random.Range(0, n);

            if (n % 2 == 0)
            {
                for (int i2 = 0; i2 < n; i2++)
                {
                    if (i2 >= n / 2)
                    {
                        arr10[i2] = -n / 2 + i2 + 1;
                    }
                    else arr10[i2] = -n / 2 + i2;

                    if (firstDestroyed < n / 2) //when odd and firstDestroy == n/2, then increment is 0
                        increment = 0.5f;
                    else increment = -0.5f;

                    sphere.transform.position = new Vector3(arr10[firstDestroyed] + increment, n / 2 + 1, n / 2 - 0.5f);
                }
            }
            else
            {
                for (int i2 = 0; i2 < n; i2++)
                {
                    arr10[i2] = -(n - 1) / 2 + i2;
                }

                if (firstDestroyed < n / 2)
                    increment = 0.5f;
                else if (firstDestroyed > n / 2)
                    increment = -0.5f;

                sphere.transform.position = new Vector3(arr10[firstDestroyed], n / 2 + 1, (float)(n / 2));
            }
            var sphereRenderer = sphere.GetComponent<Renderer>();
            sphereRenderer.material.SetColor("_Color", Color.red);

            int temp = Random.Range(0, n);
            if (temp != n - 1)
            {
                GameObject tempcollider = Instantiate(arr[n, temp + 1]);
                tempcollider.transform.rotation = Quaternion.Euler(0, 90, 0);
                tempcollider.transform.position = new Vector3(tempcollider.transform.position.x + 0.5f, tempcollider.transform.position.y + 0.5f, tempcollider.transform.position.z + 0.5f);
                tempcollider.name = "tempcollider";
                tempcollider.GetComponent<Renderer>().GetComponent<MeshRenderer>().enabled = false;
                tempcollider.transform.SetParent(arrParent.transform);
            }
            if (temp != 0)
            {
                GameObject tempcollider2 = Instantiate(arr[n, temp - 1]);
                tempcollider2.transform.rotation = Quaternion.Euler(0, 90, 0);
                tempcollider2.transform.position = new Vector3(tempcollider2.transform.position.x + 0.5f, tempcollider2.transform.position.y + 0.5f, tempcollider2.transform.position.z - 0.5f);
                tempcollider2.name = "tempcollider2";
                tempcollider2.GetComponent<Renderer>().GetComponent<MeshRenderer>().enabled = false;
                tempcollider2.transform.SetParent(arrParent.transform);
            }
            Destroy(arr[n, temp]);
            Destroy(arrf[0, temp]);
            int temp2 = Random.Range(0, n);
            if (temp2 == 0) //might create a method that takes on the array and random int
            {
                GameObject temp2collider = Instantiate(arrl[n, temp2 + 1]);
                temp2collider.transform.rotation = Quaternion.Euler(0, 0, 90);
                temp2collider.transform.position = new Vector3(arrl[n, temp2 + 1].transform.position.x + 0.5f, arrl[n, temp2 + 1].transform.position.y + 0.5f, arrl[n, temp2 + 1].transform.position.z);
                temp2collider.name = "temp2collider";
                temp2collider.GetComponent<Renderer>().GetComponent<MeshRenderer>().enabled = false;

                GameObject temp2collider2 = Instantiate(temp2collider);
                temp2collider2.name = "temp2collider2";
                temp2collider2.transform.position = new Vector3(temp2collider2.transform.position.x, temp2collider2.transform.position.y + 1, temp2collider2.transform.position.z);
                temp2collider2.transform.SetParent(arrfParent.transform);
                temp2collider.transform.SetParent(arrfParent.transform);
            }
            else if (temp2 == n - 1)
            {
                GameObject temp2collider2 = Instantiate(arrl[n, temp2 - 1]);
                temp2collider2.transform.rotation = Quaternion.Euler(0, 0, 90);
                temp2collider2.transform.position = new Vector3(arrl[n, temp2 - 1].transform.position.x + 0.5f, arrl[n, temp2 - 1].transform.position.y - 0.5f, arrl[n, temp2 - 1].transform.position.z);
                temp2collider2.name = "temp2collider2";
                temp2collider2.GetComponent<Renderer>().GetComponent<MeshRenderer>().enabled = false;

                GameObject temp2collider = Instantiate(temp2collider2);
                temp2collider.name = "temp2collider";
                temp2collider.transform.position = new Vector3(temp2collider.transform.position.x, temp2collider.transform.position.y - 1, temp2collider.transform.position.z);
                temp2collider.transform.SetParent(arrfParent.transform);
                temp2collider2.transform.SetParent(arrfParent.transform);
            }
            else
            {
                GameObject temp2collider = Instantiate(arrl[n, temp2 + 1]);
                temp2collider.transform.rotation = Quaternion.Euler(0, 0, 90);
                temp2collider.transform.position = new Vector3(arrl[n, temp2 + 1].transform.position.x + 0.5f, arrl[n, temp2 + 1].transform.position.y + 0.5f, arrl[n, temp2 + 1].transform.position.z);
                temp2collider.name = "temp2collider";
                temp2collider.GetComponent<Renderer>().GetComponent<MeshRenderer>().enabled = false;

                GameObject temp2collider2 = Instantiate(arrl[n, temp2 - 1]);
                temp2collider2.transform.rotation = Quaternion.Euler(0, 0, 90);
                temp2collider2.transform.position = new Vector3(arrl[n, temp2 - 1].transform.position.x + 0.5f, arrl[n, temp2 - 1].transform.position.y - 0.5f, arrl[n, temp2 - 1].transform.position.z);
                temp2collider2.name = "temp2collider2";
                temp2collider2.GetComponent<Renderer>().GetComponent<MeshRenderer>().enabled = false;
                temp2collider2.transform.SetParent(arrfParent.transform);
                temp2collider.transform.SetParent(arrfParent.transform);
            }
            Destroy(arr2f[n, temp2]);
            Destroy(arrl[n, temp2]);
            int temp3 = Random.Range(0, n);
            if (temp3 == 0) //perhaps instead of repeating all this code i just create variables for each position.x, y, and z that have different values depending on temp3
            {
                GameObject temp3collider = Instantiate(arrl[n, 1]);
                temp3collider.transform.rotation = Quaternion.Euler(0, 0, 90);
                temp3collider.transform.position = new Vector3(arrl[0, 1].transform.position.x - 0.5f, arrl[0, 1].transform.position.y - 0.5f, arrl[0, 1].transform.position.z);
                temp3collider.name = "temp3collider";
                temp3collider.GetComponent<Renderer>().GetComponent<MeshRenderer>().enabled = false;

                GameObject temp3collider2 = Instantiate(temp3collider);
                temp3collider2.name = "temp3collider2";
                temp3collider2.transform.position = new Vector3(temp3collider.transform.position.x, temp3collider.transform.position.y + 1, temp3collider.transform.position.z);
                temp3collider2.transform.SetParent(arrlParent.transform);
                temp3collider.transform.SetParent(arrlParent.transform);
            }
            else if (temp3 == n - 1)
            {
                GameObject temp3collider = Instantiate(arrl[n, temp3 - 1]);
                temp3collider.transform.rotation = Quaternion.Euler(0, 0, 90);
                temp3collider.transform.position = new Vector3(arrl[0, temp3 - 1].transform.position.x - 0.5f, arrl[0, temp3 - 1].transform.position.y - 0.5f, arrl[0, temp3 - 1].transform.position.z);
                temp3collider.name = "temp3collider";
                temp3collider.GetComponent<Renderer>().GetComponent<MeshRenderer>().enabled = false;

                GameObject temp3collider2 = Instantiate(temp3collider);
                temp3collider2.name = "temp3collider2";
                temp3collider2.transform.position = new Vector3(temp3collider.transform.position.x, temp3collider.transform.position.y - 1, temp3collider.transform.position.z);
                temp3collider2.transform.SetParent(arrlParent.transform);
                temp3collider.transform.SetParent(arrlParent.transform);
            }
            else
            {
                GameObject temp3collider = Instantiate(arrl[n, temp3 - 1]);
                temp3collider.transform.rotation = Quaternion.Euler(0, 0, 90);
                temp3collider.transform.position = new Vector3(arrl[0, temp3 - 1].transform.position.x - 0.5f, arrl[0, temp3 - 1].transform.position.y - 0.5f, arrl[0, temp3 - 1].transform.position.z);
                temp3collider.name = "temp3collider";
                temp3collider.GetComponent<Renderer>().GetComponent<MeshRenderer>().enabled = false;
                temp3collider.transform.SetParent(arrlParent.transform);

                GameObject temp3collider2 = Instantiate(temp3collider);
                temp3collider2.name = "temp3collider2";
                temp3collider2.transform.position = new Vector3(temp3collider.transform.position.x, temp3collider.transform.position.y - 1, temp3collider.transform.position.z);
                temp3collider2.transform.SetParent(arrlParent.transform);
                temp3collider.transform.SetParent(arrlParent.transform);
            }
            Destroy(arrl[0, temp3]);
            Destroy(arr2back[n, n - temp3 - 1]);
            int temp4 = Random.Range(0, n);
            if (temp4 == 0)
            {
                GameObject temp4collider = Instantiate(arrR[0, 1]);
                temp4collider.transform.rotation = Quaternion.Euler(0, 0, 90);
                temp4collider.transform.position = new Vector3(arrR[0, 1].transform.position.x - 0.5f, arrR[0, 1].transform.position.y - 0.5f, arrR[0, 1].transform.position.z);
                temp4collider.name = "temp4collider";
                temp4collider.GetComponent<Renderer>().GetComponent<MeshRenderer>().enabled = false;

                GameObject temp4collider2 = Instantiate(temp4collider);
                temp4collider2.name = "temp4collider2";
                temp4collider2.transform.position = new Vector3(temp4collider.transform.position.x, temp4collider.transform.position.y - 1, temp4collider.transform.position.z);
                temp4collider2.transform.SetParent(arrbackParent.transform);
                temp4collider.transform.SetParent(arrbackParent.transform);
            }
            else if (temp4 == n - 1)
            {
                GameObject temp4collider = Instantiate(arrR[0, temp4 - 1]);
                temp4collider.transform.rotation = Quaternion.Euler(0, 0, 90);
                temp4collider.transform.position = new Vector3(arrR[0, temp4 - 1].transform.position.x - 0.5f, arrR[0, temp4 - 1].transform.position.y + 0.5f, arrR[0, temp4 - 1].transform.position.z);
                temp4collider.name = "temp4collider";
                temp4collider.GetComponent<Renderer>().GetComponent<MeshRenderer>().enabled = false;

                GameObject temp4collider2 = Instantiate(temp4collider);
                temp4collider2.name = "temp4collider2";
                temp4collider2.transform.position = new Vector3(temp4collider.transform.position.x, temp4collider.transform.position.y + 1, temp4collider.transform.position.z);
                temp4collider2.transform.SetParent(arrbackParent.transform);
                temp4collider.transform.SetParent(arrbackParent.transform);
            }
            else
            {
                GameObject temp4collider = Instantiate(arrR[0, temp4 - 1]);
                temp4collider.transform.rotation = Quaternion.Euler(0, 0, 90);
                temp4collider.transform.position = new Vector3(arrR[0, temp4 - 1].transform.position.x - 0.5f, arrR[0, temp4 - 1].transform.position.y + 0.5f, arrR[0, temp4 - 1].transform.position.z);
                temp4collider.name = "temp4collider";
                temp4collider.GetComponent<Renderer>().GetComponent<MeshRenderer>().enabled = false;

                GameObject temp4collider2 = Instantiate(temp4collider);
                temp4collider2.name = "temp4collider2";
                temp4collider2.transform.position = new Vector3(temp4collider.transform.position.x, temp4collider.transform.position.y + 1, temp4collider.transform.position.z);
                temp4collider2.transform.SetParent(arrbackParent.transform);
                temp4collider.transform.SetParent(arrbackParent.transform);
            }
            Destroy(arr2back[0, temp4]);
            Destroy(arrR[0, temp4]);
            int temp5 = Random.Range(0, n);
            if (temp5 == 0)
            {
                GameObject temp5collider = Instantiate(arr2R[0, 1]);
                temp5collider.transform.rotation = Quaternion.Euler(0, 90, 0);
                temp5collider.transform.position = new Vector3(arr2R[0, 1].transform.position.x - 0.5f, arr2R[0, 1].transform.position.y - 0.5f, arr2R[0, 1].transform.position.z);
                temp5collider.name = "temp5collider";
                temp5collider.GetComponent<Renderer>().GetComponent<MeshRenderer>().enabled = false;
                temp5collider.transform.SetParent(arrRParent.transform);

                GameObject temp5collider2 = Instantiate(temp5collider);
                temp5collider2.name = "temp5collider2";
                temp5collider2.transform.position = new Vector3(temp5collider.transform.position.x - 1, temp5collider.transform.position.y, temp5collider.transform.position.z);
                temp5collider2.transform.SetParent(arrRParent.transform);
            }
            else if (temp5 == n - 1)
            {
                GameObject temp5collider = Instantiate(arr2R[0, temp5 - 1]);
                temp5collider.transform.rotation = Quaternion.Euler(0, 90, 0);
                temp5collider.transform.position = new Vector3(arr2R[0, temp5 - 1].transform.position.x + 0.5f, arr2R[0, temp5 - 1].transform.position.y - 0.5f, arr2R[0, temp5 - 1].transform.position.z);
                temp5collider.name = "temp5collider";
                temp5collider.GetComponent<Renderer>().GetComponent<MeshRenderer>().enabled = false;
                temp5collider.transform.SetParent(arrRParent.transform);

                GameObject temp5collider2 = Instantiate(temp5collider);
                temp5collider2.name = "temp5collider2";
                temp5collider2.transform.position = new Vector3(temp5collider.transform.position.x + 1, temp5collider.transform.position.y, temp5collider.transform.position.z);
                temp5collider2.transform.SetParent(arrRParent.transform);
            }
            else
            {
                GameObject temp5collider = Instantiate(arr2R[0, temp5 - 1]);
                temp5collider.transform.rotation = Quaternion.Euler(0, 90, 0);
                temp5collider.transform.position = new Vector3(arr2R[0, temp5 - 1].transform.position.x + 0.5f, arr2R[0, temp5 - 1].transform.position.y - 0.5f, arr2R[0, temp5 - 1].transform.position.z);
                temp5collider.name = "temp5collider";
                temp5collider.GetComponent<Renderer>().GetComponent<MeshRenderer>().enabled = false;
                temp5collider.transform.SetParent(arrRParent.transform);

                GameObject temp5collider2 = Instantiate(temp5collider);
                temp5collider2.name = "temp5collider2";
                temp5collider2.transform.position = new Vector3(temp5collider.transform.position.x + 1, temp5collider.transform.position.y, temp5collider.transform.position.z);
                temp5collider2.transform.SetParent(arrRParent.transform);
            }
            Destroy(arr2R[0, temp5]);
            Destroy(arr2b[n, temp5]);

            //sphere.AddComponent<BottomBehavior>();

            cube.AddComponent<CameraBehavior>();
        }

        gameRan = true;
    }

    void FixedUpdate()
    {
        if (game)
        {
            Rigidbody rb = sphere.GetComponent<Rigidbody>();
            if (sphere.transform.position.y > n / 2)
            {
                rb.AddForce(Input.acceleration.x * 3, 0, Input.acceleration.y * 3);
            }
        }
    }
}

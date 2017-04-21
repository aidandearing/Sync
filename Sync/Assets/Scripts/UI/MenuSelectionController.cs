using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuSelectionController : MonoBehaviour
{
    public static float inputPaddingFactor = 0.25f;

    [Header("References")]
    public MenuController menu;
    public RectTransform rect;
    public Image[] images;
    public RawImage[] splats;
    new public FreeOrbit camera;
    public FreeOrbit cameraPrefab;

    [Header("Game Objects")]
    public GameObject[] gameObjects;

    [Header("Colours")]
    public Color[] colours;
    public Color[] coloursSplats;

    [Header("Players")]
    public GameObject[] players;
    public int playersCount = 0;

    public int focus;
    public int focusLast;

    public float transitionDuration = 0.25f;
    public float transitionCurrent = 0.0f;

    public bool isGameScene = false;
    public Vector2 hidingPosition = new Vector2(0, -2);
    public bool isHidden = false;
    public bool isHideTransitioning = false;
    public float hidingDuration = 0.25f;
    public float hidingCurrent = 0.0f;

    public void Start()
    {
        focus = 0;
        focusLast = -1;

        for (int i = 0; i < images.Length; i++)
        {
            images[i].color = colours[i * 2];
            splats[i].color = coloursSplats[i * 2];
        }

        string[] joystickNames = Input.GetJoystickNames();

        players = new GameObject[Input.GetJoystickNames().Length - 1];

        menu.enabled = false;
    }

    public void Update()
    {
        // For Reference -----------------------------------------
        //images[focus].color = colours[(focus * 2) + 1];

        // Only need to update all the colour logic if the menu is not currently hidden
        if (!isHidden)
        {
            // As well the colour only needs to be changed if the current focus is not the last (a new menu item has been selected)
            if (focus != focusLast)
            {
                // Determine how far into the transition we are
                float evaluate = Mathf.Clamp(transitionCurrent / transitionDuration, 0, 1);

                // Make sure focus is not set to the default no position value
                if (focus != -1)
                {
                    images[focus].color = Color.Lerp(colours[(focus * 2)], colours[(focus * 2) + 1], evaluate);
                    splats[focus].color = Color.Lerp(coloursSplats[(focus * 2)], coloursSplats[(focus * 2) + 1], evaluate);
                }
                // As well make sure the last focus item is not the same
                if (focusLast != -1)
                {
                    images[focusLast].color = Color.Lerp(colours[(focusLast * 2) + 1], colours[(focusLast * 2)], evaluate);
                    splats[focusLast].color = Color.Lerp(coloursSplats[(focusLast * 2) + 1], coloursSplats[(focusLast * 2)], evaluate);
                }

                // So long as we are less than the transition duration we should increment transition current
                if (transitionCurrent < transitionDuration)
                    transitionCurrent += Time.deltaTime;
                // Otherwise it is a good time to assign the current focus to the old focus, which makes sure we escape this whole branch
                else
                {
                    focusLast = focus;
                    images[focus].color = colours[(focus * 2) + 1];
                    splats[focus].color = coloursSplats[(focus * 2) + 1];
                }
            }// focus != focusLast
            // If focus is equal to focus last we are currently selecting a menu item, and not transitioning to a new one
            else
            {
                // All menu items result in the creation of a controller prefab, and the assignment of its player index
                // This determines if a load is not currently being evaluated
                if (playersCount <= Input.GetJoystickNames().Length - 2)
                {
                    // If the player presses A
                    if (Input.GetButton(Literals.Strings.Input.Standard.Interact + "1"))
                    {
                        // If that player has already been selected
                        if (players.Contains(gameObjects[focus]))
                        {
                            if (playersCount == Input.GetJoystickNames().Length - 2)
                            {
                                // If so hide the menu
                                if (!isHideTransitioning)
                                {
                                    hidingCurrent = 0;
                                    isHideTransitioning = true;

                                    // And set up the players
                                    for (int i = 0; i < players.Length; i++)
                                    {
                                        players[i] = Instantiate(players[i], Vector3.zero, new Quaternion());
                                        players[i].transform.position = new Vector3((i - (players.Length / 2.0f)) * 5.0f, 0.5f, 0.0f);

                                        if (i == 0)
                                        {
                                            camera.target = players[i].transform;
                                            Camera cam = camera.GetComponent<Camera>();
                                            cam.rect = new Rect(i / (float)players.Length, 0, (float)players.Length, 1);
                                            PlayerController player = players[i].GetComponent<PlayerController>();
                                            player.camera = cam;
                                            player.cameraRayQuery = cam.GetComponent<RayQuery>();
                                            player.Player = i + 1;
                                        }
                                        else
                                        {
                                            FreeOrbit camOrbit = Instantiate(cameraPrefab, Vector3.zero, new Quaternion());
                                            camOrbit.target = players[i].transform;
                                            camOrbit.GetComponent<Camera>().rect = new Rect(i / (float)players.Length, 0, (float)players.Length, 1);
                                            Camera cam = camera.GetComponent<Camera>();
                                            cam.rect = new Rect(i / (float)players.Length, 0, (float)players.Length, 1);
                                            PlayerController player = players[i].GetComponent<PlayerController>();
                                            player.camera = cam;
                                            player.cameraRayQuery = cam.GetComponent<RayQuery>();
                                            player.Player = i + 1;
                                        }
                                    }
                                }
                            }
                        }
                        // Otherwise assign the player
                        else
                        {
                            //load = SceneManager.LoadSceneAsync(scenes[focus]);
                            //load.allowSceneActivation = true;
                            players[0] = gameObjects[focus];
                            playersCount++;
                        }
                    }//Input.GetButton("Submit")

                    // If the player presses A
                    if (Input.GetButton(Literals.Strings.Input.Standard.Interact + "2"))
                    {
                        // If that player has already been selected
                        if (players.Contains(gameObjects[focus]))
                        {
                            if (playersCount >= Input.GetJoystickNames().Length - 2)
                            {
                                // If so hide the menu
                                if (!isHideTransitioning)
                                {
                                    hidingCurrent = 0;
                                    isHideTransitioning = true;
                                    menu.enabled = true;

                                    // And set up the players
                                    for (int i = 0; i < players.Length; i++)
                                    {
                                        players[i] = Instantiate(players[i], Vector3.zero, new Quaternion());
                                        players[i].transform.position = new Vector3((i - (players.Length / 2.0f)) * 5.0f, 0.5f, 0.0f);

                                        if (i == 0)
                                        {
                                            camera.target = players[i].transform;
                                            Camera cam = camera.GetComponent<Camera>();
                                            cam.rect = new Rect(i / (float)players.Length, 0, (float)players.Length, 1);
                                            PlayerController player = players[i].GetComponent<PlayerController>();
                                            player.camera = cam;
                                            player.cameraRayQuery = cam.GetComponent<RayQuery>();
                                            player.Player = i + 1;
                                        }
                                        else
                                        {
                                            FreeOrbit camOrbit = Instantiate(cameraPrefab, Vector3.zero, new Quaternion());
                                            camOrbit.target = players[i].transform;
                                            camOrbit.GetComponent<Camera>().rect = new Rect(i / (float)players.Length, 0, (float)players.Length, 1);
                                            Camera cam = camera.GetComponent<Camera>();
                                            cam.rect = new Rect(i / (float)players.Length, 0, (float)players.Length, 1);
                                            PlayerController player = players[i].GetComponent<PlayerController>();
                                            player.camera = cam;
                                            player.cameraRayQuery = cam.GetComponent<RayQuery>();
                                            player.Player = i + 1;
                                        }
                                    }
                                }
                            }
                        }
                        // Otherwise assign the player
                        else
                        {
                            //load = SceneManager.LoadSceneAsync(scenes[focus]);
                            //load.allowSceneActivation = true;
                            players[1] = gameObjects[focus];
                            playersCount++;
                        }
                    }//Input.GetButton("Submit")
                }//load == null

                // If we are not transitioning then we should check to see if a transition is desired
                if (transitionCurrent > transitionDuration)
                {
                    // Get Input, find out if a horizontal transition across title regions should be done
                    float input = Input.GetAxis(Literals.Strings.Input.Controller.StickLeftHorizontal);

                    if (input == 0)
                        Input.GetAxis(Literals.Strings.Input.Controller.PadHorizontal);

                    // If Input is positive a focus transition to the right should be made
                    if (input > 0)
                    {
                        focus = (focus + 1) % images.Length;

                        transitionCurrent -= transitionDuration;
                    }//input > 0
                    // If Input is negative a focus transition to the left should be made
                    else if (input < 0)
                    {
                        focus--;
                        if (focus < 0)
                            focus += images.Length;

                        transitionCurrent -= transitionDuration;
                    }//input < 0
                }//transitionCurrent > transitionDuration
            }//focus != focusLast
        }//!isHidden
        else
        {

        }

        // Whenever we are hide transitioning a bunch of special logic wants to be handled
        if (isHideTransitioning)
        {
            // Default start and end to the position for onscreen
            Vector2 start = Vector2.zero;
            Vector2 end = Vector2.zero;

            // If we are currently hidden we should tell the transition that we are starting at the hiding position, and leave end at the screen, as that is where we would like to end up
            if (isHidden)
            {
                start = hidingPosition;
            }
            // If we are currently visible we should tell the transition that we are ending at the hiding position, and leave the start at the screen, as that is where we currently are
            else
            {
                end = hidingPosition;
            }

            // Move along start to end by hiding time
            rect.anchoredPosition = Vector2.Lerp(start, end, hidingCurrent / hidingDuration);

            // Increment hiding time
            hidingCurrent += Time.deltaTime;

            // End the transition if done
            if (hidingCurrent > hidingDuration)
            {
                isHidden = !isHidden;
                isHideTransitioning = false;

                // For posterity, ensure the rect ends where we want it to be
                rect.anchoredPosition = end;
            }
        }
    }
}

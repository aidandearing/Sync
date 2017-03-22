using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public static float inputPaddingFactor = 0.25f;

    [Header("References")]
    public RectTransform rect;
    public Image[] images;
    public RawImage[] splats;

    [Header("Scenes")]
    public string[] scenes;

    [Header("Colours")]
    public Color[] colours;
    public Color[] coloursSplats;

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

    private AsyncOperation load;

    public void Start()
    {
        focus = 0;
        focusLast = -1;

        for (int i = 0; i < images.Length; i++)
        {
            images[i].color = colours[i * 2];
            splats[i].color = coloursSplats[i * 2];
        }
    }

    public void Update()
    {
        // For Reference -----------------------------------------
        //images[focus].color = colours[(focus * 2) + 1];

        if (!isHidden)
        {
            if (focus != focusLast)
            {
                float evaluate = Mathf.Clamp(transitionCurrent / transitionDuration, 0, 1);

                if (focus != -1)
                {
                    images[focus].color = Color.Lerp(colours[(focus * 2)], colours[(focus * 2) + 1], evaluate);
                    splats[focus].color = Color.Lerp(coloursSplats[(focus * 2)], coloursSplats[(focus * 2) + 1], evaluate);
                }
                if (focusLast != -1)
                {
                    images[focusLast].color = Color.Lerp(colours[(focusLast * 2) + 1], colours[(focusLast * 2)], evaluate);
                    splats[focusLast].color = Color.Lerp(coloursSplats[(focusLast * 2) + 1], coloursSplats[(focusLast * 2)], evaluate);
                }

                if (transitionCurrent < transitionDuration)
                    transitionCurrent += Time.deltaTime;
                else
                    focusLast = focus;
            }
            else
            {
                images[focus].color = colours[(focus * 2) + 1];
                splats[focus].color = coloursSplats[(focus * 2) + 1];

                if (load == null)
                {
                    if (Input.GetButton(Literals.Strings.Input.Controller.ButtonA))
                    {
                        if (SceneManager.GetActiveScene().name == scenes[focus])
                        {
                            if (!isHideTransitioning)
                            {
                                hidingCurrent = 0;
                                isHideTransitioning = true;
                            }
                        }
                        else
                        {
                            load = SceneManager.LoadSceneAsync(scenes[focus]);
                            load.allowSceneActivation = true;
                        }
                    }//Input.GetButton("Submit")
                }//load == null

                if (transitionCurrent > transitionDuration)
                {
                    // Get Input, find out if a horizontal transition across title regions should be done
                    float input = Input.GetAxis(Literals.Strings.Input.Controller.StickLeftHorizontal);

                    if (input == 0)
                        input = Input.GetAxis(Literals.Strings.Input.Controller.PadHorizontal);

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

        if (isHideTransitioning)
        {
            Vector2 start = Vector2.zero;
            Vector2 end = Vector2.zero;

            if (isHidden)
            {
                start = hidingPosition;
            }
            else
            {
                end = hidingPosition;
            }

            rect.anchoredPosition = Vector2.Lerp(start, end, hidingCurrent / hidingDuration);

            hidingCurrent += Time.deltaTime;

            if (hidingCurrent > hidingDuration)
            {
                isHidden = !isHidden;
                isHideTransitioning = false;

                rect.anchoredPosition = end;
            }
        }
        else
        {
            if ((Input.GetButton(Literals.Strings.Input.Controller.ButtonB) && !isGameScene) || Input.GetButton(Literals.Strings.Input.Controller.ButtonMenu))
            {
                hidingCurrent = 0;
                isHideTransitioning = true;
            }// CANCEL
        }
    }
}

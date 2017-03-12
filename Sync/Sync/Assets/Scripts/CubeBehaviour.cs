using UnityEngine;
using System.Collections;

public class CubeBehaviour : MonoBehaviour
{
    public float RearrangeTimeMin = 15;
    public float RearrangeTimeMax = 60;

    private Vector3 startPos;
    private Vector3 goalPos;

    private bool isStarted;
    private bool isDone;

    private float timeElapsed;
    private float rearrangeTimer;

    // Use this for initialization
    void Start()
    {
        startPos = transform.position;
        isStarted = false;
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;

        if (!isDone)
        {
            if (OrbSynchronism.isStarting)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(startPos.x, startPos.y + (100 * OrbSynchronism.startPercent) * (Time.time % 1.0f), startPos.z), 0.01f);
            }
            else
            {
                if (!isStarted)
                {
                    isStarted = true;
                    Rearrange();
                }

                if ((transform.position - goalPos).magnitude >= 0.01f)
                {
                    transform.position = Vector3.Lerp(transform.position, goalPos, 0.01f);
                }
                else
                {
                    transform.position = goalPos;
                    isDone = true;
                }
            }
        }
        else
        {
            if (timeElapsed >= rearrangeTimer)
            {
                Rearrange();
                timeElapsed -= rearrangeTimer;
            }
        }
    }

    void Rearrange()
    {
        float rand = (Random.Range(0, 20) + Random.Range(0, 20) + Random.Range(0, 20)) / 3;
        float y = -70 + Mathf.Round(rand / 10) * 10;
        goalPos = new Vector3(transform.position.x, y, transform.position.z);

        isDone = false;
        rearrangeTimer = Random.Range(RearrangeTimeMin, RearrangeTimeMax);
    }
}

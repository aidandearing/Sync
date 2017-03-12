using UnityEngine;
using System.Collections;

public class TowerBehaviour : MonoBehaviour
{

    public float RearrangeTimeMin = 15;
    public float RearrangeTimeMax = 60;

    private Vector3 goalPos;

    private bool isDone;

    private float timeElapsed;
    private float rearrangeTimer;

    // Use this for initialization
    void Start()
    {
        Rearrange();
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;

        if (!isDone)
        {
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
        float rand = Random.Range(0, 100);
        float y = Mathf.Round(rand / 10) * 10;
        goalPos = new Vector3(transform.position.x, y, transform.position.z);

        isDone = false;
        rearrangeTimer = Random.Range(RearrangeTimeMin, RearrangeTimeMax);
    }
}


using UnityEngine;
using System.Collections;

public class PlayerSpawnBehaviour : MonoBehaviour
{
    public Transform Spawn;
    public float SpawnRadius;
    public GameObject SpawnEffect;
    public float SpawnRate = 1;

    private float timeElapsed;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;


    }

    public void SpawnPlayer(GameObject player)
    {
        if (timeElapsed >= SpawnRate)
        {
            timeElapsed -= SpawnRate;

            GameObject flare = Instantiate(SpawnEffect, Spawn.position + new Vector3(0, 2000, 0), new Quaternion()) as GameObject;
            flare.GetComponent<FlareBehaviour>().SetSpawnable(player);
        }
    }
}

using UnityEngine;
using System.Collections;

public class WorldGen : MonoBehaviour
{
    public GameObject worldPiece;
    public GameObject outerPiece;
    public GameObject upperPiece;

    // Use this for initialization
    void Start()
    {
        float dimWidth = worldPiece.transform.localScale.x;
        float dimDepth = worldPiece.transform.localScale.z;

        for (int x = -25; x < 25; x++)
        {
            for (int z = -25; z < 25; z++)
            {
                Vector3 pos = new Vector3(x * dimWidth, -70, z * dimDepth);
                if (pos.magnitude <= 250.0f && pos.z <= 127)
                    Instantiate(worldPiece, pos, new Quaternion());
            }
        }

        float radius = Random.Range(400, 500);
        float angle = Random.Range(0, Mathf.PI * 2);
        for (int i = 0; i < 1000; i++)
        {
            Vector3 scale = new Vector3(Random.Range(10, 20), Random.Range(10, 100), Random.Range(10, 20));
            Vector3 pos = new Vector3(radius * Mathf.Cos(angle), scale.y / 2, radius * Mathf.Sin(angle));
            GameObject tower = Instantiate(outerPiece, pos, Quaternion.LookRotation(new Vector3(-pos.x, 0, -pos.z))) as GameObject;
            tower.transform.localScale = scale;
            angle = Random.Range(0, Mathf.PI * 2);
            radius = Random.Range(400, 500);
        }

        radius = 300;
        angle = Random.Range(0, Mathf.PI * 2);
        for (int i = 0; i < 100; i++)
        {
            Vector3 scale = new Vector3(Random.Range(10, 20), Random.Range(10, 100), Random.Range(10, 20));
            Vector3 pos = new Vector3(radius * Mathf.Cos(angle), 0, radius * Mathf.Sin(angle));
            GameObject tower = Instantiate(upperPiece, pos, Quaternion.LookRotation(new Vector3(-pos.x, 0, -pos.z))) as GameObject;
            tower.transform.localScale = scale;
            angle = Random.Range(0, Mathf.PI * 2);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

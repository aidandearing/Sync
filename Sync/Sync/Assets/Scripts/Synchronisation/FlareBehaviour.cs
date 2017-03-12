using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/Synchronisation/Flare Behaviour")]
public class FlareBehaviour : MonoBehaviour
{
    public float Velocity = 2000;
    public GameObject Shockwave;

    private GameObject Spawnable;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0, -Velocity, 0) * Time.deltaTime;
        if (transform.position.y <= 50)
        {
            Vector3 pos = transform.position;
            pos.y = 0;
            Destroy(Instantiate(Shockwave, pos, Quaternion.LookRotation(new Vector3(0, 1, 0))), 1);
            Vector3 delta = new Vector3(0, 0, 0) - pos;
            pos = pos + Random.insideUnitSphere * 10;
            pos.y = 0;
            Instantiate(Spawnable, pos, Quaternion.LookRotation(delta));
            Destroy(gameObject);
        }
    }

    public void SetSpawnable(GameObject spawnable)
    {
        Spawnable = spawnable;
    }
}

using UnityEngine;
using System.Collections;

public class TempoReverse : MonoBehaviour
{
    public float lifetime = 1;
    [SerializeField]
    private Renderer distort;
    [SerializeField]
    private Controller parent;

    private float lifeElapsed = 0;

    private Vector3 lastPosition;

    private bool firstUpdate = true;

    // Use this for initialization
    void Start()
    {
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        lifeElapsed += Time.deltaTime;

        float percent = lifeElapsed / lifetime;

        lastPosition = transform.position;

        distort.material.shader = Shader.Find("FX/Glass/Stained BumpDistort");
        distort.material.SetFloat("_BumpAmt", Mathf.Lerp(16, 0, percent));

        if (firstUpdate) FirstUpdate();
    }

    void FirstUpdate()
    {
        Collider[] insiders = Physics.OverlapSphere(transform.position, 5);
        foreach (Collider insider in insiders)
        {
            // Projectile stuff
            ProjectileBehaviour projectile = insider.gameObject.GetComponent<ProjectileBehaviour>();
            if (projectile != null)
            {
                if (projectile.parent.Faction != parent.Faction)
                {
                    projectile.Flip(); 
                }
            }
        }

        firstUpdate = false;
    }

    public void SetParent(Controller par)
    {
        parent = par;
    }
}

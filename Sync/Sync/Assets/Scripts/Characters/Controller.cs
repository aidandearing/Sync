using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Controller : NetworkBehaviour
{
    [SerializeField]
    private string _faction;
    public string Faction { get { return _faction; } set { _faction = value; } }

    // Use this for initialization
    protected virtual void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {

    }
}

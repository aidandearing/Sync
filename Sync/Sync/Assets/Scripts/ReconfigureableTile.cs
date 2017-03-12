using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReconfigureableTile : MonoBehaviour
{
    public GameObject[] configurations;
    public GameObject currentConfiguration;
    public GameObject newConfiguration;

    public bool timerBased = true;
    public float configuration_time = 120;
    [SerializeField]
    private float configuration_timer = 0;

    public float reconfiguration_time = 0.01f;
    public float concealHeight = -1.3f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (currentConfiguration == null)
        {
            NewConfiguration();
            currentConfiguration = newConfiguration;
            currentConfiguration.transform.position = transform.position;
            newConfiguration = null;
        }

        if (newConfiguration != null)
        {
            // We have a new configuration to become.
            // 1- We should move the current configuration into the ground.
            // 2- Then we should destroy it
            // 3- Finally we should make the new configuration the current
            // 4- And we should null the new configuration, as we don't have another new one to go to

            // 1- We should move the current configuration into the ground.
            currentConfiguration.transform.position = Vector3.MoveTowards(currentConfiguration.transform.position, transform.position + new Vector3(0, concealHeight, 0), reconfiguration_time);

            // When it is in the ground
            if (currentConfiguration.transform.position.y <= concealHeight)
            {
                // 2- Then we should destroy it
                Destroy(currentConfiguration);

                // 3- Finally we should make the new configuration the current
                currentConfiguration = newConfiguration;

                // 4- And we should null the new configuration, as we don't have another new one to go to
                newConfiguration = null;
            }
        }
        else
        {
            // We have no new configuration
            // 1- We should move the current configuration out of the ground
            // 2- Then we should update our reconfiguration timer, if we are using that logic

            // 1- We should move the current configuration out of the ground
            currentConfiguration.transform.position = Vector3.MoveTowards(currentConfiguration.transform.position, transform.position, reconfiguration_time);

            // 2- Then we should update our reconfiguration timer, if we are using that logic
            if (timerBased)
            {
                configuration_timer += Time.deltaTime;

                if (configuration_timer >= configuration_time)
                {
                    configuration_timer -= configuration_time;
                    NewConfiguration();
                }
            }
        }
    }

    void NewConfiguration()
    {
        newConfiguration = Instantiate(configurations[Random.Range(0, configurations.Length - 1)], transform.position + new Vector3(0, concealHeight, 0), new Quaternion());
    }
}

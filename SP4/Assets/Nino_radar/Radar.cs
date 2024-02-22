using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    [SerializeField]
    private GameObject pingPrefab;

    [SerializeField]
    private float pingInterval = 10f;

    private float pingTimer;

    private void Start()
    {
        pingTimer = pingInterval;
    }

    // Update is called once per frame
    void Update()
    {
        pingTimer -= Time.deltaTime;

        if (pingTimer <= 0)
        {
            Collider[] radarColliders = Physics.OverlapSphere(Camera.main.transform.position, 10.0f);
            foreach (var collider in radarColliders)
            {
                if (collider.gameObject.CompareTag("Enemy"))
                {
                    Instantiate(pingPrefab, collider.transform.position + new Vector3(0, 1.25f, 0), Quaternion.identity);
                }
            }
            pingTimer = pingInterval;
        }
        
        //Camera update
        Vector3 newPos = Camera.main.transform.position;
        newPos.y = transform.position.y;
        transform.position = newPos;

        transform.rotation = Quaternion.Euler(90.0f, Camera.main.transform.eulerAngles.y, 0.0f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireflyGenerator : MonoBehaviour
{
    public Transform fireflyPrefab;
    public int count;

    void Start()
    {
     for (int i = 0; i < count; i++)
        {
            
           Transform firefly = Instantiate(fireflyPrefab, Random.onUnitSphere * Random.Range(1.0f, 10.0f), Quaternion.identity, transform);
        }   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireflyGenerator : MonoBehaviour
{
    public Transform fireflyPrefab;
    private List<Transform> fireflies;

    private void Awake()
    {
        fireflies = new List<Transform>();
    }

    public void GenerateFireflies(int count)
    {
        for (int i = 0; i < fireflies.Count; ++i)
            Destroy(fireflies[i].gameObject);
        fireflies.Clear();

        for (int i = 0; i < count; i++)
        {
            Transform tempFirefly = Instantiate(fireflyPrefab, new Vector3(), Quaternion.identity, transform);
            tempFirefly.transform.localPosition = Random.onUnitSphere * Random.Range(1.0f, 10.0f);
            fireflies.Add(tempFirefly);
        }
    }
}

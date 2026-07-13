using System.Collections.Generic;
using UnityEngine;

public class OrePileManager : MonoBehaviour
{
    public static OrePileManager Instance;

    [Header("Which item this pile represents")]
    public ItemData oreItem; // should match the ItemData used for "iron ore"

    [Header("Spawning")]
    public GameObject orePrefab;   // the physical ore object (needs a Rigidbody + Collider)
    public Transform dropPoint;    // position above the crate where ore drops from
    public float spawnRadius = 0.3f; // random horizontal spread so they don't all land in one spot
    public float dropForce = 1.5f;   // small random tumble force so they don't land in a perfect stack

    private List<GameObject> pile = new List<GameObject>();

    void Awake()
    {
        Instance = this;
    }

    // Call this once per ore purchased
    public void AddOre()
    {
        if (orePrefab == null || dropPoint == null)
        {
            Debug.LogWarning("OrePileManager: Ore Prefab or Drop Point not assigned.");
            return;
        }

        Vector3 randomOffset = new Vector3(
            Random.Range(-spawnRadius, spawnRadius),
            0f,
            Random.Range(-spawnRadius, spawnRadius));

        GameObject ore = Instantiate(orePrefab, dropPoint.position + randomOffset, Random.rotation);

        Rigidbody rb = ore.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 randomTorque = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)) * dropForce;

            rb.AddTorque(randomTorque, ForceMode.Impulse);
        }

        pile.Add(ore);

        Debug.Log("Ore dropped into pile. Pile size: " + pile.Count);
    }

    // Call this once per ore consumed (e.g. when smelting starts)
    public void RemoveOre()
    {
        if (pile.Count == 0)
        {
            Debug.LogWarning("OrePileManager: tried to remove ore, but pile is already empty.");
            return;
        }

        GameObject last = pile[pile.Count - 1];
        pile.RemoveAt(pile.Count - 1);

        if (last != null)
            Destroy(last);

        Debug.Log("Ore removed from pile. Pile size: " + pile.Count);
    }

    public int PileCount => pile.Count;
}
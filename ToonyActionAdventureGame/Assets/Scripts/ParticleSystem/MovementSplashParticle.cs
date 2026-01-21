using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MovementSplashParticle : MonoBehaviour
{
    [SerializeField] private GameObject splashParticleFx; // Hold Reference for the Prefab 
    [SerializeField] private Transform[] spawnPositions;

    public static MovementSplashParticle instance;

    private void Start()
    {
        instance = this;
    }
    public IEnumerator play()
    {
        //foreach(Transform positions in spawnPositions)
        //{
        //    GameObject prefab = Instantiate(splashParticleFx, positions);
        //    yield return new WaitForSeconds(1f);

        //}

        GameObject prefab = Instantiate(splashParticleFx, spawnPositions[0]);
        //yield return null;
        yield return new WaitForSeconds(0.45f);
        //Destroy(prefab);
    }
}

using System.Collections;
using UnityEngine;


public class PlayerRunningTrail : MonoBehaviour
{
    
    [SerializeField] private GameObject _SplashFX;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float rayDistance = 0.10f;

    private bool isPlaying;

    private void Update()
    {
        if (isPlaying)
            return;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, rayDistance, groundLayer.value))
        {
            Play();
        }
    }

    private void Play()
    {
        isPlaying = true;
        GameObject prefab = Instantiate(_SplashFX, transform.position, Quaternion.identity);
        isPlaying = false;
    }
}

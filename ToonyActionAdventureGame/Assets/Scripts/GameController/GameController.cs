
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    private void Awake()
    {
        instance = this;

    }

    private void Update()
    {
    }
    
}

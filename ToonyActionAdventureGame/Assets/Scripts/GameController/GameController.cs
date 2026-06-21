
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    private void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;

    }

    private void Update()
    {
    }
    
}

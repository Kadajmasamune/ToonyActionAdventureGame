
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    //private Player player;


    private void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //player = FindFirstObjectByType<Player>();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            //player.transform.position = new Vector3(-4.61999989f, 8.06999969f, 67.1800003f);
        }
    }
    
}

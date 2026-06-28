using UnityEngine ;
using System.Collections.Generic;

public class Entity : MonoBehaviour  
{
    [SerializeField] private List<IEntitySystem> systems;

    private void Start()
    {
        foreach (IEntitySystem system in systems)
        {
            system.Init();
        }
    }

    public void OnEnable()
    {
        Ticker.OnTick += Tick;   
    }

    public void OnDisable()
    {
        Ticker.OnTick -= Tick;
    }

    public void Tick()
    {
        foreach (IEntitySystem system in systems)
        {
            system.Update();
        }
    }

}
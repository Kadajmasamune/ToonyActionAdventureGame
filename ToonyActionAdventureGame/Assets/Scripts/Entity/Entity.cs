using UnityEngine ;
using System.Collections.Generic;
public class Entity : MonoBehaviour  
{
    [SerializeField] public List<IEntitySystem> systems;

    private void Start()
    {
        systems = new List<IEntitySystem>();
        foreach (IEntitySystem script in this.gameObject.GetComponents(typeof(IEntitySystem)))
        {
            systems.Add(script);
        }
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
        //Debug.Log(systems.Count);


        foreach (IEntitySystem system in systems)
        {
            system.Tick();
        }
    }

}
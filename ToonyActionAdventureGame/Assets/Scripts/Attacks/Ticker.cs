using UnityEngine;
using System.Collections.Generic;


public class Ticker: MonoBehaviour
{
	private const float tickRate = 1f/60f;  //How much a tick costs
	private float accumulator = 0f; //Wallet  

	private int currentTick = 0; //Total lifetime Ticks 
    
    public int CurrentTick { get { return currentTick; } private set { currentTick = value; }  }
    public static Ticker instance;
     
    private void Start()
    {
        instance = this; 
    }


    private void Update()
    {
        accumulator += Time.deltaTime; //You're paid over frames.
        while (canTick())
        {
            //Debug.Log($"Accumulator : {accumulator} , Ticks : {currentTick}");
            accumulator -= tickRate;
            Tick();

        }

    }
    
    private int Tick()
    {
        return currentTick++;
    }



    
	private bool canTick ()  
	{
		return accumulator >= tickRate; //If we've accumulated enough time, tick 
	}


}


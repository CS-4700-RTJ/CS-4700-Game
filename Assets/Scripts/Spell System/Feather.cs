using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feather : MonoBehaviour, ICastable
{
    public float featherDuration = 10;
    public int midAirJumps = 2;
    

    public void Cast(Vector3 castDirection, Transform casterTransform)
    {
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        
        player.ApplyFeather(featherDuration, midAirJumps);
        
        Destroy(gameObject);
    }
}

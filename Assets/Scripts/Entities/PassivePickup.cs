using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassivePickup : MonoBehaviour
{
    [Tooltip("The Passive Pickup Stats will be ADDED to the player.")]
    public PassiveStats passiveStats;

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().Pickup(passiveStats);            
            GameObject.Destroy(gameObject);
        }
    }

    [System.Serializable]    
    public class PassiveStats
    {
        public string passiveName;
        public Sprite passiveImage;
        public EntityStats stats;
    }
}

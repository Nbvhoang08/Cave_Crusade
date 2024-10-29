using System;
using UnityEngine;

namespace Script
{
    public class Player_Controller : MonoBehaviour
    {
        public Player player;

        private void Awake()
        {
            if (player == null)
            {
                player = player.GetComponent<Player>();
            }
        }
        
        
        
        
    }
}
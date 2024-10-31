using System;
using UnityEngine;

namespace Script
{
    public class SelfDestruct : MonoBehaviour
    {
        public float DestructTime = 1.5f;
        private float DestructTimer;

        private void Start()
        {
            DestructTimer = DestructTime;
        }

        void Update()
        {
            DestructTimer -= Time.deltaTime;
            if (DestructTimer <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Script
{
    public enum EnemyState
    {
        Idle,
        Atk,
        Def, 
        PrepareAttack
    }
    
    public class Enemies : MonoBehaviour
    {
        public int hp;
        public int maxHp;
        public bool isDead => hp <= 0;
        public EnemyState currentState ;
        public List<EnemyState> stateSequence = new List<EnemyState> {  };
        private int currentStateIndex = 0;

        public void Start()
        {
            hp = maxHp;
        }

        public void Update()
        {
            // while (true)
            // {
            //     switch (currentState)
            //     {
            //         case EnemyState.Idle:
            //             HandleIdleState();
            //             break;
            //         case EnemyState.Def:
            //             HandleDefendState();
            //             break;
            //         case EnemyState.PrepareAttack: 
            //             HandlePrepareAttackState();
            //             break;
            //         case EnemyState.Atk:
            //             HandleAttackState();
            //             break;
            //     }
            //
            // }

           
        }
        
        public void TakeDamage(int damage)
        {
            //hp -= damage;
            
        }

        
        public void NextState()
        {
            currentStateIndex++;
            if (currentStateIndex >= stateSequence.Count)
            {
                currentStateIndex = 0; // Quay lại trạng thái đầu tiên
            }
            currentState= stateSequence[currentStateIndex];
        }
      

        public void HandleIdleState()
        {
            Debug.Log("Enemy is Idle");
        }

        public void HandleDefendState()
        {
            // Logic cho trạng thái Defend
            Debug.Log("Enemy is Defending");
    
        }

        public void HandlePrepareAttackState()
        {
            // Logic cho trạng thái chuẩn bị tấn công
            Debug.Log("Enemy is preparing to Attack");
           
        }

        public void HandleAttackState()
        {
            Debug.Log("Enemy is Attacking");
            currentState = EnemyState.Idle;
        }

    }
}
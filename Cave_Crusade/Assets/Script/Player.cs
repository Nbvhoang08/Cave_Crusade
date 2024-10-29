using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Script
{

    public class Player : Singleton<Player>
    {
        public float moveSpeed = 5f;
        public float attackRange = 2f;
        public float healAmount = 50f;

        [SerializeField] private PlayerState currentState = PlayerState.Idle;
        [SerializeField] private bool isAttacking = false;
        [SerializeField] private bool isDefending = false;
        [SerializeField] private bool isHealing = false;
        
        [SerializeField] private float moveDistance = 5f; // Khoảng cách di chuyển cố định
        private Vector3 targetPosition;
        
        public Animator anim;
        public String currentAnimName;


        void Start()
        {
            anim = GetComponent<Animator>();
        }

        void Update()
        {
            switch (currentState)
            {
                case PlayerState.Idle:
                    HandleIdleState();
                    break;
                case PlayerState.Walk:
                    HandleWalkState();
                    break;
                case PlayerState.Attack:
                    HandleAttackState();
                    break;
                case PlayerState.Defend:
                    HandleDefendState();
                    break;
                case PlayerState.Heal:
                    HandleHealState();
                    break;
            }
        }

        public void OnMoveButtonClick()
        {
            currentState = PlayerState.Walk;
            targetPosition = transform.position + Vector3.right * moveDistance; // Đặt vị trí đích
        }

        public void OnAttackButtonClick()
        {
            currentState = PlayerState.Attack;
            isAttacking = true;
        }

        public void OnDefendButtonClick()
        {
            currentState = PlayerState.Defend;
            isDefending = true;
        }

        public void OnHealButtonClick()
        {
            currentState = PlayerState.Heal;
            isHealing = true;
        }

        // Các phương thức xử lý trạng thái vẫn giữ nguyên như trước


        void HandleIdleState()
        {
            // Player is standing still
            Debug.Log("Player Idle");
            ChangeAnim("Idle");
        }

        void HandleWalkState()
        {
            // Move the player based on input
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * moveDistance);
            
            // Kiểm tra nếu đã đến vị trí đích thì chuyển về trạng thái Idle
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                currentState = PlayerState.Idle;
                Debug.Log("Player Idle");
                ChangeAnim("Idle");
            }
            else
            {
                ChangeAnim("Walk");
            }
        }

        void HandleAttackState()
        {
            // Perform attack logic
            if (isAttacking)
            {
                // Check for enemies within attack range and apply damage
                Debug.Log("Player ATK");
                // Chuyển về trạng thái Idle sau khi tấn công
                isAttacking = false;
                ChangeAnim("Atk");
                StartCoroutine(ResetState());
            }
        }

        void HandleDefendState()
        {
            // Perform defend logic
            if (isDefending)
            {
                // Reduce incoming damage
                Debug.Log("Player DEF");
                // Chuyển về trạng thái Idle sau khi phòng thủ
                isDefending = false;
                ChangeAnim("Def");
                StartCoroutine(ResetState());
            }
        }

        IEnumerator ResetState()
        {
            yield return new WaitForSeconds(3f);
            currentState = PlayerState.Idle;
        }
        void HandleHealState()
        {
            // Perform heal logic
            if (isHealing)
            {
                // Restore player's health by the heal amount
                Debug.Log("Player HEAL");
                ChangeAnim("Heal");
                // Chuyển về trạng thái Idle sau khi hồi máu
                isHealing = false;
                StartCoroutine(ResetState());
            }
        }
        public void ChangeAnim(string animName)
        {
            if (currentAnimName != animName)
            {
                anim.ResetTrigger(animName);
                currentAnimName = animName;
                anim.SetTrigger(animName);
          
            }
        }
    }
}


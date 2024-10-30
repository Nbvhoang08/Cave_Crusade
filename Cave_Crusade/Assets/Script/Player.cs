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
        public Animator anim;
        public String currentAnimName;
        [SerializeField] private PlayerState currentState = PlayerState.Idle;
        [SerializeField] private bool isAttacking = false;
        [SerializeField] private bool isDefending = false;
        [SerializeField] private bool isHealing = false;
        
        [SerializeField] private float moveDistance = 5f; // Khoảng cách di chuyển cố định
        private Vector3 targetPosition;
        public bool canDo;
        
        

        void Start()
        {
            if (anim == null)
            {
                anim = GetComponent<Animator>();
                Debug.Log(anim);
            }
            
            
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
        // Hàm bắt đầu lượt mới
        


        public void OnMoveButtonClick()
        {
            currentState = PlayerState.Walk;
            targetPosition = transform.position + Vector3.right * moveDistance; // Đặt vị trí đích
            canDo = false;
        }

        public void OnAttackButtonClick()
        {
            currentState = PlayerState.Attack;
            isAttacking = true;
            canDo = false;
        }

        public void OnDefendButtonClick()
        {
            currentState = PlayerState.Defend;
            isDefending = true;
            canDo = false;
        }

        public void OnHealButtonClick()
        {
            currentState = PlayerState.Heal;
            isHealing = true;
            canDo = false;
        }

        // Các phương thức xử lý trạng thái vẫn giữ nguyên như trước


        void HandleIdleState()
        {
            // Player is standing still
            ChangeAnim("Idle");
            canDo = true;
        }

        void HandleWalkState()
        {
            // Move the player based on input
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * moveDistance);
            //Debug.Log(targetPosition);
           
            // Kiểm tra nếu đã đến vị trí đích thì chuyển về trạng thái Idle
            if (Vector2.Distance(transform.position, targetPosition) <= 0.001f)
            {
                currentState = PlayerState.Idle;
               
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
    
                // Chuyển về trạng thái Idle sau khi phòng thủ
                isDefending = false;
                ChangeAnim("Def");
                StartCoroutine(ResetState());
            }
        }

        IEnumerator ResetState()
        {
            yield return new WaitForSeconds(2f);
            currentState = PlayerState.Idle;
            
        }
        void HandleHealState()
        {
            // Perform heal logic
            if (isHealing)
            {
                // Restore player's health by the heal amoun
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


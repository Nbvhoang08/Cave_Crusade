using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        public Animator anim ;
        public String currentAnimName;
        public bool isInvincible;
        public int attackDamage;
        public int attackRange;
        public bool attacking;
        public GameObject PopUP;
        public TMP_Text popUpText;
        public void Start()
        {
            hp = maxHp;
            if (anim == null)
            {
                anim = GetComponent<Animator>();
            }
           
            currentState = stateSequence[currentStateIndex];
        }

        public void Update()
        {
            if (!isDead)
            {
                switch (currentState)
                {
                    case EnemyState.Idle:
                        HandleIdleState();
                        break;
                    case EnemyState.Def:
                        HandleDefendState();
                        break;
                    case EnemyState.PrepareAttack:
                        HandlePrepareAttackState();
                        break;
                    case EnemyState.Atk:
                        HandleAttackState();
                        break;
                }
            }
            else
            {
                Death();
            }
            


        }

        public void TakeDamage(int damage)
        {

            //StartCoroutine(DelayDamage(damage));
            if (!isInvincible)
            {
                hp -= damage;
                
            }
           


        }


       
        private void Death()
        { 
            StartCoroutine(DesSpawn());
        }

        IEnumerator DesSpawn()
        {
            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
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

        public virtual void NextState()
        {
            currentStateIndex++;
            if (currentStateIndex >= stateSequence.Count)
            {
                currentStateIndex = 0; // Quay lại trạng thái đầu tiên
            }
            currentState= stateSequence[currentStateIndex];
        }
      

        public virtual void HandleIdleState()
        {
            ChangeAnim("idle");
            isInvincible = false;
            attacking = false;
        }

        public virtual void HandleDefendState()
        {
           isInvincible = true;
          

        }

        public virtual void HandlePrepareAttackState()
        {
            
            
           
        }

        public virtual void HandleAttackState()
        {
            if (attacking)
            {
                return;
            }
            else
            {
                StartCoroutine(ApplyDamge());
                attacking = true;
                Debug.Log("attacking");
            }
           
            //ApplyDamageToPlayer();
        }

        public virtual void ApplyDamageToPlayer()
        {
  
            Vector2 start = transform.position;
            Vector2 end = start + Vector2.left * attackRange;

         

            RaycastHit2D[] hits = Physics2D.LinecastAll(start, end);
            Debug.Log(hits.Length);
            foreach (RaycastHit2D hit in hits)
            {   
                
                if (hit.collider.CompareTag("Player"))
                {
                    hit.collider.GetComponent<Player>().TakeDamage(attackDamage);
                    //attacking = true; // Đánh dấu đã gây damage
                    Debug.Log(hit.collider.name);
                    break; // Thoát khỏi vòng lặp sau khi gây damage
                }
                else
                {
                    Debug.Log(hit.collider.name);
                }
            }

        }
     
        public IEnumerator ResetState()
        {
            yield return new WaitForSeconds(2f);
            currentState = EnemyState.Idle;
        }
        public IEnumerator ApplyDamge()
        {
            yield return new WaitForSeconds(0.1f);
            ApplyDamageToPlayer();
        }
    }
}
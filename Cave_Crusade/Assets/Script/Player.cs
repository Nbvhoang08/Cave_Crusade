using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

namespace Script
{

    public class Player : Singleton<Player>
    {
        public float moveSpeed = 5f;
        public float attackRange = 2f;
        public int attackDamage= 1 ;
        public float dectectRange;
        public float healAmount = 50f;
        public Animator anim;
        public String currentAnimName;
        public LayerMask enemyLayer;
        [SerializeField] private PlayerState currentState = PlayerState.Idle;
        [SerializeField] private bool isAttacking = false;
        [SerializeField] private bool isDefending = false;
        [SerializeField] private bool isHealing = false;

        [SerializeField] private float moveDistance = 5f; // Khoảng cách di chuyển cố định
        private Vector3 targetPosition;
        public bool canDo;
        public GameObject PopUP;
        public TMP_Text popUpText;
        [SerializeField] private int _hp;

        public int hp
        {
            get { return _hp; }
            set
            {
                // Đảm bảo giá trị hp không vượt quá maxHp và không nhỏ hơn 0
                if (value > maxHp)
                {
                    _hp = maxHp;
                }
                else if (value < 0)
                {
                    _hp = 0;
                }
                else
                {
                    _hp = value;
                }
            }
        }

        public int maxHp;


        void Start()
        {
            if (anim == null)
            {
                anim = GetComponent<Animator>();
                Debug.Log(anim);
            }
            _hp = maxHp;

        }

        void Update()
        {
            Debug.Log(isDefending);
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
            Debug.Log(EnemyInFront());

        }
        // Hàm bắt đầu lượt mới

        public void TakeDamage(int damage)
        {
            StartCoroutine(DelayDamage(damage));
        }

        IEnumerator DelayDamage(int damage)
        {
            yield return new WaitForSeconds(0.5f);
            if (!isDefending)
            {
                hp -= damage;   
                Instantiate(PopUP, transform.position, Quaternion.identity);
                popUpText.text ="- " +damage.ToString("D2");
                
            }
            else
            {
                Instantiate(PopUP, transform.position, Quaternion.identity);
                popUpText.text = "Block";
                SoundManager.Instance.PlayVFXSound(3);
            }
        }
        public void OnMoveButtonClick()
        {
            if (!EnemyInFront())
            {
                currentState = PlayerState.Walk;
                targetPosition = transform.position + Vector3.right * moveDistance; // Đặt vị trí đích
                canDo = false;
            }
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
            isDefending = false;
            isAttacking = false;
            canDo = true;
        }

        void HandleWalkState()
        {
            // Kiểm tra nếu có kẻ địch trước mặt
           
            // Kiểm tra nếu có kẻ địch trước mặt khi đang đứng yên
            if ( EnemyInFront())
            {
                ChangeAnim("Idle");
                return; // Không di chuyển
            }

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

     bool EnemyInFront()
        {
            // Logic để kiểm tra nếu có kẻ địch trước mặt (sử dụng Raycast hoặc các phương pháp khác)
            RaycastHit2D hit = Physics2D.Raycast(transform.position,  Vector2.right, dectectRange, enemyLayer); // Sử dụng Raycast để kiểm tra phía trước
            if (hit.collider != null && hit.collider.CompareTag("Enemy"))
            {
                return true;
            }

            return false;
        }

        void OnDrawGizmos()
        {


            Gizmos.color = Color.red;
            // Vẽ tia ray từ vị trí hiện tại của đối tượng theo hướng Vector2.right
            Gizmos.DrawRay(transform.position, Vector2.right * dectectRange);
            //Gizmos.DrawWireSphere(transform.position, attackRange);
        }

        void HandleAttackState()
        {
            // Perform attack logic
            if (isAttacking)
            {
                // Check for enemies within attack range and apply damage
                StartCoroutine(ApplyDamge());
                SoundManager.Instance.PlayVFXSound(0);
                // Chuyển về trạng thái Idle sau khi tấn công
                isAttacking = false;
                ChangeAnim("Atk");
                StartCoroutine(ResetState());
            }
        }

        void ApplyDamageToEnemies()
        {
            // Logic gây damage cho kẻ địch trong tầm tấn công (chỉ gây damage một lần)
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange);
            foreach (Collider2D enemy in enemies)
            {
                if (enemy.CompareTag("Enemy"))
                {
                    enemy.GetComponent<Enemies>().TakeDamage(attackDamage);
                }
            }


        }

        void HandleDefendState()
            {
                // Perform defend logic
                if (isDefending)
                {
                    ChangeAnim("Def");
                }
            }
        IEnumerator ApplyDamge()
        {
            yield return new WaitForSeconds(0.5f);
            ApplyDamageToEnemies();
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
                    SoundManager.Instance.PlayVFXSound(2);
                    isHealing = false;
                    hp += 1;
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



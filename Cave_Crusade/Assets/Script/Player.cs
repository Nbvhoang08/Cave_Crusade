using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.SceneManagement;

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
        [SerializeField] private bool isAttack = false;
        [SerializeField] private bool isDefending = false;
        [SerializeField] private bool isHealing = false;

        [SerializeField] private float moveDistance = 5f; // Khoảng cách di chuyển cố định
        private Vector3 targetPosition;
        public bool canDo;
        public GameObject PopUP;
        public TMP_Text popUpText;
        [SerializeField] private int _hp;
        private Coroutine resetStateCoroutine;
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

            if (isAttack)
            {
                Debug.Log("atk");
            }

        }
        // Hàm bắt đầu lượt mới

        public void TakeDamage(int damage)
        {
            if (!isDefending)
            {
                hp -= damage;
                GameObject popUp = Instantiate(PopUP, transform.position, Quaternion.identity);
                popUp.GetComponent<TextMeshPro>().text = "- " + damage.ToString("D2");
            
                Debug.Log(damage);

            }
            else
            {
                //Instantiate(PopUP, transform.position, Quaternion.identity);
                //popUpText.text = "Block";
                GameObject popUp = Instantiate(PopUP, transform.position, Quaternion.identity);
                popUp.GetComponent<TextMeshPro>().text = "Block";
                SoundManager.Instance.PlayVFXSound(3);
                Debug.Log("Defend");
          

            }

        }
        
        public void OnMoveButtonClick()
        {
            if (!EnemyInFront() && canDo )
            {
                StopCoroutine("ResetState");
                currentState = PlayerState.Walk;
                targetPosition = transform.position + Vector3.right * moveDistance; // Đặt vị trí đích
                canDo = false;
               
            }
        }

        public void OnAttackButtonClick()
        {
            if(canDo)
            {
                StopCoroutine("ResetState");
                currentState = PlayerState.Attack;
                isAttack = true;
                canDo = false;
                ChangeAnim("Atk");
            }
            

        }

        public void OnDefendButtonClick()
        {
            if(canDo)
            {
                StopCoroutine("ResetState");
                currentState = PlayerState.Defend;
                isDefending = true;
                //canDo = false;
            }
            
           
        }

        public void OnHealButtonClick()
        {
            if(canDo)
            {
                StopCoroutine("ResetState");
                currentState = PlayerState.Heal;
                isHealing = true;
                canDo = false;
            }
            
            
        }
        public IEnumerator LoadNextScene()
        {
            // Thêm hiệu ứng chờ đợi hoặc hiệu ứng chuyển cảnh tại đây nếu cần
            yield return new WaitForSeconds(1f); // Chờ 1 giây trước khi chuyển cảnh

            // Chuyển sang cảnh tiếp theo
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            Time.timeScale = 1;

            // Đảm bảo rằng cảnh mới đã được tải hoàn toàn trước khi di chuyển Player
            yield return new WaitForSeconds(0.1f); // Chờ một chút để cảnh mới tải xong
           
            // Di chuyển Player đến vị trí mới
            transform.position = new Vector3(0,-1,0);
        }

        public IEnumerator LoadFirtSence()
        {
            // Chờ 1 giây trước khi load scene (tùy chỉnh thời gian nếu muốn)
            yield return new WaitForSeconds(1f);

            // Load scene đầu tiên (index 0)
            SceneManager.LoadScene(0);
            yield return new WaitForSeconds(0.1f); // Chờ một chút để cảnh mới tải xong
            hp = maxHp;
            // Di chuyển Player đến vị trí mới
            transform.position = new Vector3(0, -1, 0);

        }





        void HandleIdleState()
        {
            // Player is standing still
            ChangeAnim("Idle");
            isDefending = false;
            canDo = true;
        }

        void HandleWalkState()
        {
            // Kiểm tra nếu có kẻ địch trước mặt
           
            // Kiểm tra nếu có kẻ địch trước mặt khi đang đứng yên
            if ( EnemyInFront())
            {
                //ChangeAnim("Idle");
                currentState = PlayerState.Idle;
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
            if (isAttack) 
            {
                // Check for enemies within attack range and apply damage
                StartCoroutine(ApplyDamge());
                SoundManager.Instance.PlayVFXSound(0);
                // Chuyển về trạng thái Idle sau khi tấn công
                isAttack = false;
                isDefending = false;    

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
            yield return new WaitForSeconds(0.2f);
            ApplyDamageToEnemies();
        }
        public void  ResetState()
        {

            currentState = PlayerState.Idle;
            canDo = true;  // Đảm bảo người chơi có thể thực hiện hành động tiếp theo
         

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
                    isDefending = false;
                    hp += 1;
                   
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
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Gate"))
            {
                StartCoroutine(LoadNextScene());
            }
        }

    }

    
        
    }



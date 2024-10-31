using UnityEngine;

namespace Script
{
    public class Trap : Enemies
    {
        [SerializeField] private bool isActive;

        public override void NextState()
        {
            if (isActive)
            {
                Debug.Log("Trap");
                base.NextState();
            }
                
            
        }

        public override void HandleAttackState()
        {
            base.HandleAttackState();
            ChangeAnim("atk");
            StartCoroutine(ResetState());
        }

        public override void HandlePrepareAttackState()
        {
            base.HandlePrepareAttackState();
            ChangeAnim("prepare");
        }
        public override void ApplyDamageToPlayer()
        {
            if (attacking)
            {
                return;
            }

            Vector2 start = transform.position;
            Vector2 end = start + Vector2.up * attackRange; // Thay đổi hướng Raycast sang bên trên
            Debug.DrawLine(start, end, Color.green, 1f); // Thay đổi màu để dễ nhận biết hơn
            RaycastHit2D[] hits = Physics2D.LinecastAll(start, end);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    hit.collider.GetComponent<Player>().TakeDamage(attackDamage);
                    attacking = true; // Đánh dấu đã gây damage
                    break; // Thoát khỏi vòng lặp sau khi gây damage
                }
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                isActive = true;
                
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                isActive = false;
            }
        }
        
        
        
    }
}
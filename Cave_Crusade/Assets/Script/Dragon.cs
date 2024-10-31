using UnityEngine;

namespace Script
{
    public class Dragon:Enemies
    {
        public GameObject Fireball;
        public float fireballSpacing = 2f; 
        private bool fireballsSpawned = false;
        public float fireballLifetime  ;
        public override void HandlePrepareAttackState()
        {
            base.HandlePrepareAttackState();
            ChangeAnim("prepare");
            
       
        }

        public override void HandleIdleState()
        {
            base.HandleIdleState();
            fireballsSpawned = false;
        }

        public override void HandleAttackState()
        {
            base.HandleAttackState();
            ChangeAnim("atk");
            if (!fireballsSpawned)
            {
                
                // Tạo quả cầu lửa đầu tiên
                Vector3 fireballPosition1 = transform.position + Vector3.left * fireballSpacing;
                GameObject fireball1 = Instantiate(Fireball, fireballPosition1, Quaternion.identity);
                Destroy(fireball1, fireballLifetime); // Hủy quả cầu lửa sau một khoảng thời gian

                // Tạo quả cầu lửa thứ hai
                Vector3 fireballPosition2 = transform.position + Vector3.left * fireballSpacing * 2;
                GameObject fireball2 = Instantiate(Fireball, fireballPosition2, Quaternion.identity);
                Destroy(fireball2, fireballLifetime); 

                fireballsSpawned = true; // Đánh dấu rằng fireballs đã được spawn
            }
            StartCoroutine(ResetState());
            
        }
    
    }
}
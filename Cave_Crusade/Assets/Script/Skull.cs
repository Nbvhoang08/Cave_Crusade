
namespace Script
{
   
    
    public class Skull : Enemies
    {
        
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

        public override void HandleDefendState()
        {
            base.HandleDefendState();
            ChangeAnim("def");
        }
    }
}
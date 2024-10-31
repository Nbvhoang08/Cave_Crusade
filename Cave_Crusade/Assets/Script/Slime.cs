namespace Script
{
    public class Slime: Enemies
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
    }
}
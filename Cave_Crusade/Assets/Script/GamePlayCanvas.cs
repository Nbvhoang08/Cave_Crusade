namespace Script
{
    public class GamePlayCanvas : UICanvas
    {


        public void AtkButton()
        {
            Player.Instance.OnAttackButtonClick();
        }

        public void DefButton()
        {
            Player.Instance.OnDefendButtonClick();
            
        }

        public void HealButton()
        {
            Player.Instance.OnHealButtonClick();
        }

        public void MoveButton()
        {
            Player.Instance.OnMoveButtonClick();
        }
        
    }
}
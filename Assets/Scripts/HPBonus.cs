public class HpBonus : Pellet
{
    protected override void Eat()
    {
        GameManager.Instance.HpBonusEaten(this);
    }
}

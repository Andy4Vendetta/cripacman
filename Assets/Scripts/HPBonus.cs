using UnityEngine;
public class HPBonus : Pellet
{
    protected override void Eat()
    {
        GameManager.Instance.HPBonusEaten(this);
    }
}

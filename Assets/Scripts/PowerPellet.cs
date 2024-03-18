using UnityEngine;

public class PowerPellet : Pellet
{
    public float duration = 7f; // продолжительность действия 

    protected override void Eat()
    {
        GameManager.Instance.PowerPelletEaten(this);
    }
}

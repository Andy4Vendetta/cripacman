using UnityEngine;

public class PowerPellet : Pellet
{
    public float duration = 8f; // продолжительность действия 

    protected override void Eat()
    {
        GameManager.Instance.PowerPelletEaten(this);
    }
}

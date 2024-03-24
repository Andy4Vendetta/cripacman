public class PowerPellet : Pellet
{
    public float duration = 7f; 
    protected override void Eat()
    {
        GameManager.Instance.PowerPelletEaten(this);
    }
}

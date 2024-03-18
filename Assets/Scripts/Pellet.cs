using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Pellet : MonoBehaviour
{
    public int points = 10;

    protected virtual void Eat()
    {
        GameManager.Instance.PelletEaten(this);
    }

// такой же, отдельный класс, похожая структура, OnTriggerEnter2D, все кроме 17 строки
//вызывать метод из 8 строки как у меня будет назван
// !Это чтобы регистрировать столкновение пакмана с пеллетами!
// сомнительно, но окай (оно вроде наследуется.. иначе не понимаю поч нет в PowerPellet)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pacman")) {
            Eat();
        }
    }

}

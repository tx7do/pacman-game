using UnityEngine;

public class Pacdot : MonoBehaviour
{
    public bool isSuperPacdot;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name != "Pacman") return;

        if (isSuperPacdot)
        {
            GameManager.Instance.OnEatPacdot(gameObject);
            GameManager.Instance.OnEatSuperPacdot();
            Destroy(gameObject);
        }
        else
        {
            GameManager.Instance.OnEatPacdot(gameObject);
            Destroy(gameObject);
        }
    }
}
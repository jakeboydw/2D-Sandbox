using UnityEngine;

public class DropTileController : MonoBehaviour
{
    public ItemClass item;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Inventory>().Add(item);
            Destroy(gameObject);
        }
    }
}

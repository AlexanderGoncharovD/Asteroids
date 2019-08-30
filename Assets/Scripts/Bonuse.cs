using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Скрипт вешается на бонусный объект
 */
public class Bonuse : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "ColliderPlayer")
        {
            collision.transform.parent.GetComponent<CharacterControl>().RegenerationHealth();
            Destroy(gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}

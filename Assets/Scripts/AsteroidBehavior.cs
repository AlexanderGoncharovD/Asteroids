using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Скрипт вешается на астероид, отвечает за его поведение (вращение и уничтожение)
 */
public class AsteroidBehavior : MonoBehaviour
{
    public int Damage, Score;
    public GameObject CrashEffect;
    public Vector2 RangeRotationForce = new Vector2(-2, 2);
    public AsteroidsGenerator AsteroidsGenerator;

    private void Start()
    {
        GetComponent<Rigidbody2D>().AddTorque(Random.Range(RangeRotationForce.x, RangeRotationForce.y), ForceMode2D.Impulse);
    }

    public void Crash()
    {
        if (CrashEffect != null)
            Instantiate(CrashEffect, transform.position, Quaternion.identity);

        AsteroidsGenerator.gameObject.GetComponent<AudioSource>().Play();
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "ColliderPlayer")
        {
            collider.transform.parent.GetComponent<CharacterControl>().Damage(Damage);
            Crash();
        }
    }

    private void OnBecameInvisible()
    {
        if (transform.position.y < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        AsteroidsGenerator.CurNumberAsteroids--;
    }
}

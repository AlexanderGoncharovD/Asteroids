using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Скрипт вешается на снаряд игрока
 * Отвечает за прямолинейный полёт снаряда и обработку столкновений с объектами на сцене
 */
public class Bullet : MonoBehaviour
{
    public float Force = 2.0f;
    public Vector2 Direction;
    public int Damage = 1;
    public GameObject CrushEffect;
    public CharacterControl character;
    private Rigidbody2D rigidbody;
    private Quaternion angle;

    void Start()
    {
        if (transform.position.y < -5.0f)
            Destroy(gameObject);

        rigidbody = GetComponent<Rigidbody2D>();
        Direction = new Vector2(0, 1.0f);
    }

    void Update()
    {
        rigidbody.velocity = Direction * Force;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        var tag = collider.gameObject.tag;
        if (tag == "Asteroid")
        {
            if (collider.transform.position.y <= 5.0f)
            {
                var asteroid = collider.gameObject.GetComponent<AsteroidBehavior>();
                character.GetComponent<CharacterControl>().BulletHitTarget(asteroid.Score);
                asteroid.Crash();
            }
            Instantiate(CrushEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else if (tag == "Enemy")
        {
            var enemy = collider.gameObject.transform.parent.GetComponent<EnemyControl>();
            enemy.Damage(Damage);
            Instantiate(CrushEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else if (tag == "Rocket")
        {
            Instantiate(CrushEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}

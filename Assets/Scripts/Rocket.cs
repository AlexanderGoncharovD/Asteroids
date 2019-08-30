using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Скрипт вешается на снаряд вражеского корабля
 * Отвечает за направление и ускорение снаряда в сторону игрока, обработку столкновений
 */
public class Rocket : MonoBehaviour
{
    public float Force = 2.0f;
    public int Damage = 1;
    public Transform Target;
    public GameObject CrushEffect;

    private Vector2 Direction;
    private Rigidbody2D rigidbody;
    private Quaternion angle;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        Direction = (Target.position - transform.position).normalized;
        angle = Quaternion.LookRotation(Direction);
        StartCoroutine(LookTarget());
    }

    void Update()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, angle, 4.0f * Time.deltaTime);
        rigidbody.velocity = transform.forward * Force;
    }

    private IEnumerator LookTarget()
    {
        yield return new WaitForSeconds(0.5f);
        Direction = (Target.position - transform.position).normalized;
        angle = Quaternion.LookRotation(Direction);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        var tag = collider.gameObject.tag;
        if (tag == "ColliderPlayer")
        {
            Target.GetComponent<CharacterControl>().Damage(Damage);
            Instantiate(CrushEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else if (tag == "Bullet")
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

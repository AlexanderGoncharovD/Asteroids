using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Скрипт вешается на родительский объект вражеского корабля
 * Отвеает за полёт от точки до точки своего пути, стрельбу по игроку, обработку урона
 */
public class EnemyControl : MonoBehaviour
{
    public int Score = 15, CurWave, damage, Health;
    public int[] AllHealthOnWave;
    public float[] RateOfFire;
    public GameObject[] Ways;
    public Transform[] way, Gun;
    public Transform cerebellum;
    public float Speed;
    public GameObject Bullet, CrushEffect, DamageEffect, Bonuse;
    public EnemyGenerator generator;
    [Range(0.0f, 1.0f)]
    public float ChanceSpawnBonuse = 1.0f;

    private Rigidbody2D rigidbody;
    private Transform player, target;
    private Vector3 thisPos;
    private float timer;

    private Vector2 direction;
    private Quaternion angle;
    private int curIndexTarget;
    private bool isMovement = true;
    private E_WaypointProperties action;
    private WaypointProperties waypointProperties;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        var newWay = Instantiate(Ways[CurWave], transform.position, Quaternion.identity);

        if (newWay.transform.position.x > 0)
        {
            newWay.transform.localScale = new Vector3(-1, 1, 1);
        }
        way = newWay.transform.GetComponentsInChildren<Transform>();
        target = way[curIndexTarget];
        Health = AllHealthOnWave[CurWave];
        timer = RateOfFire[CurWave];
    }

    void Update()
    {
        thisPos = transform.position;

        if (isMovement)
        {
            if ((thisPos - target.position).sqrMagnitude < 1.5f)
            {
                if (action == E_WaypointProperties.Next)
                {
                    NewWayPoint();
                }
                else if (action == E_WaypointProperties.Stop)
                {
                    isMovement = false;
                    rigidbody.velocity = Vector2.zero;
                    StartCoroutine(StopMovment(waypointProperties.StopTime));
                }
                else if (action == E_WaypointProperties.End)
                {
                    isMovement = false;
                    rigidbody.velocity = Vector2.zero;
                }
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, angle, 2.0f * Time.deltaTime);
                rigidbody.velocity = transform.forward * Speed;
            }
        }

        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            Shot();
        }
    }

    private IEnumerator StopMovment (float stopTime)
    {
        yield return new WaitForSeconds(stopTime);
        NewWayPoint();
;       isMovement = true;
    }

    private void NewWayPoint()
    {
        curIndexTarget++;
        if (curIndexTarget < way.Length)
        {
            target = way[curIndexTarget];
            direction = (target.position - transform.position).normalized;
            cerebellum.LookAt(target);
            angle = cerebellum.rotation;
            waypointProperties = target.GetComponent<WaypointProperties>();
            action = waypointProperties.Properties;
        }
        else
        {
            Destroy(way[0].gameObject);
            Destroy(gameObject);
        }
    }

    private void Shot()
    {
        if (thisPos.x > -2.0f && thisPos.x < 2.0f)
        {
            timer = RateOfFire[CurWave];
            for (int index = 0; index < Gun.Length; index++)
            {
                var newBullet = Instantiate(Bullet, Gun[index].position, Quaternion.identity);
                var scriptBullet = newBullet.GetComponent<Rocket>();
                scriptBullet.Target = player;
                scriptBullet.Damage = damage;
            }
        }
    }

    public void Damage(int value)
    {
        Health -= value;
        if (Health > 0)
        {
            Instantiate(DamageEffect, thisPos, Quaternion.identity);
        }
        else
        {
            Instantiate(CrushEffect, thisPos, Quaternion.identity);
            player.GetComponent<CharacterControl>().BulletHitTarget(Score);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Limiter")
        {
            Destroy(transform.parent.gameObject);
        }
    }

    private void OnDestroy()
    {
        generator.curNumberEnemies--;

        if (way[0] != null)
        {
            Destroy(way[0].gameObject);
        }

        if (Health <= 0)
        {
            generator.GetComponent<AudioSource>().Play();
        }

        if (Bonuse != null  && Health <= 0)
        {
            if (Random.Range(0.0f, 1.0f) <= ChanceSpawnBonuse)
            {
                Instantiate(Bonuse, thisPos, Quaternion.identity);
            }
        }
    }
}

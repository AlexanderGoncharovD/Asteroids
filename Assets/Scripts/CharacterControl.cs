using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 * Скрипт вешается на родительский объект корабля игрока
 * Отвечает за перемещение по горизонтали, стрельбу, обработку урона, воспроизведение анимаций для корабля
 */
public class CharacterControl : MonoBehaviour
{
    public float Speed, SpeedShooting;
    public int Health, unbreaking, score;
    public GameObject Bullet, HitEffect;
    public Transform[] Guns;
    public Text ScoreT, UnbreakingT;
    public bool isShooting;
    public AudioClip ShotSound, RepairSound, FeilSound;

    private Rigidbody2D rigidbody;
    private Vector2 directionMove, thisPos;
    private Animator animator;
    private float UnbreakingOneHealth, screenWpercent;
    private GameObject mainCamera;
    private Camera camera;
    private bool isRightSwipe;
    private AudioSource audio;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        camera = mainCamera.GetComponent<Camera>();
        animator = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();

        screenWpercent = Screen.width * 0.01f;
        UnbreakingOneHealth = 100.0f / Health;
        animator.SetFloat("SpeedShooting", SpeedShooting);
    }

    void Update()
    {
        thisPos = transform.position;

        if (Health > 0)
        {
            TouchTheScreen(camera.WorldToScreenPoint(thisPos));
        }
        else
        {
            if (animator.GetInteger("Strength") != 0)
            {
                animator.SetInteger("Strength", 0);
            }
        }

        rigidbody.velocity = directionMove * Speed;
    }

    private void TouchTheScreen(Vector2 screenPosCharacter)
    {
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Moved:
                        directionMove = (touch.position - screenPosCharacter).normalized;
                        break;

                    case TouchPhase.Ended:
                        directionMove = Vector2.zero;
                        break;
                }
            }
        }
    }

    public void Shot()
    {
        if(isShooting && Health > 0)
        {
            for (int index = 0; index < Guns.Length; index++)
            {
                var newBullet = Instantiate(Bullet, Guns[index].position, Quaternion.identity);
                newBullet.GetComponent<Bullet>().character = GetComponent<CharacterControl>();
                audio.PlayOneShot(ShotSound);
            }
        }
    }

    public void BulletHitTarget(int value)
    {
        score += value;
        ScoreT.text = score + "";
    }

    public void Damage(int value)
    {
        Health -= value;
        CalculateUnbreaking(Health);
        Instantiate(HitEffect, thisPos, Quaternion.identity);
        if (Health <= 0)
        {
            mainCamera.GetComponent<GameControl>().GameOver(score);

        }
    }

    private void CalculateUnbreaking(int health)
    {
        unbreaking = Mathf.RoundToInt(UnbreakingOneHealth * Health);
        animator.SetInteger("Strength", unbreaking);
        if (unbreaking < 0)
            unbreaking = 0;
        UnbreakingT.text = unbreaking + "%";
    }

    public void RegenerationHealth()
    {
        if (Health > 0)
        {
            Health = 100;
            CalculateUnbreaking(Health);
            audio.PlayOneShot(RepairSound);
        }
    }

    public void FeilSoundPlay()
    {
        audio.PlayOneShot(FeilSound);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Скрипт вешется на пустой объект
 * Объект должен находится на нужной для генерации астероидов высоте
 */
public class AsteroidsGenerator : MonoBehaviour
{
    public Vector2 RangeY = new Vector2(2.0f, 5.0f);
    public float RangeXGeneration = 1.5f;
    public GameObject[] Asteroids;
    public int CurNumberAsteroids;

    private float delayDistGeneration;
    private Vector3 generatorPos;
    private Vector2 AsteroidsPerMinute;
    private GameControl gameControl;
    private GameObject newAsteroid;
    private int NumberAsteroidsGenerated;
    private bool isGeneration;

    private void Awake()
    {
        gameControl = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameControl>();
    }

    void Start()
    {
        generatorPos = transform.position;
    }

    public void Update()
    {
        if (isGeneration)
        {
            if (newAsteroid != null)
            {
                if ((transform.position.y - newAsteroid.transform.position.y) > delayDistGeneration)
                {
                    Generation();
                }
            }
            else
            {
                Generation();
            }
        }
        else
        {
            if (CurNumberAsteroids == 0)
            {
                gameControl.EndWave();
            }
        }
    }

    public void Generate()
    {
        Generation();
        isGeneration = true;
    }

    public void StopGeneration()
    {
        isGeneration = false;
        if (newAsteroid.transform.position.y > 5.0f)
        {
            Destroy(newAsteroid);
        }
    }

    private void Generation( )
    {
        var newGenPos = new Vector3(generatorPos.x + Random.Range(-RangeXGeneration, RangeXGeneration), generatorPos.y, 0);

        newAsteroid = Instantiate(Asteroids[Random.Range(0, Asteroids.Length)], newGenPos, Quaternion.identity);
        newAsteroid.GetComponent<AsteroidBehavior>().AsteroidsGenerator = GetComponent<AsteroidsGenerator>();

        delayDistGeneration = Random.Range(RangeY.x, RangeY.y);
        CurNumberAsteroids++;
        NumberAsteroidsGenerated++;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Скрипт вешается на пустой объект
 * Отвечает за спаун вражеских кораблей в соответсвующей волне
 */
[System.Serializable]
public struct S_EnemySettings
{
    #if UNITY_EDITOR
        public string name;
    #endif
    public GameObject Model;
    public Transform[] PointSpawn;
    public int Number;
    public bool Simultaneously;
    public float TimingSpawn, TimingBetweenPoints, TimingSpawnNextType;
}

[System.Serializable]
public struct S_Wave
{
    #if UNITY_EDITOR
        public string name;
    #endif
    public S_EnemySettings[] EnemySettings;
}

public class EnemyGenerator : MonoBehaviour
{
    public int CurWave, curNumberEnemies;
    public S_Wave[] Wave;
    
    private GameObject newEnemy;
    private EnemyGenerator generator;
    private bool isGeneration;
    private float timer;
    private int curTypeEnemy, // генерируемый тип врага (очерёдность в порядке массива)
        curNumberGenerated, // количество уже сгенерированных врагов
        curPointSpawn; // текущая точка спауна врага
    private S_EnemySettings settings;
    private GameControl gameControl;

    void Awake()
    {
        generator = GetComponent<EnemyGenerator>();
        gameControl = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameControl>();
    }

    void Update()
    {
        if (isGeneration)
        {
            if (timer > 0) // Задержка перед спауном следующего врага
            {
                timer -= Time.deltaTime;
            }
            else
            {
                // Если ещё не закончилась генерация типов врагов в данной волне
                if (curTypeEnemy < Wave[CurWave].EnemySettings.Length)
                {
                    settings = Wave[CurWave].EnemySettings[curTypeEnemy];

                    // Одновременная генерация на всех точках спауна
                    if (settings.Simultaneously)
                    {
                        // Сгенерировать врага на текущей точке спауна (для данного типа врага)
                        if (curPointSpawn < settings.PointSpawn.Length)
                        {
                            Create(settings.Model, settings.PointSpawn[curPointSpawn], CurWave); // Сгенерировать врага (модель врага, текущая точка для спауна, номер волны)
                            curNumberGenerated++; // увеличить количество сгенреированных врагов
                            curPointSpawn++; // Следующая точка спавна
                            timer = settings.TimingBetweenPoints; // Активировать задержка генерации врагов между точками
                        }
                        else // Если уже на каждую точку спауна сгенерировано по одному врагу
                        {
                            // Если всё количество враговсгенерировано (Формула: Количество врагов умноженное на количество точек спауна)
                            if (curNumberGenerated == settings.Number * settings.PointSpawn.Length)
                            {
                                curNumberGenerated = 0; // Обнулить количество сгенерированных врагов
                                curTypeEnemy++; // Переключиться на следующий тип врага
                                curPointSpawn = 0; // Обнулить точку спауна
                                timer = settings.TimingSpawnNextType; // Включить задержку для генерации следующиего типа врага
                            }
                            else // Если не сгенерированы всё количество врагов
                            {
                                curPointSpawn = 0; // Обнулить индекс точек спауна
                                timer = settings.TimingSpawn; // Включить задержку для генерации следующиего врага этого же типа
                            }
                        }
                    }
                    else // Поочерёдная генерация на всех точках спауна
                    {
                        Create(settings.Model, settings.PointSpawn[curPointSpawn], CurWave); // Сгенерировать врага (модель врага, текущая точка для спауна, номер волны)
                        curNumberGenerated++; // увеличить количество сгенреированных врагов

                        if (curNumberGenerated == settings.Number) // Если все враги сгенерированы
                        {
                            curNumberGenerated = 0; // Обнулить количество сгенерированных врагов
                                                    // Если враги одного типа были сгенерированы на всех доступных точка для спана для этого типа
                            if (curPointSpawn == settings.PointSpawn.Length - 1)
                            {
                                curTypeEnemy++; // Переключиться на следующий тип
                                curPointSpawn = 0; // Обнулить точку спауна
                                timer = settings.TimingSpawnNextType; // Включить задержку для генерации следующиего типа врага
                            }
                            else
                            {
                                curPointSpawn++; // Выбрать следущую точку для спауна врагов
                                timer = settings.TimingBetweenPoints; // Задержка генерации между точками
                            }
                        }
                        else
                        {
                            timer = settings.TimingSpawn; // Включить задержку для генерации следующиего врага этого же типа
                        }
                    }
                }
                else // Если закончилась генерация типов врагов в данной волне
                {
                    // Ожидание, когда исчезнут все враги
                    if (curNumberEnemies == 0)
                    {
                        isGeneration = false;
                        gameControl.EndWave();
                    }
                }
            }
        }    
    }

    public void Generate(int wave)
    {
        timer = 2.0f;
        curTypeEnemy = 0;
        CurWave = wave;
        isGeneration = true;
    }

    private void Create(GameObject model, Transform transform, int way)
    {
        newEnemy = Instantiate(model, transform.position, transform.rotation);
        var enemyControl = newEnemy.GetComponent<EnemyControl>();
        enemyControl.generator = generator;
        enemyControl.CurWave = CurWave;
        curNumberEnemies++;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/*
 * Скрипт вешается на основную камеру 
 * Отвечает за начало игры и спаун корабля игрока и обработку нажатий кнопок, подсчёт полученного опыта
 */
public class GameControl : MonoBehaviour
{
    public GameObject GameUI, MenuUI, PauseObj, LevelLine, ButtonBackToMenu;
    public GameObject[] Characters;
    public Transform SpawnPointCharacter, line;
    public Text WaveT, ScoreT, UnbreackingT, curLvl, nextLvl, ExpT;
    public int[] Experience = new int[3];
    public AsteroidsGenerator AsteroidsGenerator;
    public EnemyGenerator EnemyGenerator;
    public int lvl;

    private Animator animator;
    private int Wave = -1, leftAddScore, exp;
    private CharacterControl character;
    private bool isPause, gameOver, isCalculationExp;
    private AudioSource audio;

    private void Start()
    {
        animator = GetComponent<Animator>();
        audio = GetComponent<AudioSource>(); 

        exp = PlayerPrefs.GetInt("Exp");
        lvl = PlayerPrefs.GetInt("lvl");

        UpdateLevelUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameOver)
            {
                if (leftAddScore == 0)
                {
                    BackToMenu();
                }
            }
            else
            {
                if (GameUI.activeSelf)
                {
                    Pause();
                }
            }
        }

        if (gameOver && isCalculationExp)
        {
            for (int i = 0; i < 5; i++)
            {
                if (leftAddScore > 0)
                {
                    if (lvl < Experience.Length)
                    {
                        exp++;
                        leftAddScore--;
                        if (exp == Experience[lvl])
                        {
                            lvl++;
                            exp = 0;
                        }
                    }
                    else
                    {
                        exp = 0;
                        leftAddScore = 0;
                    }
                    UpdateLevelUI();
                }
                else
                {
                    PlayerPrefs.SetInt("Exp", exp);
                    PlayerPrefs.SetInt("lvl", lvl);

                    ButtonBackToMenu.SetActive(true);
                    break;
                }
            }
        }
    }

    public void Pause()
    {
        audio.Play();
        isPause = !isPause;
        PauseObj.SetActive(isPause);

        if (isPause)
            Time.timeScale = 0.0f;
        else
            Time.timeScale = 1.0f;
    }

    public void BackToMenu()
    {
        audio.Play();
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {
        audio.Play();
        Application.Quit();
    }

    public void CharacterStartsToShoot()
    {
        character = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterControl>();
        character.isShooting = true;
    }

    private void UpdateWave()
    {
        if (!gameOver)
        {
            if (Wave < EnemyGenerator.Wave.Length - 1)
            {
                Wave++;
                WaveT.text = "Wave " + (Wave + 1);
                animator.Play("Show wave text");
                AsteroidsGenerator.Generate();
                EnemyGenerator.Generate(Wave);
            }
            else
            {
                animator.Play("Game win");
            }

            if (character != null)
            {
                character.isShooting = false;
            }
        }
    }

    public void EndWave()
    {
        if (AsteroidsGenerator.CurNumberAsteroids == 0)
        {
            if (GameUI.activeSelf)
            {
                UpdateWave();
            }
        }
        else
        {
            AsteroidsGenerator.StopGeneration();
        }
    }

    public void GameOver(int score)
    {
        animator.SetBool("GameOver", true);
        gameOver = true;
        AsteroidsGenerator.enabled = false;
        EnemyGenerator.enabled = false;

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy);
        }

        foreach (GameObject asteroid in GameObject.FindGameObjectsWithTag("Asteroid"))
        {
            Destroy(asteroid);
        }
    }

    public void CalculationExp()
    {
        leftAddScore = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterControl>().score;
        gameOver = true;
        LevelLine.SetActive(true);
        isCalculationExp = true;
    }

    private void UpdateLevelUI()
    {
        if (lvl < Experience.Length)
        {
            curLvl.text = (lvl + 1) + "";
            nextLvl.text = (lvl + 1) < Experience.Length ? (lvl + 2) + "" : "max";
            ExpT.text = exp + " / " + Experience[lvl];

            var size = (exp / (Experience[lvl] / 100.0f)) / 50.0f;
            line.localScale = new Vector3(size, 1, 1);
        }
        else
        {
            curLvl.text = lvl + "";
            nextLvl.text = "max";
            ExpT.text = Experience[lvl-1] + " / " + Experience[lvl-1];
            line.localScale = new Vector3(2, 1, 1);
        }
    }

    public void CreateCharacter(int index)
    {
        var newCharacter = Instantiate(Characters[index], SpawnPointCharacter.position, Quaternion.identity);
        newCharacter.GetComponent<CharacterControl>().ScoreT = ScoreT;
        newCharacter.GetComponent<CharacterControl>().UnbreakingT = UnbreackingT;
        GameUI.SetActive(true);
        MenuUI.SetActive(false);
        UpdateWave();
    }
}

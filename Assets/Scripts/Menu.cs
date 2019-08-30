using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 * Скрипт вешается на элемент UI menu
 * Отвечает за выбор корабля в магазине
 */
public class Menu : MonoBehaviour
{
    public Text NeedLvl;
    public Button Play;
    public Animator animator;
    public GameObject MenuButtons, SelectCharacters, LevelLine;
    public AudioSource audio;

    private int curCharacter;
    private Vector2 touchedScreen, swipe;
    private GameControl gameControl;
    private bool isSwipe;

    private void Start()
    {
        gameControl = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameControl>();
        curCharacter = PlayerPrefs.GetInt("curCharacter");
        UpdateCharacter(curCharacter);
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        touchedScreen = touch.position;
                        break;
                    case TouchPhase.Moved:
                        swipe = touch.position - touchedScreen;
                        if (!isSwipe)
                        {
                            if (touch.deltaPosition.x > Screen.width * 0.05f)
                            {
                                if (curCharacter > 0)
                                {
                                    curCharacter--;
                                    isSwipe = true;
                                    UpdateCharacter(curCharacter);
                                }
                            }
                            else if (touch.deltaPosition.x < -(Screen.width * 0.05f))
                            {
                                if (curCharacter < gameControl.Experience.Length - 1)
                                {
                                    curCharacter++;
                                    isSwipe = true;
                                    UpdateCharacter(curCharacter);
                                }
                            }
                        }
                        break;
                    case TouchPhase.Ended:
                        touchedScreen = Vector2.zero;
                        isSwipe = false;
                        break;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackToMenu();
        }
    }

    

    private void UpdateCharacter(int cur)
    {
        if (cur <= gameControl.lvl)
        {
            Play.interactable = true;
            NeedLvl.gameObject.SetActive(false);
        }
        else
        {
            Play.interactable = false;
            NeedLvl.text = "need level " + (cur + 1);
            NeedLvl.gameObject.SetActive(true);
        }
        animator.SetInteger("curCharacter", cur);
    }

    public void OpenCharacters()
    {
        audio.Play();
        MenuButtons.SetActive(false);
        SelectCharacters.SetActive(true);
        LevelLine.SetActive(true);
    }

    public void BackToMenu()
    {
        audio.Play();
        SelectCharacters.SetActive(false);
        MenuButtons.SetActive(true);
        LevelLine.SetActive(false);
    }

    public void PlayGame()
    {
        audio.Play();
        PlayerPrefs.SetInt("curCharacter", curCharacter);
        LevelLine.SetActive(false);
        gameControl.CreateCharacter(curCharacter);
    }
}

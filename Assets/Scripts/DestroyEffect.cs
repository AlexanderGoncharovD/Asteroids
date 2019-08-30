using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Скрипт вешается на эффект взрыва
 * Отвечает за удаление эффекта после проигрывания анимации
 */
public class DestroyEffect : MonoBehaviour
{
    private void DeletEffect()
    {
        Destroy(gameObject);
    }
}

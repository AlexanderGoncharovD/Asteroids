using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Скрипт вешается на все точки в пути для вражеского корабля
 * Отвечает за указание выполнения следующего действия для корябля врага
 */
public enum E_WaypointProperties
{
    Next,
    Stop,
    End,
}

public class WaypointProperties : MonoBehaviour
{
    public E_WaypointProperties Properties;
    public float StopTime;
}

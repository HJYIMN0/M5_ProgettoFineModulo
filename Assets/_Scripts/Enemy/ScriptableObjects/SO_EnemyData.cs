using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/Enemy", order = 1)]
public class SO_EnemyData : ScriptableObject
{

    public string enemyName;
    public int hp;

    public float speed;
    public int speedMultiplier;
    
    public int damage;

    public float viewRadius;
    
    public int rayCount;

    [Range(0,360)] public float viewAngle;

    public float coroutineTime = 4.5f;


}

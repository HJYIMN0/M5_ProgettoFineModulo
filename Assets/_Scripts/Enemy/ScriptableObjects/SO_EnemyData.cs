using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/Enemy", order = 1)]
public class SO_EnemyData : ScriptableObject
{

    public string enemyName;
    public int hp;

    public float speed;
    public float speedMultiplier;
    
    public int damage;

    public float viewRadius;
    
    public int rayCount;

    public float Ui_alpha = 0.8f;

    [Range(0,360)] public float viewAngle;

    public float coroutineTime = 4.5f;

    public float lastSeenTime = 1.5f;

    public float interactionRadius = 2f;

    public LayerMask playerMask;
    public LayerMask obstructionMask;


}

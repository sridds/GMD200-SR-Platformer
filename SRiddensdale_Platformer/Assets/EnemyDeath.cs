using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    [SerializeField]
    private Health _health;

    void Start()
    {
        _health.OnHealthDepleted += Death;
    }

    private void Death()
    {

    }
}

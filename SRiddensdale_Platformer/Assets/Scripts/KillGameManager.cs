using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillGameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.Instance == null) return;
        Destroy(GameManager.Instance);
    }
}

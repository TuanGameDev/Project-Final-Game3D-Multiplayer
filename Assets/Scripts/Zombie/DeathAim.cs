using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DeathAim : MonoBehaviour
{
    public float destroyTime;
    void Start()
    {
        Invoke(nameof(OnDestroyObject),destroyTime);
    }
    private void OnDestroyObject()
    {
        Destroy(gameObject);
    }
}

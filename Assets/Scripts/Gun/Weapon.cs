using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour, IUsable
{
    [field: SerializeField]
    public UnityEvent OnShoot { get;private set; }
    [field: SerializeField]
    public UnityEvent OnZoom { get;private set; }
    [field: SerializeField]
    public UnityEvent OnReload { get; private set; }
    public void Shoot(GameObject actor)
    {
        OnShoot?.Invoke();
    }
    public void Zoom(GameObject actor)
    {
        OnZoom?.Invoke();
    }
    public void Reload(GameObject actor)
    {
        OnReload?.Invoke();
    }
}


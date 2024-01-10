using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IUsable
{
    void Shoot(GameObject actor);
    void Zoom(GameObject actor);
    UnityEvent OnShoot { get; }
    UnityEvent OnZoom { get; }
}

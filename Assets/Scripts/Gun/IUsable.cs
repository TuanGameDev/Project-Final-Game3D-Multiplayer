using UnityEngine;
using UnityEngine.Events;

public interface IUsable
{
    void Shoot(GameObject actor);
    void Zoom(GameObject actor);
    void Reload(GameObject actor);
    UnityEvent OnShoot { get; }
    UnityEvent OnZoom { get; }
    UnityEvent OnReload { get; }
}

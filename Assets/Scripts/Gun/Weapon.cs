using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class Weapon : MonoBehaviourPun, IUsable
{
    [field: SerializeField]
    public UnityEvent OnShoot { get; private set; }
    [field: SerializeField]
    public UnityEvent OnZoom { get; private set; }
    [field: SerializeField]
    public UnityEvent OnReload { get; private set; }

    public void Shoot(GameObject actor)
    {
        photonView.RPC("WeaponShootRPC", RpcTarget.All);
    }

    [PunRPC]
    void WeaponShootRPC()
    {
        OnShoot?.Invoke();
    }

    public void Zoom(GameObject actor)
    {
        photonView.RPC("WeaponZoomRPC", RpcTarget.All);
    }

    [PunRPC]
    void WeaponZoomRPC()
    {
        OnZoom?.Invoke();
    }

    public void Reload(GameObject actor)
    {
        photonView.RPC("WeaponReloadRPC", RpcTarget.All);
    }

    [PunRPC]
    void WeaponReloadRPC()
    {
        OnReload?.Invoke();
    }
}

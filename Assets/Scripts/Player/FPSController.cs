using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
public class FPSController : MonoBehaviourPun
{
    public static FPSController instance;
	public int id;
	public static FPSController me;
    public Player photonPlayer;
    PhotonView PV;

    [Header("Gun")]
    [SerializeField] Camera cameraHolder;
    public GameObject aimingObject;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    [PunRPC]
    public void Initialized(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;
        GameManager.gamemanager.players[id - 1] = this;
        if (player.IsLocal)
            me = this;
    }
    private void Start()
	{

	}
	private void Update()
	{

        /*CheckAiming();*/

    }
/*    public void CheckAiming()
    {
        Ray ray = cameraHolder.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.name == aimingObject.name || hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
                return;
            Vector3 hitPoint = hit.point;
            if (aimingObject != null)
            {
                MoveObjectToPosition(hitPoint);
            }
        }
        else
        {
            Vector3 endPoint = ray.GetPoint(200f);
            if (aimingObject != null)
            {
                MoveObjectToPosition(endPoint);
            }
        }
    }
    private void MoveObjectToPosition(Vector3 targetPosition)
    {
        Vector3 newPosition = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z);
        aimingObject.transform.position = newPosition;
    }*/
}

using UnityEngine;

public class PlayerEquipLockPick : MonoBehaviour
{
    public Transform inventoryTransform;
    public bool hasLockPick = false;

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            LockPickEquip lockPickEquip = other.GetComponent<LockPickEquip>();
            if (lockPickEquip != null)
            {
                lockPickEquip.transform.parent = inventoryTransform;
                lockPickEquip.transform.localPosition = Vector3.zero;
                lockPickEquip.transform.localRotation = Quaternion.identity;
                lockPickEquip.gameObject.SetActive(false);
                hasLockPick = true;
            }
        }
    }

    public bool HasLockPick()
    {
        return hasLockPick;
    }
}

using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRecoil : MonoBehaviour
{
    [HideInInspector] public CinemachineFreeLook playerCamera;
    [HideInInspector] public CinemachineImpulseSource cameraShake;
    [HideInInspector] public Animator rig;

    [SerializeField] Vector2[] recoilPattern;


    public float duration;

    float verRecoil;
    float horRecoil;
    float time;
    int index;

    int NextIndex(int index)
    {
        return (index +1) % recoilPattern.Length;
    }

    private void Awake()
    {
        playerCamera = GetComponentInParent<CinemachineFreeLook>();
        cameraShake = GetComponent<CinemachineImpulseSource>();
    }

    void Update()
    {
        if (time > 0)
        {
            playerCamera.m_YAxis.Value -= ((verRecoil/1000) * Time.deltaTime) / duration;
            playerCamera.m_XAxis.Value -= ((horRecoil/10) * Time.deltaTime) / duration;
            time -= Time.deltaTime;
        }
    }

    public void GenerateRecoil(string weaponName)
    {
        time = duration;
        cameraShake.GenerateImpulse(playerCamera.transform.forward);

        horRecoil = recoilPattern[index].x;
        verRecoil = recoilPattern[index].y;
        index = NextIndex(index);
        rig.Play("weapon_" + weaponName + "_recoil", 1, 0f);
    }

    public void Reset()
    {
        index = 0;
    }
}

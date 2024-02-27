using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHairTarget : MonoBehaviour
{
    Camera _cam;
    Ray _ray;
    RaycastHit _hit;
    void Start()
    {
        _cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        _ray = _cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.Raycast(_ray, out _hit))
        {
            transform.position = _hit.point;
        }
        else
        {
            transform.position = _ray.GetPoint(1000.0f);
        }
    }
}

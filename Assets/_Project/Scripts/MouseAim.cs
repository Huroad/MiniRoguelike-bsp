using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAim : MonoBehaviour
{
    public GameObject mouseAimPrefab;
    private GameObject mouseAim;
    
    void Start()
    {
        mouseAim = Instantiate(mouseAimPrefab);
        Cursor.visible = false;
    }

    void Update()
    {
        if(Camera.main == null || mouseAim == null)
            return;
        
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        mouseAim.transform.position = mousePosition;
    }

}

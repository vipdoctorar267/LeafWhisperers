using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchManager : MonoBehaviour
{
    public bool isTorchActive;
    void Start()
    {
        isTorchActive = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") != null)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                isTorchActive = true;
            }
        }
    }
}



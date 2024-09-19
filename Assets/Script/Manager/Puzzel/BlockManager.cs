using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlockData
{
    public int index; // Số thứ tự của block trong danh sách
    public GameObject block; // Khối (Block) chứa đèn
    public int firstState; // Trạng thái ban đầu của khối
    public bool isActive; // Trạng thái hiện tại (đã sáng hết đèn chưa)
}

public class BlockManager : MonoBehaviour
{
    public BlockData blockData;
    public GameObject light01;
    public GameObject light02;
    public GameObject light03;

    public TorchManager _torch01;
    public TorchManager _torch02;
    public TorchManager _torch03;


    private void Start()
    {
        UpdateLights();
        blockData.isActive = false;
    }
    private void FixedUpdate()
    {
        UpdateLights();
    }


    private void UpdateLights()
    {
        if (_torch01.isTorchActive)
        {
            light01.SetActive(true);
        }
        else
        {
            light01.SetActive(false);
        }

        if (_torch02.isTorchActive)
        {
            light02.SetActive(true);
        }
        else
        {
            light02.SetActive(false);
        }


        if (_torch03.isTorchActive)
        {
            light03.SetActive(true);
        }
        else
        {
            light03.SetActive(false);
        }

    }


    public void setBlockActive()
    {
        if (light01.activeSelf && light02.active && light03.activeSelf)
        {
            blockData.isActive = true;
        }
        else
        {
            blockData.isActive = false;
        }
    }
}

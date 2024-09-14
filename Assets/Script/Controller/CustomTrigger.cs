using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomTrigger : MonoBehaviour
{
    public event System.Action<Collider2D> EnteredTrigger;
    public event System.Action<Collider2D> ExitedTrigger;

    // Phương thức này được gọi khi một collider khác vào phạm vi trigger collider gán cho đối tượng
    void OnTriggerEnter2D(Collider2D collider)
    {
        EnteredTrigger?.Invoke(collider);
    }

    // Phương thức này được gọi khi một collider khác rời khỏi phạm vi trigger collider gán cho đối tượng
    void OnTriggerExit2D(Collider2D collider)
    {
        ExitedTrigger?.Invoke(collider);
    }
}

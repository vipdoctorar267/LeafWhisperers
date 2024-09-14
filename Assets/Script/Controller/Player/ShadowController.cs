using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowController : MonoBehaviour
{
    public GameObject _shadow;      // Transform của bóng
    public Transform player;      // Transform của nhân vật chính
    public Transform shadow;      // Transform của bóng
    public LayerMask groundLayer; // Layer của mặt đất (Tilemap)
    public float shadowHeightOffset = 0.1f; // Độ cao của bóng so với mặt đất
    public float maxShadowLength = 1.0f;    // Chiều dài tối đa của bóng

    void Update()
    {
        // Kiểm tra địa hình dưới nhân vật bằng Raycast
        RaycastHit2D hit = Physics2D.Raycast(player.position, Vector2.down, Mathf.Infinity, groundLayer);

        if (hit.collider != null)
        {
            // Tính khoảng cách từ nhân vật tới mặt đất
            float distanceToGround = player.position.y - hit.point.y;

            // Cập nhật vị trí của bóng để luôn nằm ngay dưới nhân vật
            shadow.position = new Vector3(player.position.x, hit.point.y + shadowHeightOffset, player.position.z);

            // Điều chỉnh kích thước bóng dựa trên khoảng cách đến mặt đất
            float shadowScale = Mathf.Clamp(distanceToGround, 0, maxShadowLength);

            // Thay đổi kích thước bóng (chỉ thay đổi chiều cao của bóng)
            shadow.localScale = new Vector3(shadow.localScale.x, shadowScale, shadow.localScale.z);
        }
        else
        {
            // Ẩn bóng nếu không có mặt đất dưới nhân vật
            _shadow.SetActive(false);
        }
    }
}

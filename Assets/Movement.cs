using UnityEngine;

public class IceSlidePlayer : MonoBehaviour
{
    public float moveSpeed = 5f;
    private bool isSliding = false;
    private Vector2 moveDir;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isSliding) return;

        if (Input.GetKeyDown(KeyCode.W)) Slide(Vector2.up);
        else if (Input.GetKeyDown(KeyCode.S)) Slide(Vector2.down);
        else if (Input.GetKeyDown(KeyCode.A)) Slide(Vector2.left);
        else if (Input.GetKeyDown(KeyCode.D)) Slide(Vector2.right);
    }

    void Slide(Vector2 direction)
    {
        moveDir = direction;
        StartCoroutine(SlideMovement());
    }

    System.Collections.IEnumerator SlideMovement()
    {
        isSliding = true;

        while (true)
        {
            Vector2 nextPos = (Vector2)transform.position + moveDir;

            // 🔍 Kiểm tra nếu ô kế tiếp là đích → dừng lại ngay
            Collider2D goalCheck = Physics2D.OverlapCircle(nextPos, 0.1f, LayerMask.GetMask("Goal"));
            if (goalCheck != null)
            {
                Debug.Log("🚩 Phía trước là ô đích, dừng lại tại vị trí hiện tại.");
                break;
            }

            // 🔍 Nếu là vật cản thì dừng
            Collider2D obstacleCheck = Physics2D.OverlapCircle(nextPos, 0.1f, LayerMask.GetMask("Obstacle"));
            if (obstacleCheck != null)
            {
                break;
            }

            // Nếu không, di chuyển đến ô tiếp theo
            yield return StartCoroutine(MoveOneStep(nextPos));
        }

        isSliding = false;
    }

    System.Collections.IEnumerator MoveOneStep(Vector2 targetPos)
    {
        float elapsed = 0f;
        Vector2 startPos = transform.position;
        Vector2 endPos = targetPos;

        while (elapsed < 1f / moveSpeed)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector2.Lerp(startPos, endPos, elapsed * moveSpeed);
            yield return null;
        }

        transform.position = endPos;
    }
}

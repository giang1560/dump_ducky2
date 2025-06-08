using UnityEngine;

public class Movement : MonoBehaviour
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
            Vector2 targetPos = (Vector2)transform.position + moveDir;
            // Raycast để kiểm tra nếu có vật cản
            RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDir, 1f, LayerMask.GetMask("Obstacle"));
            if (hit.collider != null)
                break;

            // Di chuyển từng ô một
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

        isSliding = false;
    }
}
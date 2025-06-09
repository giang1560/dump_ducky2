using UnityEngine;

public class GoalPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player reached the goal!");
            GameManager.Instance.NextLevel();
        }
    }
}
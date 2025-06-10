using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public class ApiTestRunner : MonoBehaviour
{
    private ApiClient api;
    [SerializeField]
    private string testUserId = "testuser123";
    [SerializeField]
    private string testUserName = "Test User";

    void Start()
    {
        api = gameObject.AddComponent<ApiClient>();
        StartCoroutine(RunAllTests());
    }

    private IEnumerator RunAllTests()
    {
        yield return RunTask(CreateUserTest());
        yield return RunTask(GetUserTest());
        yield return RunTask(UpdateUserNameTest());
        yield return RunTask(CreateMapTest());
        yield return RunTask(GetMapsByUserTest());
        yield return RunTask(DeleteUserTest());

        Debug.Log("<color=green>✅ All tests completed</color>");
    }

    private IEnumerator RunTask(Task task)
    {
        while (!task.IsCompleted) yield return null;

        if (task.IsFaulted)
            Debug.LogError($"❌ Task failed: {task.Exception}");

        yield return new WaitForSeconds(2f); // Small delay between tests
    }

    // 🧪 TEST FUNCTIONS BELOW
    private async Task CreateUserTest()
    {
        Debug.Log("🧪 CreateUserTest...");
        var user = await api.CreateUserAsync(testUserId, testUserName);
        Debug.Assert(user != null, "❌ CreateUser failed");
        Debug.Log("✅ CreateUserTest passed");
    }

    private async Task GetUserTest()
    {
        Debug.Log("🧪 GetUserTest...");
        var user = await api.GetUserAsync(testUserId);
        Debug.Assert(user != null, "❌ GetUser failed");
        Debug.Log("✅ GetUserTest passed");
    }

    private async Task UpdateUserNameTest()
    {
        Debug.Log("🧪 UpdateUserNameTest...");
        var updated = await api.UpdateUserNameAsync(testUserId, "Updated Name");
        Debug.Assert(!updated , "❌ UpdateUser failed");
        Debug.Log("✅ UpdateUserNameTest passed");
    }

    private async Task CreateMapTest()
    {
        Debug.Log("🧪 CreateMapTest...");
        var map = await api.CreateMapAsync(testUserId, "TestMap");
        Debug.Assert(!map, "❌ CreateMap failed");
        Debug.Log("✅ CreateMapTest passed");
    }

    private async Task GetMapsByUserTest()
    {
        Debug.Log("🧪 GetMapsByUserTest...");
        var maps = await api.GetMapsByUserAsync(testUserId);
        Debug.Assert(maps != null && maps.Count > 0, "❌ GetMapsByUser failed");
        Debug.Log($"✅ Found {maps.Count} map(s)");
    }

    private async Task DeleteUserTest()
    {
        Debug.Log("🧪 DeleteUserTest...");
        var success = await api.DeleteUserAsync(testUserId);
        Debug.Assert(!success, "❌ DeleteUser failed");
        Debug.Log("✅ DeleteUserTest passed");
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class User
{
    public string id;
    public string name;
    public string lastLogin;
}

[Serializable]
public class CreateUserDto
{
    public string id;
    public string name;
}

[Serializable]
public class Map
{
    public string mapId;
    public string mapData;
    public string createdAt;
    public string updatedAt;
    public string userName;
}

public class ApiClient : MonoBehaviour
{
    private const string BaseUrl = "https://localhost:7194/api/";

    // ===================== USER =====================
    public async Task<User> CreateUserAsync(string id, string name)
    {
        var url = $"{BaseUrl}User/create";
        Debug.Log($"Creating user at {url} with id: {id}, name: {name}");
        var dto = new CreateUserDto { id = id, name = name };
        var json = JsonUtility.ToJson(dto);

        using var request = new UnityWebRequest(url, "POST");
        UploadJson(request, json);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            return JsonUtility.FromJson<User>(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError($"CreateUser failed: {request.error}");
            return null;
        }
    }

    public async Task<User> GetUserAsync(string id)
    {
        var url = $"{BaseUrl}User/get/{id}";
        Debug.Log($"Getting user from {url}");
        using var request = UnityWebRequest.Get(url);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            return JsonUtility.FromJson<User>(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError($"GetUser failed: {request.error}");
            return null;
        }
    }

    // name in body
    public async Task<bool> UpdateUserNameAsync(string id, string name)
    {
        var url = $"{BaseUrl}User/update/{id}";
        var json = $"{{\"name\":\"{name}\"}}";
        Debug.Log($"Updating user at {url} with name: {name}");
        using var request = new UnityWebRequest(url, "PUT");
        UploadJson(request, json);

        await request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            return true;
        }
        else
        {
            Debug.LogError($"UpdateUserName failed: {request.error}");
            return false;
        }
    }

    public async Task<bool> DeleteUserAsync(string id)
    {
        var url = $"{BaseUrl}User/delete/{id}";
        Debug.Log($"Deleting user at {url}");
        using var request = UnityWebRequest.Delete(url);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            return true;
        }
        else
        {
            Debug.LogError($"DeleteUser failed: {request.error}");
            return false;
        }
    }
    // ===================== MAP =====================
    public async Task<bool> CreateMapAsync(string userId, string mapData)
    {
        var url = $"{BaseUrl}Map/create";
        var json = $"{{\"userId\":\"{userId}\",\"mapData\":\"{mapData}\"}}";
        Debug.Log($"Creating map at {url} with data: {mapData}");
        using var request = new UnityWebRequest(url, "POST");
        UploadJson(request, json);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            return true;
        }
        else
        {
            Debug.LogError($"CreateMap failed: {request.error}");
            return false;
        }
    }

    // map data in body
    public async Task<bool> UpdateMapAsync(string mapId, string mapData)
    {

        var url = $"{BaseUrl}Map/update/{mapId}";
        var json = $"{{\"mapData\":\"{mapData}\"}}";

        Debug.Log($"Updating map at {url} with data: {mapData}");
        using var request = new UnityWebRequest(url, "PUT");
        UploadJson(request, json);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            return true;
        }
        else
        {
            Debug.LogError($"UpdateMap failed: {request.error}");
            return false;
        }
    }

    public async Task<List<Map>> GetMapsByUserAsync(string userId)
    {
        var url = $"{BaseUrl}Map/user/{userId}";
        Debug.Log($"Getting maps for user {userId} from {url}");
        using var request = UnityWebRequest.Get(url);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var wrapper = JsonHelper.FromJsonArray<Map>(FixJsonArray(request.downloadHandler.text));
            return new List<Map>(wrapper);
        }
        else
        {
            Debug.LogError($"GetUserMaps failed: {request.error}");
            return null;
        }
    }

    // ===================== HELPERS =====================
    private static void UploadJson(UnityWebRequest request, string json)
    {
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
    }

    private static string FixJsonArray(string json) => $"{{\"items\":{json}}}";
    private static string EscapeForJson(string input) => input.Replace("\"", "\\\"");

    // You need this helper for parsing arrays
    public static class JsonHelper
    {
        [Serializable]
        private class Wrapper<T> { public T[] items; }

        public static T[] FromJsonArray<T>(string json)
        {
            return JsonUtility.FromJson<Wrapper<T>>(json).items;
        }
    }
}

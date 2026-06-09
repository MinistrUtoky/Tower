using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AbstractReleasable<T>
{
    private AsyncOperationHandle _handle;
    public void AttachAddressableHandle(AsyncOperationHandle handle)
    {
        if (handle.IsValid()) _handle = handle;
    }
    public void ReleaseResources()
    {
        if (_handle.IsValid())
        {
            Addressables.Release(_handle);
            _handle = default;
        }
    }
    public static async Task<T> LoadAsync(string key)
    {
        var handle = Addressables.LoadAssetAsync<T>(key);
        await handle.Task; 
        if (handle.Status == AsyncOperationStatus.Succeeded)
            return handle.Result;

        Debug.LogError($"Failed to load: {key}");
        return default;
    }

    public static void ReleaseAsset(AsyncOperationHandle handle)
    {
        if (handle.IsValid()) Addressables.Release(handle);
    }
}

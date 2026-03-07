using UnityEngine;

public abstract class AbstractSaveable<T> : MonoBehaviour where T : MonoBehaviour
{
    private static bool m_ShuttingDown = false;
    private static object m_Lock = new object();
    private static T m_Instance;

    public static T Instance
    {
        get
        {
            if (m_ShuttingDown) {
                Debug.LogWarning(
                    string.Format($"[Singleton] Instance '{0}' already destroyed. Returning null.", typeof(T)));
                return null;
            }
            lock (m_Lock) 
            {
                if (m_Instance == null)
                {
                    m_Instance = (T)FindAnyObjectByType(typeof(T));
                    if (m_Instance == null)
                    {
                        var singletonObject = new GameObject();
                        m_Instance = singletonObject.AddComponent<T>();
                        DontDestroyOnLoad(singletonObject);
                    }
                }
                return m_Instance;
            }
        }
    }
    private void OnApplicationQuit() => m_ShuttingDown = true;   
    private void OnDestroy() { if (m_Instance == this) m_ShuttingDown = true; }
    protected void Save(string name, string value)
    {
        Debug.Log(name + " is set to " + value);
        PlayerPrefs.SetString(name, value);
        PlayerPrefs.Save();
    }
    protected int GetInt(string name)
    {
        int value = 0;
        int.TryParse(PlayerPrefs.GetString(name), out value);
        return value;
    }
    protected float GetFloat(string name)
    {
        float value = 0;
        float.TryParse(PlayerPrefs.GetString(name), out value);
        return value;
    }
    protected string Get(string name) => PlayerPrefs.GetString(name);
}

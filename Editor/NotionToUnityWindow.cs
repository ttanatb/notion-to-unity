using UnityEngine;
using UnityEditor;
using UnityEngine.Assertions;

// ReSharper disable once CheckNamespace
namespace NotionToUnity.Editor
{
    public class ConfigWindow : EditorWindow
    {
        private const string EditorPrefsKey = "NotionToUnityEditorConfigWindow";
        [SerializeField]
        private string m_apiKey;
        [SerializeField]
        private string m_version = "2022-02-22";

        [SerializeField]
        private string m_namespace = "Database";
        [SerializeField]
        private string m_editorScriptPath = "Scripts/Editor/Database";
        [SerializeField]
        private string m_structPath = "Scripts/Database";
        [SerializeField]
        private string m_soFormat = "{0}Database";

        [SerializeField]
        private bool m_logDebug = false;
        [SerializeField]
        private bool m_useTestJson = false;

        // TODO: support multiple databases more elegantly?
        [SerializeField]
        private string m_dbId;

        private string m_soTypeName = "ItemDatabase";
        private ScriptableObject m_librarySo;

        [MenuItem("Tools/Notion To Unity")]
        private static void Init()
        {
            var window = (ConfigWindow)GetWindow(typeof(ConfigWindow));
            window.titleContent = new GUIContent("Notion To Unity");
            window.Show();
        }
        private void OnEnable()
        {
            string data = EditorPrefs.GetString(EditorPrefsKey, JsonUtility.ToJson(this, false));
            JsonUtility.FromJsonOverwrite(data, this);
        }

        private void OnDisable()
        {
            string data = JsonUtility.ToJson(this, false);
            EditorPrefs.SetString(EditorPrefsKey, data);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Notion To Unity Config", EditorStyles.boldLabel);
            m_apiKey = EditorGUILayout.PasswordField("API Key", m_apiKey);
            m_version = EditorGUILayout.TextField("Version", m_version);

            m_namespace = EditorGUILayout.TextField("Namespace", m_namespace);
            m_editorScriptPath = EditorGUILayout.TextField("Editor Script Path", m_editorScriptPath);
            m_structPath = EditorGUILayout.TextField("Struct Path", m_structPath);
            m_soFormat = EditorGUILayout.TextField("Scriptable Object Naming Format", m_soFormat);

            // TODO: support multiple databases more elegantly?
            m_dbId = EditorGUILayout.TextField("Database ID", m_dbId);
            m_librarySo = (ScriptableObject)EditorGUILayout.ObjectField(new GUIContent("Library SO"), m_librarySo,
                typeof(ScriptableObject), false);


            if (GUILayout.Button("Generate Code"))
            {
                DatabaseCodeGenerator.Init(m_namespace, m_editorScriptPath, m_structPath, m_soFormat);
                if (m_useTestJson)
                {
                    DatabaseCodeGenerator.Process(TestData.Values.DbProperty, out m_soTypeName);
                }
                else
                {
                    WebRequestHandler.GetDatabaseProperties(m_dbId, m_apiKey, m_version, json => {
                        if (m_logDebug) Debug.Log(json);
                        DatabaseCodeGenerator.Process(json, out m_soTypeName);
                    }, () => {
                        Debug.LogError("Error fetching database.");
                    });
                }
            }
            if (GUILayout.Button("Fill Database"))
            {
                var soInstances = ClassExtensions.GetAllInstances(m_soTypeName);
                if (soInstances.Length > 1)
                {
                    Debug.LogError($"Found multiple {m_soTypeName} databases (too many ScriptableObject instances)");
                }

                Assert.IsFalse(soInstances.Length == 0,
                    $"Missing instance of ScriptableObject. As a workaround, please create the" +
                    $"{m_soTypeName} ScriptableObject manually.");
                m_librarySo = soInstances[0];

                if (m_useTestJson)
                {
                    DatabaseItemGenerator.Process(TestData.Values.DbResult, m_librarySo);
                }
                else
                {
                    WebRequestHandler.QueryDatabase(m_dbId, m_apiKey, m_version, json => {
                        if (m_logDebug) Debug.Log(json);
                        DatabaseItemGenerator.Process(json, m_librarySo);
                    }, () => {
                        Debug.LogError("Error querying database.");
                    });
                }
            }

            m_logDebug = EditorGUILayout.Toggle("Debug Log", m_logDebug);
            m_useTestJson = EditorGUILayout.Toggle("Use Test JSON", m_useTestJson);
        }
    }
}




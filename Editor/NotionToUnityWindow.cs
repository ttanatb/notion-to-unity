using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.Assertions;
using NotionToUnity.DataTypes;
using NotionToUnity.JsonProcessor;
using NotionToUnity.JsonProcessor.Types;
using NotionToUnity.Networking;
using NotionToUnity.Utils.Unity;

// ReSharper disable once CheckNamespace
namespace NotionToUnity.Editor
{
    public class ConfigWindow : EditorWindow
    {
        // For serializing the values in the window.
        private const string EditorPrefsKey = "NotionToUnityEditorConfigWindow";
        [SerializeField]
        private string m_apiKey;
        private readonly NotionApi.Version m_version = NotionApi.Version.Feb2022;
        
        [SerializeField]
        private NamingConvention m_namingConvention = new NamingConvention() {
            Namespace = "Database",
            EditorScriptPath = "Editor/Scripts/Database",
            StructPath = "Scripts/Database",
            ResourceClassNameFormat = "{0}Database",
            ResourceFilePath = "Data",
            IndentCount = 4,
            IndentType = IndentConfig.IndentType.Space,
        };

        [SerializeField]
        private bool m_logDebug = false;

        [SerializeField]
        private List<NotionDatabaseDefinition> m_databases;
        private ReorderableList m_databaseList;

        [SerializeField]
        private GeneratedResourceDef[] m_generatedSos = null;
        
        #region Unity Function/Messages
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
            m_databaseList = new ReorderableList(m_databases, typeof(NotionDatabaseDefinition))
            {
                drawElementCallback = DrawListItems,
                drawHeaderCallback = (rect) => {
                    EditorGUI.LabelField(rect, "Notion Databases");
                }
            };
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

            m_namingConvention.Namespace = EditorGUILayout.TextField("Namespace", m_namingConvention.Namespace);
            m_namingConvention.EditorScriptPath =
                EditorGUILayout.TextField("Editor Script Path", m_namingConvention.EditorScriptPath);
            m_namingConvention.StructPath = EditorGUILayout.TextField("Struct Path", m_namingConvention.StructPath);
            m_namingConvention.ResourceClassNameFormat =
                EditorGUILayout.TextField("Scriptable Object Naming Format", m_namingConvention.ResourceClassNameFormat);
            m_namingConvention.ResourceFilePath =
                EditorGUILayout.TextField("Scriptable Object Asset Path", m_namingConvention.ResourceFilePath);

            m_databaseList.DoLayoutList();

            if (GUILayout.Button("Generate Code"))
            {
                GenerateCode();
            }

            m_logDebug = EditorGUILayout.Toggle("Debug Log", m_logDebug);
        }
        #endregion

        private void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = (NotionDatabaseDefinition)m_databaseList.list[index];

            EditorGUI.LabelField(new Rect(rect.x, rect.y, 20, EditorGUIUtility.singleLineHeight), "ID");
            element.Id = EditorGUI.TextField(
                new Rect(rect.x + 20, rect.y, 230, EditorGUIUtility.singleLineHeight),
                element.Id
            );

            EditorGUI.LabelField(new Rect(rect.x + 270, rect.y, 35, EditorGUIUtility.singleLineHeight), "Type");
            element.Type = (NotionDatabaseDefinition.DatabaseType)EditorGUI.EnumPopup(
                new Rect(rect.x + 305, rect.y, 90, EditorGUIUtility.singleLineHeight),
                element.Type);

            m_databases[index] = element;
        }


        /// <summary>
        /// Fetches the JSON from Notion
        /// </summary>
        private void GenerateCode()
        {
            NotionApi.GetDbProperties(
                m_databases, m_apiKey, m_version, dbPropJsons => {
                    foreach (var json in dbPropJsons)
                        if (m_logDebug) Debug.Log(json);
                    
                    NotionApi.GetDatabaseContents(
                        m_databases, m_apiKey, m_version, dbContentsJsons => {
                            foreach (var json in dbContentsJsons)
                                if (m_logDebug) Debug.Log(json);

                            GenerateCodePartTwo(dbPropJsons, dbContentsJsons);
                        }, () => {
                            Debug.LogError("Error querying database content.");
                        });
                }, () => {
                    Debug.LogError("Error fetching database.");
                });
        }

        /// <summary>
        /// Generates the code and propagate the resource fields
        /// </summary>
        /// <param name="dbPropertyJsons">Dictionary of database ID to table properties</param>
        /// <param name="dbContentsJsons">Dictionary of database ID to table contents</param>
        private void GenerateCodePartTwo(
            Dictionary<string, JObject> dbPropertyJsons, Dictionary<string, List<JObject>> dbContentsJsons)
        {
            var codeGenerator = new CodeGenerator(m_namingConvention);
            m_generatedSos = codeGenerator.Process(m_databases, dbPropertyJsons, dbContentsJsons);
            AssetDatabase.Refresh();

            foreach (var generatedSo in m_generatedSos)
            {
                var soInstances = generatedSo.ResourceTypeName.GetAllInstances();
                if (soInstances.Length > 1)
                {
                    Debug.LogError(
                        $"Found multiple {generatedSo.ResourceTypeName} databases (too many ScriptableObject instances)");
                }

                if (soInstances.Length == 0)
                {
                    Debug.LogError("Please press the button a second time, asset database needs to be refreshed.");
                    return;
                }

                Assert.IsFalse(soInstances.Length == 0,
                    "Missing instance of ScriptableObject. As a workaround, please wait until after asset " +
                    "database is refreshed, then press the button again.");
                var librarySo = soInstances[0];

                ResourceContentPropagator.Process(dbContentsJsons[generatedSo.DbId], librarySo);
                Debug.Log("Process complete: " + librarySo);
            }
        }
    }
}




using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
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
        private NamingConvention m_namingConvention = new NamingConvention() {
            Namespace = "Database",
            EditorScriptPath = "Editor/Scripts/Database",
            StructPath = "Scripts/Database",
            SoFormat = "{0}Database",
            SoPath = "Data",
        };

        [SerializeField]
        private bool m_logDebug = false;
        [SerializeField]
        private bool m_useTestJson = false;

        [SerializeField]
        private List<NotionDatabaseDefinition> m_databases;
        private ReorderableList m_databaseList;

        [SerializeField]
        private GeneratedSo[] m_generatedSos = null;

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

        private void OnDisable()
        {
            string data = JsonUtility.ToJson(this, false);
            EditorPrefs.SetString(EditorPrefsKey, data);
        }

        private void GenerateCode()
        {
            CodeGenerator.Init(m_namingConvention);
            var databaseProperties = new List<JObject>();
            var databaseContents = new Dictionary<string, JObject>();

            WebRequestHandler.GetDatabaseProperties(
                m_databases, m_apiKey, m_version, jsons => {
                    for (int i = 0; i < jsons.Count(); i++)
                        if (m_logDebug) Debug.Log(jsons[i]);
                    databaseProperties = jsons;
                    GenerateCodePartTwo(databaseProperties, databaseContents);
                }, () => {
                    Debug.LogError("Error fetching database.");
                });

            WebRequestHandler.GetDatabaseContents(
                m_databases, m_apiKey, m_version, jsons => {
                    foreach (var json in jsons)
                        if (m_logDebug) Debug.Log(json);

                    databaseContents = jsons;
                    GenerateCodePartTwo(databaseProperties, databaseContents);
                }, () => {
                    Debug.LogError("Error querying database content.");
                });
        }

        private void GenerateCodePartTwo(List<JObject> dbPropertyJsons, Dictionary<string, JObject> dbContentsJsons)
        {
            if (dbPropertyJsons.Count != m_databases.Count || dbContentsJsons.Count != m_databases.Count)
                return;

            var generatedSos = CodeGenerator.Process(m_databases, dbPropertyJsons, dbContentsJsons);
            m_generatedSos = generatedSos.ToArray();
            AssetDatabase.Refresh();

            foreach (var generatedSo in m_generatedSos)
            {
                var soInstances = ClassExtensions.GetAllInstances(generatedSo.SoTypeName);
                if (soInstances.Length > 1)
                {
                    Debug.LogError(
                        $"Found multiple {generatedSo.SoTypeName} databases (too many ScriptableObject instances)");
                }

                Assert.IsFalse(soInstances.Length == 0,
                    $"Missing instance of ScriptableObject. As a workaround, please create the" +
                    $"{generatedSo.SoTypeName} ScriptableObject manually.");
                var librarySo = soInstances[0];

                FieldPropagator.Process(dbContentsJsons[generatedSo.DbId], librarySo);
                Debug.Log("Process complete: " + librarySo);
            }
        }

        private void FillSoFields()
        {
            // WebRequestHandler.GetDatabaseContents(
            //     m_databases, m_apiKey, m_version, jsons => {
            //         for (int i = 0; i < jsons.Count(); i++)
            //             if (m_logDebug) Debug.Log(jsons[i]);
            //
            //         foreach (var generatedSo in m_generatedSos)
            //         {
            //             var soInstances = ClassExtensions.GetAllInstances(generatedSo.SoTypeName);
            //             if (soInstances.Length > 1)
            //             {
            //                 Debug.LogError(
            //                     $"Found multiple {generatedSo.SoTypeName} databases (too many ScriptableObject instances)");
            //             }
            //
            //             Assert.IsFalse(soInstances.Length == 0,
            //                 $"Missing instance of ScriptableObject. As a workaround, please create the" +
            //                 $"{generatedSo.SoTypeName} ScriptableObject manually.");
            //             var librarySo = soInstances[0];
            //
            //             FieldPropogator.Process(TestData.Values.DbResult, librarySo);
            //         }
            //     }, () => {
            //         Debug.LogError("Error querying database content.");
            //     });
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Notion To Unity Config", EditorStyles.boldLabel);
            m_apiKey = EditorGUILayout.PasswordField("API Key", m_apiKey);
            m_version = EditorGUILayout.TextField("Version", m_version);

            m_namingConvention.Namespace = EditorGUILayout.TextField("Namespace", m_namingConvention.Namespace);
            m_namingConvention.EditorScriptPath =
                EditorGUILayout.TextField("Editor Script Path", m_namingConvention.EditorScriptPath);
            m_namingConvention.StructPath = EditorGUILayout.TextField("Struct Path", m_namingConvention.StructPath);
            m_namingConvention.SoFormat =
                EditorGUILayout.TextField("Scriptable Object Naming Format", m_namingConvention.SoFormat);
            m_namingConvention.SoPath =
                EditorGUILayout.TextField("Scriptable Object Asset Path", m_namingConvention.SoPath);

            m_databaseList.DoLayoutList();

            if (GUILayout.Button("Generate Code"))
            {
                GenerateCode();
            }

            if (GUILayout.Button("Fill Database"))
            {
                FillSoFields();
            }

            m_logDebug = EditorGUILayout.Toggle("Debug Log", m_logDebug);
            m_useTestJson = EditorGUILayout.Toggle("Use Test JSON", m_useTestJson);
        }
    }
}




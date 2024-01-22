using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SoftBody2D.Editor
{
    public static class SoftBodyEditorTools
    {
        public static void DrawHorizontalLine()
        {
            GUILayout.Space(10f);
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2f), Color.gray);
            GUILayout.Space(10f);
        }
        
        public static void MarkAsDirty(Object[] targets)
        {
            if (!Application.isPlaying)
            {
                foreach (var script in targets)
                {
                    var component = script as Component;
                    if (component != null)
                    {
                        EditorUtility.SetDirty(component);
                        EditorSceneManager.MarkSceneDirty(component.gameObject.scene);
                    }
                }
            }
        }

        public static bool IsPrefabsSupported()
        {
#if UNITY_2018_4_OR_NEWER
            return true;
#else
            return false;
#endif
        }
        
        public static bool IsGameObjectPrefab(GameObject gameObject)
        {
#if UNITY_2018_3_OR_NEWER
            var isPrefab = PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject) != null;
#else
            var isPrefab = PrefabUtility.GetPrefabObject(gameObject) != null;
#endif
            return isPrefab;
        }

        public static void RecordObject(Object currentObject, bool recordHierarchyUndo = true)
        {
            Undo.RecordObject(currentObject, "Update SoftObject");
            if (recordHierarchyUndo)
            {
                RecordHierarchyUndo(currentObject);
            }
        }

        public static void RecordHierarchyUndo(Object currentObject)
        {
            var softObject = currentObject as SoftObject;
            if (!ReferenceEquals(softObject, null) && softObject != null && softObject.Joints != null)
            {
                Undo.RegisterFullObjectHierarchyUndo(softObject, "Update SoftObject");
            }
        }
    }
}
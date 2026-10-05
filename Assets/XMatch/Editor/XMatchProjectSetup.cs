using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace XMatch.Editor
{
    [InitializeOnLoad]
    public static class XMatchProjectSetup
    {
        private const string SceneFolder =
            "Assets/Scenes";
        private const string PrototypeScene =
            "Assets/Scenes/XMatchPrototype.unity";

        static XMatchProjectSetup()
        {
            EditorApplication.delayCall +=
                EnsurePrototypeProject;
        }

        [MenuItem(
            "X MATCH/Setup Prototype Project")]
        public static void EnsurePrototypeProject()
        {
            PlayerSettings.productName =
                "X MATCH";
            PlayerSettings.defaultInterfaceOrientation =
                UIOrientation.Portrait;

            if (!AssetDatabase.IsValidFolder(
                    SceneFolder))
            {
                AssetDatabase.CreateFolder(
                    "Assets",
                    "Scenes");
            }

            SceneAsset existing =
                AssetDatabase.LoadAssetAtPath<SceneAsset>(
                    PrototypeScene);

            if (existing == null)
            {
                Scene scene =
                    EditorSceneManager.NewScene(
                        NewSceneSetup.EmptyScene,
                        NewSceneMode.Single);

                EditorSceneManager.SaveScene(
                    scene,
                    PrototypeScene);
            }

            EditorBuildSettings.scenes =
                new[]
                {
                    new EditorBuildSettingsScene(
                        PrototypeScene,
                        true)
                };

            Scene active =
                SceneManager.GetActiveScene();

            if (active.path != PrototypeScene)
            {
                EditorSceneManager.OpenScene(
                    PrototypeScene,
                    OpenSceneMode.Single);
            }

            AssetDatabase.SaveAssets();
        }
    }
}

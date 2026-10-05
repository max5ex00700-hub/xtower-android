using UnityEngine;

namespace XMatch.Puzzle
{
    public static class XMatchRuntimeBootstrap
    {
        [RuntimeInitializeOnLoadMethod(
            RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (Object.FindFirstObjectByType<PuzzleBoardController>() != null)
            {
                return;
            }

            Application.targetFrameRate = 60;

            var root = new GameObject(
                "X MATCH Prototype");
            Object.DontDestroyOnLoad(root);

            root.AddComponent<PuzzleBoardController>();
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

namespace InteractionEffects
{
    public class LoadScene : InteractionEffect
    {
        [SerializeField] private string sceneName;


        public override bool CheckSetup()
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("Scene to load not set on InteractionEffect! Please set it.");
            }
            else if (!DoesSceneExist(sceneName))
            {
                Debug.LogError("Specified scene to load on InteractionEffect is not added to the BuildSettings!");
            }

            return !string.IsNullOrEmpty(sceneName) && DoesSceneExist(sceneName);
        }

        public override void Trigger()
        {
            SceneManager.LoadScene(sceneName);
        }
    
        /// <summary>
        /// Returns true if the scene 'name' exists and is in your Build settings, false otherwise
        /// </summary>
        private bool DoesSceneExist(string name)
        {
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                var lastSlash = scenePath.LastIndexOf("/");
                var sceneName = scenePath.Substring(lastSlash + 1, scenePath.LastIndexOf(".") - lastSlash - 1);

                if (string.Compare(name, sceneName, true) == 0)
                    return true;
            }

            return false;
        }
    }
}
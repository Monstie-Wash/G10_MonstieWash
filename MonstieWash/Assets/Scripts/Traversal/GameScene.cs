using UnityEngine;

[CreateAssetMenu(fileName = "GameScene", menuName = "ScriptableObjects/GameScene")]
public class GameScene : ScriptableObject
{
    [SerializeField] private string sceneName;
    [SerializeField] private Sprite sceneThumb;
    [HideInInspector] public string SceneName { get { return sceneName; } }
    public Sprite SceneThumb { get { return sceneThumb; } }
}

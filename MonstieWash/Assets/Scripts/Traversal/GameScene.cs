using UnityEngine;

[CreateAssetMenu(fileName = "GameScene", menuName = "ScriptableObjects/GameScene")]
public class GameScene : ScriptableObject
{
    [SerializeField] private string sceneName;
    [HideInInspector] public string SceneName { get { return sceneName; } }
}

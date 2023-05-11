using UnityEngine;

[CreateAssetMenu(fileName = "New LevelInfo", menuName = "SO/LevelData")]
public class LevelInfo_SO : ScriptableObject
{

    [Header("Level Data")] 
    public int levelNum;
    public int levelSceneNum;
    public int levelTotalScore;
    public string levelBGM;
}


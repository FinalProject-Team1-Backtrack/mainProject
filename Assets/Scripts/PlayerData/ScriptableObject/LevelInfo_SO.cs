namespace PlayerData.ScriptableObject
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New LevelInfo", menuName = "SO/Data")]
    public class LevelInfo_SO : ScriptableObject
    {

        [Header("Level Data")] 
        public int levelNum;
        public int levelSceneNum;

        public string levelBGM;
    }

}
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class SavePointManager : Singleton<SavePointManager>
{
    public string saveSceneName;
    public Vector3 savePointVector3;
    public Vector3 saveRotationVector3;
    public string saveBGMName;
    public float saveBGMTime;
    public int saveScores;

    public void SetSavePoint(string sceneName, Vector3 pointVector,Vector3 rotation,  string bgmName, float bgmTime,int scores)
    {
        saveSceneName = sceneName;
        savePointVector3 = pointVector;
        saveRotationVector3 = rotation;
        saveBGMName = bgmName;
        saveBGMTime = bgmTime;
        saveScores = scores;
    }
}

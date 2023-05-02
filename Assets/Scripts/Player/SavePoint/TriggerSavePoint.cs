using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Player;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerSavePoint : TriggerBase
{
    protected override void enterEvent(Collider c)
    {
        string sceneName = SceneManager.GetActiveScene().name;
        Vector3 point = this.transform.position;
        Vector3 rotation = this.transform.rotation.eulerAngles;
        string bgmName = AudioManager.Instance.GetMusicIsPlaying();
        float time = AudioManager.Instance.GetMusicTime(bgmName);
        int scores = ScoreManager.Instance.CurrentScoreInLevel;
        SavePointManager.Instance.SetSavePoint(sceneName,
            point,
            rotation,
            bgmName,
            time,
            scores);
    }
}

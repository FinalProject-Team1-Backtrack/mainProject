using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class SettingMenuManager : MonoBehaviour
{
    #region 通用
    
    private void Awake()
    {
        // Resolution
        _resolution = transform.Find("Resolution").gameObject;
        Resolutions = Screen.resolutions;

        // Volume
        _volume = transform.Find("Volume").gameObject;
        _audioMixer = Resources.Load<AudioMixer>("Audio/Mixer/GameMixer");

        // Brightness
        _brightness = transform.Find("Brightness").gameObject;
    }

    private void Start()
    {
        ChangeVolume(DEFAULT_VOLUME);
        ChangeBrightness(DEFAULT_BRIGHTNESS);
        gameObject.SetActive(false);
    }

    public void UpdateCircleUI(Transform ui, float perc)
    {
        GameObject circleBar = ui.Find("CircleSlideBar/CircleFill")?.gameObject;
        circleBar.GetComponent<Image>().fillAmount = Mathf.Clamp01(perc);
    }

    public void UpdateNumberUI(Transform ui, float text)
    {
        GameObject number = ui.Find("AdjustNumUI/Number")?.gameObject;
        number.GetComponent<TMPro.TMP_Text>().text = $"{text}%";
    }

    private float ReMap(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
    }

    #endregion 

    #region 调分辨率

    public Resolution[] Resolutions;
    private GameObject _resolution;
    private int index = 0;
    [SerializeField] private int DEFAULT_RESOLUTION_IDX = 1;

    public string[] ResolutionStrings()
    {
        string[] sArr = new string[Resolutions.Length];
        for (var i = 0; i < Resolutions.Length; i++)
        {
            sArr[i] =  i + ": " + Resolutions[i].ToString();
        }

        return sArr;
    }

    public void SetFullScreen(bool isFullScreen)
    {
        if (isFullScreen)
        {
            Screen.fullScreen = true;
            Debug.Log("全屏");
        }
        else
        {
            Screen.fullScreen = false;
            Debug.Log("窗口化");
        }
    }

    public void SetResolution(int listIndex)
    {
        Resolution resolution = Resolutions[listIndex];
        Screen.SetResolution(resolution.width,resolution.height,Screen.fullScreen,resolution.refreshRate);
        Debug.Log("当前分辨率设置为 " + resolution);
    }

    //void OnGUI()
    //{
    //    if (GUI.Button(new Rect(10, 10, 100, 50), "change resolution++"))
    //    {
    //        index++;
    //        SetResolution(index);
    //    }

    //    if (GUI.Button(new Rect(200, 10, 100, 50), "change resolution--"))
    //    {
    //        index--;
    //        SetResolution(index);
    //    }

    //    if (GUI.Button(new Rect(400, 10, 100, 50), "change fullscreen"))
    //    {
    //        if (Screen.fullScreen)
    //        {
    //            SetFullScreen(false);
    //        }
    //        else
    //        {
    //            SetFullScreen(true);
    //        }
    //    }
    //    if (GUI.Button(new Rect(600, 10, 100, 50), "show all resolution"))
    //    {
    //        string[] s = ResolutionStrings();
    //        foreach (var s1 in s)
    //        {
    //            Debug.Log(s1);
    //        }
    //    }
    //}

    #endregion

    #region 调音量
    
    private GameObject _volume;
    private AudioMixer _audioMixer;
    private float _masterVolume = 0f;
    [SerializeField] private int DEFAULT_VOLUME = 70;

    public void ChangeVolume(float volume)
    {
        if (_audioMixer == null)
        {
            Debug.LogError($"Audio Mixer is Missing!");
            return;
        }

        if (_masterVolume + volume < 0 || _masterVolume + volume > 100)
        {
            Debug.LogWarning("Volume Reaches Limits!");
            return;
        }

        _masterVolume += volume;
        _audioMixer.SetFloat("MasterVolume", ReMap(_masterVolume, 0f, 100f, -80f, 20f));
        UpdateCircleUI(_volume.transform, _masterVolume / 100f);
        UpdateNumberUI(_volume.transform, _masterVolume);
    }

    #endregion

    #region 调亮度

    private GameObject _brightness;
    private float _masterBrightness = 0f;
    [SerializeField] private int DEFAULT_BRIGHTNESS = 60;

    public void ChangeBrightness(float brightness)
    {
        if (_masterBrightness + brightness < 0 || _masterBrightness + brightness > 100)
        {
            Debug.LogWarning("Brightness Reaches Limits!");
            return;
        }

        _masterBrightness += brightness;
        UpdateCircleUI(_brightness.transform, _masterBrightness / 100f);
        UpdateNumberUI(_brightness.transform, _masterBrightness);
    }

    #endregion
}


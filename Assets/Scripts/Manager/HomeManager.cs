using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeManager : MonoBehaviour
{
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject settingPanel;

    public void OpenCreditsPanel() => creditsPanel.SetActive(true);
    public void CloseCreditsPanel() => creditsPanel.SetActive(false);
    public void OpenSettingPanel() => settingPanel.SetActive(true);
    public void CloseSettingPanel() => settingPanel.SetActive(false);
}

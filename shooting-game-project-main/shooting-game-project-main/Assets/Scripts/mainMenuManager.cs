using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class mainMenuManager : MonoBehaviour
{
    public playerSettings playerSettings;
    public Slider sensX;
    public Slider sensY;
    public Slider volumePercent;
    [SerializeField] TMP_Text horizontalSensText;
    [SerializeField] TMP_Text verticalSensText;

    private int sensXNumber;
    private int sensYNumber;
    private int volumePercentNumber;
    
    void Start()
    {
        sensX.value = playerSettings.horizontalSensitivity;
        sensY.value = playerSettings.verticalSensitivity;
        volumePercent.value = playerSettings.volumePercent;
    }

    void Update()
    {
        sensXNumber = (int)sensX.value;
        sensYNumber = (int)sensY.value;
        volumePercentNumber = (int)volumePercent.value;

        playerSettings.horizontalSensitivity = sensXNumber;
        playerSettings.verticalSensitivity = sensYNumber;
        playerSettings.volumePercent = volumePercentNumber;

        horizontalSensText.text = sensXNumber.ToString();
        verticalSensText.text = sensYNumber.ToString();

    }
}

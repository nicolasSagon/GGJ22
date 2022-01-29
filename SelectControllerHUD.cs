using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectControllerHUD : MonoBehaviour
{

    private GameObject fightText;
    private GameObject p1Controller;
    private GameObject p2Controller;

    // Start is called before the first frame update
    void Start()
    {
        fightText = GameObject.Find("Fight");
        p1Controller = GameObject.Find("P1");
        p2Controller = GameObject.Find("P2");

        fightText.SetActive(false);
    }

    public void setFirstPlayerController(CustomInputDevice customInputDevice) {
        var p1Text = p1Controller.GetComponent<TextMeshProUGUI>();
        selectController(customInputDevice.InputType, p1Text);
    }

    public void setSecondPlayerController(CustomInputDevice customInputDevice) {
        var p2Text = p2Controller.GetComponent<TextMeshProUGUI>();
        selectController(customInputDevice.InputType, p2Text);
    }

    public void displayFightText(string text) {
        fightText.SetActive(true);
        var fightTextPro = fightText.GetComponent<TextMeshProUGUI>();
        fightTextPro.text = text;
    }

    public void hideFightText(string text) {
        fightText.SetActive(false);
    }

    private void selectController(CustomInputDevice.InputTypeEnum inputType, TextMeshProUGUI text) {
        switch(inputType) {
            case CustomInputDevice.InputTypeEnum.GAMEPAD:
                text.text = "Gamepad Selected";
                break;
            case CustomInputDevice.InputTypeEnum.KEYBOARD:
                text.text = "Keyboard Selected";
                break;
        }
    }

}

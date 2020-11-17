using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class HudPassive : MonoBehaviour
{
    [HideInInspector]
    public string passiveName;
    [HideInInspector]
    public Sprite passiveImage;
    [HideInInspector]
    public Text passiveText;
    int count = 1;

    public void InitPassive(string _abilityName, Sprite _abilityImage)
    {
        passiveName = _abilityName;
        passiveImage = _abilityImage;
    }

    public void IncreaseCount()
    {
        count++;
        passiveText.text = count.ToString();
    }
}

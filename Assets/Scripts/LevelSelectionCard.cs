using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionCard : MonoBehaviour {
    public TextMeshProUGUI title;
    public Image icon;
    public List<Image> stars;
    public Button button;

    private Color32 activeStarColor = new Color32(0xd6, 0xab, 0x00, 0xff);
    private Color32 inactiveStarColor = new Color32(0x3c, 0x3c, 0x3c, 0xff);

    public void Render(bool isActive, int starsActive) {
        title.color = isActive ? Color.white : inactiveStarColor;
        icon.color = isActive ? Color.white : inactiveStarColor;
        for (int i = 0; i < this.stars.Count; i++) {
            stars[i].color = (starsActive & (1 << i)) != 0 ? activeStarColor : inactiveStarColor;
        }
        button.interactable = isActive;
    }
}

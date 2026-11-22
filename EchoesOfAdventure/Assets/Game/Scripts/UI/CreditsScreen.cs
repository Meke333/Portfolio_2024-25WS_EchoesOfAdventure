using UnityEngine;
using UnityEngine.UI;

public class CreditsScreen : MonoBehaviour
{
    [Header("UI")]
    public Button backButton;
    public Button reneeButton;              //= 0
    public Button anastasiabackButton;      //= 1
    public Button maxbackButton;            //= 2
    public Button benjaminbackButton;       //= 3
    public Button mikebackButton;           //= 4
    public Button liambackButton;           //= 5
    public Button nilsbackButton;           //= 6
    public Button telasbackButton;          //= 7
    public GameObject mainMenu;

    public void OnEnable()
    {
        backButton.onClick.AddListener(Back);
        reneeButton.onClick.AddListener(() => { OpenURL(0); });
        anastasiabackButton.onClick.AddListener(() => { OpenURL(1); });
        maxbackButton.onClick.AddListener(() => { OpenURL(2); });
        benjaminbackButton.onClick.AddListener(() => { OpenURL(3); });
        mikebackButton.onClick.AddListener(() => { OpenURL(4); });
        liambackButton.onClick.AddListener(() => { OpenURL(5); });
        nilsbackButton.onClick.AddListener(() => { OpenURL(6); });
        telasbackButton.onClick.AddListener(() => { OpenURL(7); });
    }

    public void OnDisable()
    {
        backButton.onClick.RemoveListener(Back);
        reneeButton.onClick.RemoveListener(() => { OpenURL(0); });
        anastasiabackButton.onClick.RemoveListener(() => { OpenURL(1); });
        maxbackButton.onClick.RemoveListener(() => { OpenURL(2); });
        benjaminbackButton.onClick.RemoveListener(() => { OpenURL(3); });
        mikebackButton.onClick.RemoveListener(() => { OpenURL(4); });
        liambackButton.onClick.RemoveListener(() => { OpenURL(5); });
        nilsbackButton.onClick.RemoveListener(() => { OpenURL(6); });
        telasbackButton.onClick.RemoveListener(() => { OpenURL(7); });
    }

    public void Back()
    {
        mainMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    public void OpenURL(int index)
    {
        string url = "";

        switch (index)
        {
            case 0:
                url = "https://youtube.com/@ray_nee";
                break;
            case 1:
                url = "https://linktr.ee/a.schleicher";
                break;
            case 2:
                url = "https://linktr.ee/xaivu";
                break;
            case 3:
                url = "https://linktr.ee/clyfdir";
                break;
            case 4:
                url = "https://linktr.ee/meke333dev";
                break;
            case 5:
                url = "https://linktr.ee/liam_hermann";
                break;
            case 6:

                break;
            case 7:
                url = "https://linktr.ee/evermoonn";
                break;
        }


        if (url.Length <= 0)
            return;

        Application.OpenURL(url);
    }
}

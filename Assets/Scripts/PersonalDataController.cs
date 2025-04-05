using System.Security;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PersonalDataController : MonoBehaviour
{

    public TMP_InputField nameInput;
    public TMP_InputField ageInput;
    public TMP_Dropdown genderDropdown;
    public TMP_InputField classInput;
    public TMP_InputField regionInput;
    public Toggle concentToggle;

    private bool shouldSave = false;

    private void Start()
    {
        nameInput.text = Store.Instance.PersonalDataName;
        ageInput.text = Store.Instance.PersonalDataAge;
        genderDropdown.value = Store.Instance.PersonalDataGender;
        classInput.text = Store.Instance.PersonalDataClass;
        regionInput.text = Store.Instance.PersonalDataRegion;
        concentToggle.isOn = Store.Instance.PersonalDataConcent;
        shouldSave = false; // must be set after setting the values
    }

    public void SetName(string value)
    {
        shouldSave = true;
        Store.Instance.PersonalDataName = value;
    }

    public void SetAge(string value)
    {
        shouldSave = true;
        Store.Instance.PersonalDataAge = value;
    }

    public void SetGender(int value)
    {
        shouldSave = true;
        Store.Instance.PersonalDataGender = value;
    }

    public void SetClass(string value)
    {
        shouldSave = true;
        Store.Instance.PersonalDataClass = value;
    }

    public void SetRegion(string value)
    {
        shouldSave = true;
        Store.Instance.PersonalDataRegion = value;
    }

    public void SetPersonalDataConcent(bool value)
    {
        shouldSave = true;
        Store.Instance.PersonalDataConcent = value;
    }

    public void SaveAndExit()
    {
        if (shouldSave)
        {
            if (Store.Instance.PersonalDataConcent)
            {
                string jsonLog = "{" +
                    "\"message\":\"Saved settings with values and concent\"," +
                    "\"name\":\"" + SecurityElement.Escape(Store.Instance.PersonalDataName) + "\"," +
                    "\"age\":\"" + SecurityElement.Escape(Store.Instance.PersonalDataAge) + "\"," +
                    "\"gender\":\"" + Store.Instance.PersonalDataGender + "\"," +
                    "\"class\":\"" + SecurityElement.Escape(Store.Instance.PersonalDataClass) + "\"," +
                    "\"region\":\"" + SecurityElement.Escape(Store.Instance.PersonalDataRegion) + "\"," +
                    "\"concent\":\"" + Store.Instance.PersonalDataConcent + "\"" +
                "}";
                LoggingService.Log(LoggingService.LogCategory.Settings, jsonLog);
            }
            else
            {
                LoggingService.Log(LoggingService.LogCategory.Settings, "Saved settings, but user did not concent to save personal data");
            }
        }
        else
        {
            LoggingService.Log(LoggingService.LogCategory.Settings, "Exited settings without changes");
        }
        LoggingService.Log(LoggingService.LogCategory.Navigation, "Navigated to MainMenu");
        SceneManager.LoadScene("MainMenu");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum MigrationStatus { local, student, undocumented, working };

public class CharacterScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private const int prosperityMax = 33;

    private int prosperityCounter = 0;
    private string characterName;
    private string jobName;
    private bool isMale = false;
    private MigrationStatus migrationStatus;

    string GetMirationStatusText()
    {
        switch (migrationStatus)
        {
            case MigrationStatus.local: return "Local";
            case MigrationStatus.student: return "Student";
            case MigrationStatus.undocumented: return "Undocumented";
            case MigrationStatus.working: return "Working";
        }

        return "NONE";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EnableMask(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EnableMask(false);
    }

    void EnableMask(bool enabled)
    {
        transform.Find("TextMask").GetComponent<Image>().enabled = enabled;
        transform.Find("CharacterText").GetComponent<Text>().enabled = enabled;
    }

    void PolicyDeck()
    {
        //Draw
    }

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void InitCharacter(Character character)
    {
        characterName = character.name;
        jobName = character.roleName;
        migrationStatus = character.migrationStatus;
        isMale = character.isMale;
        transform.Find("NameText").GetComponent<Text>().text = characterName;
        transform.Find("CharacterText").GetComponent<Text>().text = "Status: " + GetMirationStatusText() + "\n\nRole: " + jobName;
        //transform.Find("ProsperityBar").Find("Slider").GetComponent<RectTransform>().anchoredPosition = new Vector2(prosperityOffset, 0.0f);
    }

    public void OnPolicyCard(PolicyCard card)
    {
        prosperityCounter += card.ammount;
        if(prosperityCounter > prosperityMax)
        {
            prosperityCounter = prosperityMax;
        }
        if (prosperityCounter < -prosperityMax)
        {
            //Player is out of the Game!
            prosperityCounter = -prosperityMax;
        }

        transform.Find("ProsperityBar").Find("Slider").GetComponent<RectTransform>().anchoredPosition = new Vector2((float)prosperityCounter, 0.0f);
    }
}

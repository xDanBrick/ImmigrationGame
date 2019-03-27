using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum MigrationStatus { local, student, undocumented, working };
public enum RoleType { skilled, unskilled, management, owner, student, unemployed, retired };

public class Job
{ 
    public Job(RoleType type, string name)
    {
        roleType = type;
        roleName = name;
    }
    public RoleType roleType;
    public string roleName;
}

public class CharacterScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private const int prosperityMax = 70;

    private int prosperityCounter = 0;
    private Job job;
    private string characterName;
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
        ChangeAlpha(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ChangeAlpha(false);
    }

    void ChangeAlpha(bool enabled)
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
        characterName = Characters.names[Random.Range(0, 12)];
        job = Characters.jobs[Random.Range(0, 12)];
        migrationStatus = (MigrationStatus)Random.Range(0, 4);
        transform.Find("NameText").GetComponent<Text>().text = characterName;
        transform.Find("CharacterText").GetComponent<Text>().text = "Status: " + GetMirationStatusText() + "\n\nRole: " + job.roleName;
        //transform.Find("ProsperityBar").Find("Slider").GetComponent<RectTransform>().anchoredPosition = new Vector2(prosperityOffset, 0.0f);
    }
	
	// Update is called once per frame
	void Update () {
		
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

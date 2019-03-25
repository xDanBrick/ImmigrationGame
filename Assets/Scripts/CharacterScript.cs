using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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


public class Characters
{
    public const int characterCount = 12;
    public static string[] names = new string[characterCount] { "Leanne", "Jim", "John", "Steve", "Chris", "Annie", "Will", "Steph", "Karen", "Ellie", "Ommar", "Alanah" };
    public static Job[] jobs = new Job[characterCount] {
        new Job(RoleType.owner, "Buiness Owner"), new Job(RoleType.management, "Buiness Manager"), new Job(RoleType.unskilled, "Buisness Worker"), new Job(RoleType.skilled, "Doctor"),
        new Job(RoleType.skilled, "Nurse"), new Job(RoleType.skilled, "Lawyer"), new Job(RoleType.skilled, "Judge"), new Job(RoleType.unskilled, "Paralegal"),
        new Job(RoleType.retired, "Retired"), new Job(RoleType.unemployed, "Unemployed"), new Job(RoleType.student, "Student"), new Job(RoleType.owner, "Shop Owner")
    };
}

public class CharacterScript : MonoBehaviour {

    private const int prosperityStart = 50;
    private const int prosperityMax = 100;
    private const float prosperityOffset = 70.0f;

    private int prosperityCounter = prosperityStart;
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

    // Use this for initialization
    void Start () {
        characterName = Characters.names[Random.Range(0, 12)];
        job = Characters.jobs[Random.Range(0, 12)];
        migrationStatus = (MigrationStatus)Random.Range(0, 4);
        transform.Find("NameText").GetComponent<Text>().text = "Name: " + characterName;
        transform.Find("MigrationText").GetComponent<Text>().text = "Migration Status: " + GetMirationStatusText();
        transform.Find("RoleText").GetComponent<Text>().text = "Role: " + job.roleName;
        transform.Find("ProsperityBar").Find("Slider").GetComponent<RectTransform>().anchoredPosition = new Vector2(prosperityOffset, 0.0f);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnPolicyCard(PolicyCard card)
    {

    }
}

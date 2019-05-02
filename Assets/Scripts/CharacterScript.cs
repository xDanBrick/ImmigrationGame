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
    private JobTier jobTier;
    private string proffession;
    private string relationshipName;
    private CharacterScript relationshipScript;

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
        relationshipName = character.relationshipName;
        jobName = character.roleName;
        migrationStatus = character.migrationStatus;
        isMale = character.isMale;
        transform.Find("NameText").GetComponent<Text>().text = characterName;
        transform.Find("CharacterText").GetComponent<Text>().text = "Status: " + GetMirationStatusText() + "\n\nRole: " + jobName + "\n\nRelationship: " + relationshipName;

        string resourceName = "WRONG";

        if(jobName == "Buisness Manager" || jobName == "Buisness Worker" || jobName == "Buisness Owner")
        {
            resourceName = "Manager";
        }
        else if(jobName == "Paralegal" || jobName == "Lawyer" || jobName == "Judge")
        {
            resourceName = "Lawyer";
        }
        else if (jobName == "Doctor" || jobName == "Nurse")
        {
            resourceName = "Doctor";
        }
        else if (jobName == "Student")
        {
            resourceName = "Student";
        }
        else if(jobName == "Retired" || jobName == "Unemployed")
        {
            resourceName = "None";
        }
        else if (jobName == "Shop Owner" || jobName == "Retail Worker")
        {
            resourceName = "Retail";
        }
        else if (jobName == "Plumber")
        {
            resourceName = "Overall";
        }
        else if (jobName == "Police")
        {
            resourceName = "Police";
        }

        var sprite = Resources.Load<Sprite>(isMale ? "idmale" : "idfemale");

        transform.Find("PersonPart").Find("PersonFigure").GetComponent<Image>().sprite = sprite;
        transform.Find("RolePart").Find("RoleOutline").GetComponent<Image>().sprite = Resources.Load<Sprite>("Role" + (isMale ? "Male" : "Female"));
        transform.Find("RolePart").Find("RoleFigure").GetComponent<Image>().sprite = Resources.Load<Sprite>(resourceName + (isMale ? "Male" : "Female"));

        //transform.Find("ProsperityBar").Find("Slider").GetComponent<RectTransform>().anchoredPosition = new Vector2(prosperityOffset, 0.0f);
    }

    public void IncrementCounter(int ammount)
    {
        prosperityCounter += ammount;
        if (prosperityCounter > prosperityMax)
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

    public void SetRelationship()
    {
        Transform characters = GameObject.Find("Characters").transform;
        for (int j = 0; j < characters.childCount; ++j)
        {
            if(characters.GetChild(j).GetComponent<CharacterScript>().relationshipName == relationshipName)
            {
                relationshipScript = characters.GetChild(j).GetComponent<CharacterScript>();
                return;
            }
        }
        characters = GameObject.Find("OtherCharacters").transform;
        for (int i = 0; i < characters.childCount; ++i)
        {
            for (int j = 0; j < characters.GetChild(i).childCount; ++j)
            {
                if (characters.GetChild(i).GetChild(j).GetComponent<CharacterScript>().relationshipName == relationshipName)
                {
                    relationshipScript = characters.GetChild(i).GetChild(j).GetComponent<CharacterScript>();
                    return;
                }
            }
        }
    }

    public void OnPolicyCard(PolicyCard card, bool isRelationship)
    {
        int ammount = card.ammount;
        if(isRelationship)
        {
            ammount /= 2;
            if(ammount > 0)
            {
                --ammount;
            }
            
        }
        else
        {
            relationshipScript.OnPolicyCard(card, true);
        }
        
        switch (card.policyType)
        {
            case PolicyType.MigrationStatus:
            {
                    //If you have illegal migration status with N prosperity, gain Documented status - 2 Cards.
                    if (card.policyIndex == 0)
                    {
                        if(migrationStatus == MigrationStatus.undocumented)
                        {
                            if (prosperityCounter > (prosperityMax / 2))
                            {
                                migrationStatus = MigrationStatus.working;
                            }
                        }
                        
                    }
                    //+n Prosperity if you are illegal, -N prosperity for legal. (vice versa) - 2 Cards.
                    else if (card.policyIndex == 1)
                    {
                        if (migrationStatus == MigrationStatus.undocumented)
                        {
                            IncrementCounter(card.ammount);
                        }
                        else
                        {
                            IncrementCounter(-card.ammount);
                        }
                    }
                    //Become a citizen if working for 4 years and prosperity is +n in any of the next 3 turns. - 1 cards (2 duplicates)
                    else if (card.policyIndex == 2)
                    {

                    }
                    //Lose job if illegal with -n prosperity, legals get +N prosperity. (after 3 turns) - 2 Cards
                    else if (card.policyIndex == 3)
                    {

                    }
                    
                break;
            }
            case PolicyType.Occupation:
            {
                    //If you are of X profession +-n. - one card for each profession = 7 cards (any duplicates?)
                    if (card.policyIndex == 0)
                    {
                        if (jobTier == JobTier.tierOne)
                        {
                            if (card.professionName == proffession)
                            {
                                IncrementCounter(card.ammount);
                            }
                        }
                    }
                    //If you are of -N prosperity as X profession +n else -n. - 7 cards
                    else if (card.policyIndex == 1)
                    {
                        if (jobTier == JobTier.tierOne && prosperityCounter < 0)
                        {
                            if (card.professionName == proffession)
                            {
                                IncrementCounter(card.ammount);
                            }
                        }
                    }
                    //If your prosperity is +n you get a promotion in your job, (if you are legal) (after n turns) - 7 cards.
                    else if (card.policyIndex == 2)
                    {
                        if (migrationStatus != MigrationStatus.undocumented)
                        {
                            if (jobTier != JobTier.tierThree)
                            {
                                //Do promotion
                            }
                        }
                    }
                    //Gain +N prosperity for X profession, lose -n prosperity for Z profession. - 4P2 = 12cards, only the first four occupation group.
                    else if (card.policyIndex == 3)
                    {
                        if (card.professionName == proffession)
                        {

                        }
                    }
                    break;
            }
            case PolicyType.OccupationHierarchy:
            {
                    //Tier X workers can gain +n prosperity but tier Z workers will lose -n Prosperity (Vice Versa.) - 3P2 = 6 cards.
                    if (card.policyIndex == 0)
                    {
                        if (jobTier == JobTier.tierOne)
                        {
                            IncrementCounter(card.ammount);
                        }
                        else
                        {
                            IncrementCounter(-card.ammount);
                        }
                    }
                    //Tier X workers can change jobs or promote, if positive prosperity. - 3 Cards.
                    else if (card.policyIndex == 1)
                    {
                        if(jobTier == JobTier.tierOne)
                        {
                            if(prosperityCounter > 0)
                            {

                            }
                        }
                    }
                    //Tier X workers have a chance to lose their jobs, -n prosperity. - 3 Cards.
                    else if (card.policyIndex == 2)
                    {
                        if (jobTier == JobTier.tierOne)
                        {

                        }
                    }
                    //Tier X workers gain +1 prosperity for the next 2 rounds. - 3 Cards.
                    else if (card.policyIndex == 3)
                    {

                    }
                    break;
            }
        }
    }
}

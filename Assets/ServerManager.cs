using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerManager : MonoBehaviour {

    private List<PolicyCard> policyDeck = new List<PolicyCard>();
    private CharacterSet charactersSet = new CharacterSet();
    private int[] policyVotes = new int[3];
    private int currentPlayerIndex = 0;
    private int voteCounter = 0;
    private PolicyCard[] currentPolicyCards = new PolicyCard[3];

    // Use this for initialization
    void Start () {
        CreatePlayerDeck();
        CreatePolicyDeck();
        DontDestroyOnLoad(gameObject);

        ResetPolicyVotes();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void ResetPolicyVotes()
    {
        GameObject policyChoices = GameObject.Find("PolicyChoices");
        for (int i = 0; i < policyVotes.Length; ++i)
        {
            policyVotes[i] = 0;
            if(policyChoices)
            {
                policyChoices.transform.GetChild(i).Find("VoteCount").GetComponent<Text>().text = "0";
            }
        }
    }

    void CreatePlayerDeck()
    {
        List<Character> characters = new List<Character>();

        List<string> names = new List<string>(Characters.names);
        List<string> tierOne = new List<string>(Characters.tierOneJobs);
        List<string> tierTwo = new List<string>(Characters.tierTwoJobs);
        List<string> tierThree = new List<string>(Characters.tierThreeJobs);

        int relationshipCount = 0;

        for (int i = 0; i < 12; ++i)
        {
            ++relationshipCount;
            int randomIndex = Random.Range(0, names.Count);
            Character character = new Character();
            character.name = names[randomIndex];
            names.Remove(character.name);
            character.isMale = randomIndex < 6;
            character.migrationStatus = (MigrationStatus)Random.Range(0, 4);
            switch (character.migrationStatus)
            {
                case MigrationStatus.local:
                    {
                        character.roleName = tierThree[Random.Range(0, tierThree.Count)];
                        break;
                    }
                case MigrationStatus.student:
                    {
                        character.roleName = "Student";
                        break;
                    }
                case MigrationStatus.undocumented:
                    {
                        character.roleName = tierOne[Random.Range(0, tierOne.Count)];
                        break;
                    }
                case MigrationStatus.working:
                    {
                        character.roleName = tierTwo[Random.Range(0, tierTwo.Count)];
                        break;
                    }
            }
            if (relationshipCount == 2)
            {
                relationshipCount = 0;
                var item = characters[characters.Count - 1];
                item.relationshipName = character.name;
                character.relationshipName = item.name;
            }

            characters.Add(character);
        }

        charactersSet.characters = new Character[3];
        charactersSet.characters2 = new Character[3];
        charactersSet.characters3 = new Character[3];
        charactersSet.characters4 = new Character[3];

        for (int i = 0; i < 3; ++i)
        {
            charactersSet.characters[i] = characters[i];
            charactersSet.characters2[i] = characters[i + 3];
            charactersSet.characters3[i] = characters[i + 6];
            charactersSet.characters4[i] = characters[i + 9];
        }
    }

    void AddPolicyCard(PolicyType type, int index, string profName, string profName2, JobTier tier, JobTier tier2)
    {
        PolicyCard card = new PolicyCard();
        card.policyType = type;
        card.professionName = profName;
        card.professionName2 = profName2;
        card.tierOne = tier;
        card.tierTwo = tier2;
        card.policyIndex = index;
        card.ammount = Random.Range(1, 20);
        policyDeck.Add(card);
    }

    void AddPolicyCard(PolicyType type, int index)
    {
        PolicyCard card = new PolicyCard();
        card.policyType = type;
        card.policyIndex = index;
        policyDeck.Add(card);
    }

    void CreatePolicyDeck()
    {
        //CATEGORY ONE
        //+- Based on proffession
        for (int i = 0; i < Characters.professions.Length; ++i)
        {
            for (int j = 0; j < 2; ++j)
            {
                AddPolicyCard(PolicyType.Occupation, j, Characters.professions[i], "", JobTier.tierOne, JobTier.tierOne);
            }
        }
        for (int i = 0; i < 4; ++i)
        {
            for (int j = 0; j < 4; ++j)
            {
                if (j != i)
                {
                    AddPolicyCard(PolicyType.Occupation, 3, Characters.professions[i], Characters.professions[j], JobTier.tierOne, JobTier.tierOne);
                }
            }
        }

        //CATEGORY TWO
        for (int i = 0; i < 3; ++i)
        {
            for (int j = 0; j < 3; ++j)
            {
                if (j != i)
                {
                    AddPolicyCard(PolicyType.OccupationHierarchy, 0, "", "", (JobTier)i, (JobTier)j);
                }
            }

            //AddPolicyCard(PolicyType.OccupationHierarchy, 1, "", "", (JobTier)i, 0);
            AddPolicyCard(PolicyType.OccupationHierarchy, 2, "", "", (JobTier)i, 0);
            AddPolicyCard(PolicyType.OccupationHierarchy, 3, "", "", (JobTier)i, 0);
        }

        //CATEGORY THREE AND FOUR
        for (int i = 0; i < 4; ++i)
        {
            AddPolicyCard(PolicyType.MigrationStatus, i);
            AddPolicyCard(PolicyType.MigrationStatus, i);
            AddPolicyCard(PolicyType.Unique, i);
        }

        //Randomise
        for (int i = 0; i < policyDeck.Count; i++)
        {
            PolicyCard temp = policyDeck[i];
            int randomIndex = Random.Range(i, policyDeck.Count);
            policyDeck[i] = policyDeck[randomIndex];
            policyDeck[randomIndex] = temp;
        }
    }

    public void DistributeCharacters()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; ++i)
        {
            players[i].GetComponent<PlayerScript>().RpcAddCharacters(charactersSet);
        }
    }

    public void DistributePolicies()
    {
        for (int i = 0; i < currentPolicyCards.Length; ++i)
        {
            currentPolicyCards[i] = policyDeck[i];
        }

        policyDeck.RemoveRange(0, currentPolicyCards.Length);

        var players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; ++i)
        {
            players[i].GetComponent<PlayerScript>().RpcRecievePolicies(currentPolicyCards);
        }
    }

    public void BeginTurn()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; ++i)
        {
            players[i].GetComponent<PlayerScript>().RpcBeginTurn(currentPlayerIndex);
        }
        ++currentPlayerIndex;
        if(currentPlayerIndex == 4)
        {
            currentPlayerIndex = 0;
        }
    }

    public void AddVote(int index)
    {
        policyVotes[index]++;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>().RpcSendVotesToClient(index, policyVotes[index]);
        ++voteCounter;

        //If all the votes are recieved
        if (voteCounter == 3)
        {
            int highest = 0;
            int voteIndex = 0;
            for(int i = 0; i < policyVotes.Length; ++i)
            {
                if(policyVotes[i] > highest)
                {
                    highest = policyVotes[i];
                    voteIndex = i;
                }
            }

            var players = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < players.Length; ++i)
            {
                players[i].GetComponent<PlayerScript>().RpcUpdatePolicyCard(currentPolicyCards[voteIndex]);
            }
            
            ResetPolicyVotes();
            voteCounter = 0;
            BeginTurn();
        }
    }
}

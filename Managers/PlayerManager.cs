using System.Collections.Generic;
using Characters;
using Characters.Allies;
using UnityEngine;

namespace Managers {
	public class PlayerManager : MonoBehaviour {
	
		public static PlayerManager pm;
	
		[SerializeField] private GameObject mainCharacter;
		[SerializeField] private GameObject otherCharacter;

		private GameObject playerTwo;
		private GameObject playerThree;
	
		private readonly List<Player> characters = new List<Player>();
		private readonly List<Player> partyMembers = new List<Player>();
	
		private readonly Dictionary<CharacterSlot, Player> partyMemberLocations = new Dictionary<CharacterSlot, Player>();
	
//    private Dictionary<string, CharacterEquipement> characterEquipements = new Dictionary<string, CharacterEquipement>();
//    private List<PartyMember> allyReserves = new List<PartyMember>();
		private const int MAX_PARTY_MEMBERS = 3;
		private int incapacitatedCount;
		public int MaxPartyMembers {
			get {
				return MAX_PARTY_MEMBERS;
			}
		}

		public Player MainCharacter {
			get { return mainCharacter.GetComponent<Player>(); }
			set { mainCharacter = value.gameObject; }
		}

		private void OnEnable() {
			if (pm == null) {
				pm = this;
			} else if (pm != this) {
				Destroy(gameObject);
			}
			DontDestroyOnLoad(gameObject);
			InitParty();
//        CreateAllCharacterEquipmentObject();
		}

		public void InitParty() {
			AddAllyToParty(mainCharacter.GetComponent<Player>());
			AddAllyToParty(otherCharacter.GetComponent<Player>());
			playerTwo = otherCharacter;
		}

		public void AddPlayableCharacter(Player player) {
			characters.Add(player);
		}
	
		public void AddAllyToParty(Player newAlly) {
			if(partyMembers.Count > MAX_PARTY_MEMBERS) {
				// If party member count > MAX_PARTY_MEMBERS, move those members to the reserves
				for (int i = partyMembers.Count - 1; i >= MAX_PARTY_MEMBERS; i--) {
					partyMembers.RemoveAt(i);
				}
				characters.Add(newAlly);
				return;
			}
			if (partyMembers.Count == MAX_PARTY_MEMBERS) {
				// If party count == MAX_PARTY_MEMBERS, add new ally to reserves
				characters.Add(newAlly);
				return;
			}
			newAlly.SetInParty(true);
			CharacterSlot slot = (CharacterSlot) partyMembers.Count; // character slot is based off last index + 1
			newAlly.slot = slot;
			partyMemberLocations.Add(slot, newAlly);
			partyMembers.Add(newAlly);
			BattleEventManager.bem.AddAllyIncapacitatedListener(slot, newAlly.gameObject, IncIncapacitatedCount);
			newAlly.SetPartySlot(partyMembers.Count-1);
			characters.Add(newAlly);
		}
	
		public List<Player> GetCharacters() {
			Debug.Log("GETTING LIST "+characters[0]);
			return characters;
		}
	
		public List<Player> GetParty() {
			return partyMembers;
		}
	
		public Dictionary<CharacterSlot, Player> GetPartyMemberLocations() {
			return partyMemberLocations;
		}

		public int GetPartyCount() {
			return partyMembers.Count;
		}
	
		public int AllyCount() {
			return partyMembers.Count;
		}
	
		public void IncIncapacitatedCount(GameObject unit) {
			Character character = unit.GetComponent<Character>();
			if (character.GetAffiliation() == Affiliation.Enemy) {
				Debug.Log("Defeated unit is an enemy...");
				return;
			}
			incapacitatedCount++;
			if (incapacitatedCount > MAX_PARTY_MEMBERS) {
				incapacitatedCount = MAX_PARTY_MEMBERS;
			}
		}

		public bool AllAlliesIncapacitated() {
//        int incapacitatedCount = 0;
//        foreach (Player player in partyMembers) {
//            if (player.IsIncapacitated()) {
//                incapacitatedCount++;
//            }
//        }
			return incapacitatedCount == partyMembers.Count;
		}

		public void IncapacitateUnit(Character unit) {
			unit.TryIncapacitate();
//        incapacitatedCount = incapacitatedCount+1 <= 3 ? incapacitatedCount+1 : MAX_PARTY_MEMBERS;
		}
	
		public void ReviveAllyFlatHp(Character unit, int hp) {
			unit.TryRemoveStatus(Status.Incapacitated);
			unit.ModifyCurrentHp(hp);
//        incapacitatedCount = incapacitatedCount - 1 >= 0 ? incapacitatedCount - 1 : 0;
		}
	
		public void ReviveAllyPercentHp(Character unit, float hpPercent) {
			unit.TryRemoveStatus(Status.Incapacitated);
			unit.ModifyCurrentHpByPercentageOfMax(hpPercent);
//        incapacitatedCount = incapacitatedCount - 1 >= 0 ? incapacitatedCount - 1 : 0;
		}
//    public void CreateAllCharacterEquipmentObject() {
//        List<Player> chars = GetCharacters();
//        foreach (Player character in chars) {
//            characterEquipements.Add(character.name, character.GetComponent<CharacterEquipement>());
//        }
//    }
//
//    public void AddCharacterEquipmentObject(Player character) {
//        characterEquipements.Add(character.name, character.GetComponent<CharacterEquipement>());
//    }
	}
}
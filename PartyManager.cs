using System;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour {

//    public static PartyManager pm;
//    [SerializeField] private GameObject player;
//
////    private enum PartySlot {
////        One, 
////        Two, 
////        Three
////    }
//    
////    private readonly Dictionary<PartySlot, PartyMember> partyMembers = new Dictionary<PartySlot,PartyMember>();
////    private readonly PartyMember[] partyMembers = new PartyMember[Enum.GetValues(typeof(PartySlot)).Length];
//    private readonly List<PartyMember> partyMembers = new List<PartyMember>();
//    private List<PartyMember> allyReserves = new List<PartyMember>();
//
//    private void OnEnable() {
//        if (pm == null) {
//            pm = this;
//        } else if(pm != this) {
//            Destroy(gameObject);
//        }
//        DontDestroyOnLoad(gameObject);
//        AddAllyToParty(player);
//    }

//    public void AddAllyToParty(GameObject ally) {
//        if(partyMembers.Count > 3) {
//            // If party member count > 3, move those members to the reserves
//            for (int i = partyMembers.Count - 1; i >= 3; i--) {
//                allyReserves.Add(partyMembers[i]);
//                partyMembers.RemoveAt(i);
//            }
//            PartyMember newAlly = new PartyMember(ally);
//            allyReserves.Add(newAlly);
//            return;
//        }
//        if (partyMembers.Count == 3) {
//            // If party count == 3, add new ally to reserves
//            PartyMember newAlly = new PartyMember(ally);
//            allyReserves.Add(newAlly);
//            return;
//        }
//        PartyMember member = new PartyMember(ally);
//        partyMembers.Add(member);
//    }
//
//    public void ReplacePartyMember(PartyMember newPartyMember) {
//        
//    }
    
//    public void InitPartyMembers() {
//        if (partyMembers.Count == 0) {
//            frontUnit.GetComponent<Player>().SetPartySlot((int) PartySlot.Front);
//            partyMembers.Add(frontUnit);
//            if (backUnit != null) {
//                backUnit.GetComponent<Player>().SetPartySlot((int) PartySlot.Back);
//                partyMembers.Add(backUnit);
//            }
//            Debug.Log("Party length: " + partyMembers.Count);
//        } else {
//            Debug.Log( "Party already instantiated.");
//        }
//    }
}


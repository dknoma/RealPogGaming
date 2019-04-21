using Characters;
using Characters.Allies;
using Managers;
using UnityEngine;

namespace BattleUI {
	public class PlayerStatBar : MonoBehaviour {

		public CharacterSlot slot;
		[SerializeField] private Sprite[] numberSprites = new Sprite[11];
	
		[SerializeField] private Transform hpBar;
		[SerializeField] private Transform mpBar;
		// Health and mana value positions
		[SerializeField] private SpriteRenderer hpOnesDoubleDigit;
		[SerializeField] private SpriteRenderer hpTensDoubleDigit;
		[SerializeField] private SpriteRenderer hpOnesTripleDigit;
		[SerializeField] private SpriteRenderer hpTensTripleDigit;
		[SerializeField] private SpriteRenderer hpHundredsDigit;
		[SerializeField] private SpriteRenderer mpOnesDoubleDigit;
		[SerializeField] private SpriteRenderer mpTensDoubleDigit;
		[SerializeField] private SpriteRenderer mpOnesTripleDigit;
		[SerializeField] private SpriteRenderer mpTensTripleDigit;
		[SerializeField] private SpriteRenderer mpHundredsDigit;
	
		[SerializeField] private SpriteRenderer hpModStatus;
		[SerializeField] private SpriteRenderer atkModStatus;
		[SerializeField] private SpriteRenderer defModStatus;
		[SerializeField] private SpriteRenderer spdModStatus;

		[SerializeField] private Sprite neutralSprite;
		[SerializeField] private Sprite statUpSprite;
		[SerializeField] private Sprite statDownSprite;

		private Player myPlayer;

		public Player MyPlayer {
			get {
				return myPlayer;
			}
			set {
				myPlayer = value;
			}
		}

		private void Start() {
			myPlayer = PlayerManager.pm.GetPartyMemberLocations()[slot];
			UpdateHpNumbers(myPlayer);
			UpdateMpNumbers(myPlayer);
		}

		private void ClearDoubleDigitHpCounter() {
			hpOnesDoubleDigit.sprite = numberSprites[10];
			hpTensDoubleDigit.sprite = numberSprites[10];
		}
	
		private void ClearDoubleDigitMpCounter() {
			mpOnesDoubleDigit.sprite = numberSprites[10];
			mpTensDoubleDigit.sprite = numberSprites[10];
		}
	
//		public void ListenOnPlayerStats(Player player) {
//			player.AddHpValueChangeListener(UpdateHpBar);
//			player.AddMpValueChangeListener(UpdateMpBar);
//		}
//		/// <summary>
//		/// Listen in on the corresponding Player who's slot matches this one's
//		/// </summary>
//		public void ListenOnPlayerStats() {
//			myPlayer = PlayerManager.pm.GetPartyMemberLocations()[slot];
//			myPlayer.AddHpValueChangeListener(UpdateHpBar);
//			myPlayer.AddMpValueChangeListener(UpdateMpBar);
//			myPlayer.AddHpModListener(UpdateHpStatSprite);
//			myPlayer.AddAtkModListener(UpdateAtkStatSprite);
//			myPlayer.AddDefModListener(UpdateDefStatSprite);
//			myPlayer.AddSpdModListener(UpdateSpdStatSprite);
//		}

		public void UpdateHpBar(Character player, float percent) {
			// Update the bar's fill
			if (percent <= 0) {
				hpBar.localScale = Vector3.zero;
			}
			Vector3 scale = hpBar.localScale;
			scale.x = percent; // % ratio of currentHP/maxHP
			hpBar.localScale = scale;
			// Update the actual HP number sprites
			UpdateHpNumbers(player);
		}
	
		public void UpdateMpBar(Character player, float percent) {
			// Update the bar's fill
			if (percent <= 0) {
				hpBar.localScale = Vector3.zero;
			}
			Vector3 scale = mpBar.localScale;
			scale.x = percent; // % ratio of currentMP/maxMP
			mpBar.localScale = scale;
			// Update the actual HP number sprites
			UpdateMpNumbers(player);
		}

		private void UpdateHpNumbers(Character player) {
			int currentHp = player.GetCurrentHp();
			if (currentHp < 0) {
				currentHp = 0;
			}
			int maxHp = player.GetMaxHp();
			int counterDigits = maxHp < 100 ? 2 : 3;
			switch (counterDigits) {
				case 2:
					for (int i = 0, digit = 1; i < counterDigits; i++, digit *= 10) {
						int valueAtDigit = currentHp / digit % 10;
						switch (digit) {
							case 1:
								if (hpOnesDoubleDigit.sprite == numberSprites[valueAtDigit]) break;
								hpOnesDoubleDigit.sprite = numberSprites[valueAtDigit];
								break;
							case 10:
								if (hpTensDoubleDigit.sprite == numberSprites[valueAtDigit]) break;
								hpTensDoubleDigit.sprite = valueAtDigit > 0 ? numberSprites[valueAtDigit] : numberSprites[10];
								break;
							default:
								Debug.Log("Value is higher than it should be.");
								break;
							
						}
					}
					break;
				case 3:
					for (int i = 0, digit = 1; i < counterDigits; i++, digit *= 10) {
						int valueAtDigit = currentHp / digit % 10;
						switch (digit) {
							case 1:
								if (hpOnesTripleDigit.sprite == numberSprites[valueAtDigit]) break;
								hpOnesTripleDigit.sprite = numberSprites[valueAtDigit];
								break;
							case 10:
								if (hpTensTripleDigit.sprite == numberSprites[valueAtDigit]) break;
								if (currentHp >= 100) {
									hpTensTripleDigit.sprite = numberSprites[valueAtDigit];
								} else {
									hpTensTripleDigit.sprite = valueAtDigit > 0 ? numberSprites[valueAtDigit] : numberSprites[10];
								}
								break;
							case 100:
								if (hpHundredsDigit.sprite == numberSprites[valueAtDigit]) break;
								hpHundredsDigit.sprite = valueAtDigit > 0 ? numberSprites[valueAtDigit] : numberSprites[10];
								break;
							default:
								Debug.Log("Value is higher than it should be.");
								break;
							
						}
					}
					break;
				default:
					Debug.Log("Error.");	
					break;
			}
		}
	
		private void UpdateMpNumbers(Character player) {
			int currentMp = player.GetCurrentMp();
			int maxMp = player.GetMaxMp();
			int counterDigits = maxMp < 100 ? 2 : 3;
			switch (counterDigits) {
				case 2:
					for (int i = 0, digit = 1; i < counterDigits; i++, digit *= 10) {
						int valueAtDigit = currentMp / digit % 10;
						switch (digit) {
							case 1:
								if (mpOnesDoubleDigit.sprite == numberSprites[valueAtDigit]) break;
								mpOnesDoubleDigit.sprite = numberSprites[valueAtDigit];
								break;
							case 10:
								if (mpTensDoubleDigit.sprite == numberSprites[valueAtDigit]) break;
								mpTensDoubleDigit.sprite = valueAtDigit > 0 ? numberSprites[valueAtDigit] : numberSprites[10];
								break;
							default:
								Debug.Log("Value is higher than it should be.");
								break;
							
						}
					}
					break;
				case 3:
					for (int i = 0, digit = 1; i < counterDigits; i++, digit *= 10) {
						int valueAtDigit = currentMp / digit % 10;
						switch (digit) {
							case 1:
								if (mpOnesTripleDigit.sprite == numberSprites[valueAtDigit]) break;
								mpOnesTripleDigit.sprite = numberSprites[valueAtDigit];
								break;
							case 10:
								if (mpTensTripleDigit.sprite == numberSprites[valueAtDigit]) break;
								if (currentMp >= 100) {
									mpTensTripleDigit.sprite = numberSprites[valueAtDigit];
								} else {
									mpTensTripleDigit.sprite = valueAtDigit > 0 ? numberSprites[valueAtDigit] : numberSprites[10];
								}
								break;
							case 100:
								if (mpHundredsDigit.sprite == numberSprites[valueAtDigit]) break;
								mpHundredsDigit.sprite = valueAtDigit > 0 ? numberSprites[valueAtDigit] : numberSprites[10];
								break;
							default:
								Debug.Log("Value is higher than it should be.");
								break;
							
						}
					}
					break;
				default:
					Debug.Log("Error.");	
					break;
			}
		}

		public void UpdateHpStatSprite(bool statUp, bool statDown) {
			if(statUp && !statDown) {
				hpModStatus.sprite = statUpSprite;
			} else if(!statUp && statDown) {
				hpModStatus.sprite = statDownSprite;
			} else {
				hpModStatus.sprite = neutralSprite;
			}
		}
	
		public void UpdateAtkStatSprite(bool statUp, bool statDown) {
			Debug.LogFormat("Updating atk stat sprite.");
			if(statUp && !statDown) {
				atkModStatus.sprite = statUpSprite;
			} else if(!statUp && statDown) {
				atkModStatus.sprite = statDownSprite;
			} else {
				atkModStatus.sprite = neutralSprite;
			}
		}
	
		public void UpdateDefStatSprite(bool statUp, bool statDown) {
			if(statUp && !statDown) {
				defModStatus.sprite = statUpSprite;
			} else if(!statUp && statDown) {
				defModStatus.sprite = statDownSprite;
			} else {
				defModStatus.sprite = neutralSprite;
			}
		}
	
		public void UpdateSpdStatSprite(bool statUp, bool statDown) {
			if(statUp && !statDown) {
				spdModStatus.sprite = statUpSprite;
			} else if(!statUp && statDown) {
				spdModStatus.sprite = statDownSprite;
			} else {
				spdModStatus.sprite = neutralSprite;
			}
		}
	}
}

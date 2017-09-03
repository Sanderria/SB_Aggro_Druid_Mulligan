using System;
using System.Collections.Generic;
using System.Linq;
using SmartBot.Database;
using SmartBot.Plugins.API;
/*
	//		in front of text means it is a comment and does nothing (you can use this to include your own comments)
	&&		AND
	||		OR
	!		NOT
	x < y	less than (true if x is less than y).
	x > y	greater than (true if x is greater than y).
	x <= y	less than or equal to.
	x >= y	greater than or equal to.
	x == y	equal to
	x != y	not equal to

*/
namespace SmartBot.Mulligan
{
    public static class Extension2
    {
        public static bool ContainsAll<T1>(this IList<T1> list, params T1[] items)
        {
            return !items.Except(list).Any();
        }
    }
    [Serializable]

		
    public class Hunter : MulliganProfile 
    {
        
/// SECTION ZERO: Name your Groups (Types)	
		
		//You need to add all new types of cards here before using them.
        //You also need to define them before using "Kept" "HasChoice" "KeptNumb" "HasChoiceNumb" or you will get an error
        private enum Defs
        {
            OneDropBeast,
            TwoDropBeast,
            ThreeDropBeast,
        }
		/// Unnecessary
#region Unnecessary
		/// DO NOT CHANGE ANYTHING BETWEEN THIS AND 'START OF MULLIGAN RULES'

		private string _log = "\r\n---Deathstalker_Mulligan_1.0 Mulligan---"; 
        private List<Card.Cards> _choices; 
        private readonly List<Card.Cards> _keep = new List<Card.Cards>();  //Defined list of cards we will keep. This is what needs to be filled in and returned in HandleMulligan

        private readonly Dictionary<Defs, List<Card.Cards>> _defsDictionary = new Dictionary<Defs, List<Card.Cards>>(); 

        public List<Card.Cards> HandleMulligan(List<Card.Cards> choices, Card.CClass opponentClass, Card.CClass ownClass)
        {
			List<Card.Cards> _myDeck;
			try
            {
                //SmartBot will always execute this line of code. 
                _myDeck = Bot.CurrentDeck().Cards.Select(card => (Card.Cards) Enum.Parse(typeof(Card.Cards), card)).ToList();
            }
            catch (Exception) //Mulligan Tester is unable to call Bot.CurrentDeck().Cards, so you have to hardcode the deck. That's what _myDeck below is for. It has no effect on SmartBot behaviour.
                             
            {
                _myDeck = new List<Card.Cards>{Cards.AzureDrake, Cards.SirFinleyMrrgglton, /*fill the rest, doesn't need to be 30. As far as Mulligan Tester is concerned you can even put 1000 cards in here. You will only change these cards for testing purposes if your logic depends on something unique inside of your deck. Such as: different logic if SirFinley is present.*/ };
                // The cards mentioned here are only for testing purposes in Mulligan Tester and they have no effect on the mulligan by SmartBot. SmartBot will check which cards are actually in the deck.
            }

			_choices = choices; //Define our choices
            bool coin = _choices.Contains(Card.Cards.GAME_005);
            if (coin)
            {
                //You no longer need to save the coin, but it's good practice. GAME_005 is a coin.
                _choices.Remove(Card.Cards.GAME_005);
                _keep.Add(Card.Cards.GAME_005);
            }
#endregion

			/// START OF MULLIGAN RULES

			/// SECTION ONE: Groups (Types)
			/// Combine cards in groups (types) if you want, like OneDrops, TwoDrops, Threedrops

			Define(Defs.OneDropBeast, Cards.Alleycat, Cards.HungryCrab);
			Define(Defs.TwoDropBeast, Cards.KindlyGrandmother, Cards.ScavengingHyena, Cards.GolakkaCrawler, Cards.DireWolfAlpha);
			Define(Defs.ThreeDropBeast, Cards.AnimalCompanion, Cards.RatPack);

			/// SECTION TWO: Cards we always keep, vs all classes:

            Keep("-> We always keep this", 
				Cards.Alleycat, Cards.Alleycat,
				Cards.HungryCrab,
				Cards.CracklingRazormaw,
				Cards.KindlyGrandmother, Cards.KindlyGrandmother,
				Cards.MistressofMixtures, Cards.MistressofMixtures
				);

			/// SECTION THREE: Cards we want to keep vs specific classes:
#region Classes
			switch (opponentClass)
            {
                
				case Card.CClass.PALADIN:
						
					Keep("-> keep both hungry crabs vs Paladin", Cards.HungryCrab, Cards.HungryCrab);
					
					break;						
            }
			switch (opponentClass)
			{
				case Card.CClass.WARRIOR:

					Keep("-> keep golakka crawler vs hunter", Cards.GolakkaCrawler);

					if (Kept(Defs.OneDropBeast) >= 1 || Kept(Defs.TwoDropBeast) >= 1)
					{
						Keep("-> keep both golakkas with a twodrop beast or onedrop beast",
							Cards.GolakkaCrawler, Cards.GolakkaCrawler);
					}

					break;
			}
#endregion

			/// SECTION FOUR: advanced mulligan rules:
			if (Kept(Defs.OneDropBeast) >= 1)
			{
				Keep("-> keep both razorwmaws with alleycat or hungry crab", Cards.CracklingRazormaw, Cards.CracklingRazormaw);
			}

			if (Kept(Defs.OneDropBeast) >= 1)
			{
				Keep("-> keep golakka and dire wolf alpha with one drops",
					Cards.GolakkaCrawler, Cards.GolakkaCrawler,
					Cards.DireWolfAlpha);
			}
			
			if (Kept(Defs.OneDropBeast) >= 1)
			{
				Keep("-> keep a hyena with hungry crab or alleycat", Cards.ScavengingHyena);
			}

			if (Kept(Defs.TwoDropBeast) >= 1 || _keep.Contains(Cards.CracklingRazormaw))
			{
				Keep("-> keep animal companions, rat packs and a Stitched Tracker with a two-drop", 
					Cards.AnimalCompanion, Cards.AnimalCompanion,
					Cards.RatPack, Cards.RatPack,
					Cards.StitchedTracker, Cards.StitchedTracker);
			}

			if (Kept(Defs.OneDropBeast) >= 1 || Kept(Defs.TwoDropBeast) >= 1 || Kept(Defs.ThreeDropBeast) >= 1 || 
				_keep.Contains(Cards.MistressofMixtures))
			{
				Keep("-> keep a bow with anything", Cards.EaglehornBow);
			}

			// Keep Houndmaster with 1 and 2 drop beast, 2x 2 drop beast or a 3 drop beast.
			#region Houndmaster
			if (Kept(Defs.TwoDropBeast) >= 1 && Kept(Defs.OneDropBeast) >= 1)
			{
				Keep("-> keep Houndmaster with a 1 drop beast and a 2 drop beast", Cards.Houndmaster);
			}
			else
				if (Kept(Defs.TwoDropBeast) >= 2)
				{
					Keep("-> keep Houndmaster with 2x 2 drop beast", Cards.Houndmaster);
				}
			else
				if (Kept(Defs.ThreeDropBeast) >= 1)
				{
				Keep("-> keep Houndmaster with a 3 drop beast", Cards.Houndmaster);
				}
			#endregion 

			/// END OF MULLIGAN RULES
			#region END
			/// DO NOT CHANGE ANYTHING BELOW (except change Deathstalker_Mulligan_1.0 to the name of the mulligan profile in 4th line from the bottom)

			PrintLog();
            return _keep;
        }
        /// <summary>
        /// Method to fill in his _keep list
        /// </summary>
        /// <param name="reason">Why?</param>
        /// <param name="cards">List of cards he wants to add</param>
        private void Keep(string reason, params Card.Cards[] cards)
        {
            var count = true;
            string str = "Keep: ";
            foreach (var card in cards)
            {
                if (_choices.Contains(card))
                {
                    str += CardTemplate.LoadFromId(card).Name + ",";
                    _choices.Remove(card);
                    _keep.Add(card);
                    count = false;
                }
            }
            if (count) return;
            str = str.Remove(str.Length - 1);
            str += " because " + reason;
            AddLog(str);
        }

        /// <summary>
        /// Defines a list of cards as a certain type on card
        /// </summary>
        /// <param name="type">The type of card that you want to define these cards as</param>
        /// <param name="cards">List of cards you want to define as the given type</param>
        private void Define(Defs type, params Card.Cards[] cards)
        {
            if (_defsDictionary.ContainsKey(type))
            {
                _defsDictionary.Add(type, cards.ToList());
            }
            else
            {
                _defsDictionary[type] = cards.ToList();
            }
        }

        /// <summary>
        /// Returns the numbers of cards of the given type that you have kept
        /// </summary>
        /// <param name="type">The type of card you want to look for</param>
        private int Kept(Defs type)
        {
            return _keep.Count(x => _defsDictionary[type].Contains(x));
        }

        /// <summary>
        /// Returns the numbers of cards of the given type that you have given as a choice
        /// </summary>
        /// <param name="type">The type of card you want to look for</param>
        private int HasChoice(Defs type)
        {
            return _choices.Count(x => _defsDictionary[type].Contains(x));
        }

        private void AddLog(string s)
        {
            _log += "\r\n" + s;
        }

        private void PrintLog()
        {
            Bot.Log(_log);
            _log = "\r\n---Deathstalker_Mulligan_1.0 Mulligan---";
        }
#endregion
	}
}
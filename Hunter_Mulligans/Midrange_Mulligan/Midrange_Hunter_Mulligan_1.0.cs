using System;
using System.Collections.Generic;
using System.Linq;
using SmartBot.Database;
using SmartBot.Plugins.API;

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
        private enum Defs
        {
            OneDropBeast,
			OneDrops,
            TwoDropBeast,
            ThreeDropBeast,
			ThreeDrops
        }
		/// Unnecessary
#region Unnecessary
		/// DO NOT CHANGE ANYTHING BETWEEN THIS AND 'START OF MULLIGAN RULES'

		private string _log = "\r\n---Midrange_Hunter_Mulligan_1.0---"; 
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

			Define(Defs.OneDropBeast, Cards.Alleycat, Cards.HungryCrab, Cards.FieryBat, Cards.JeweledMacaw);
			Define(Defs.OneDrops, Cards.JeweledMacaw, Cards.BloodsailCorsair, Cards.Alleycat, Cards.HungryCrab, Cards.FieryBat, Cards.FireFly);
			Define(Defs.TwoDropBeast, Cards.KindlyGrandmother, Cards.ScavengingHyena, Cards.GolakkaCrawler, Cards.DireWolfAlpha, Cards.CracklingRazormaw);
			Define(Defs.ThreeDropBeast, Cards.AnimalCompanion, Cards.RatPack, Cards.Bearshark);
			Define(Defs.ThreeDrops, Cards.StitchedTracker, Cards.AnimalCompanion, Cards.RatPack, Cards.Bearshark, Cards.EaglehornBow);

			/// VS AGGRO
			if (Aggro(opponentClass))
			{
				Keep("-> Always keep vs aggro", Cards.Alleycat, Cards.Alleycat, Cards.FireFly, Cards.FireFly, Cards.FieryBat, Cards.BloodsailCorsair, 
				Cards.HungryCrab, Cards.CracklingRazormaw, Cards.JeweledMacaw);
				if (coin)
				{
					if (Kept(Defs.TwoDropBeast) == 0)
					{
						Keep("VS Aggro + coin: Keep Golakka", Cards.GolakkaCrawler);
						if (Kept(Defs.TwoDropBeast) == 0)
						{
							Keep("VS Aggro + coin: Keep a Hyena", Cards.ScavengingHyena);
						}
					}
					if (Kept(Defs.OneDropBeast) > 0)
					{
						Keep("VS Aggro + coin: Keep double Razormaws and twodropbeasts with onedropbeasts", Cards.CracklingRazormaw, Cards.CracklingRazormaw, 
							Cards.KindlyGrandmother, Cards.ScavengingHyena, Cards.GolakkaCrawler);
					}
					if (Kept(Defs.OneDropBeast) == 0 && Kept(Defs.OneDrops) > 0)
					{
						Keep("VS Aggro + coin: Keep Grandmother and Golakka with onedrops", Cards.KindlyGrandmother, Cards.GolakkaCrawler);
					}
					if (Kept(Defs.OneDrops) > 0 && Kept(Defs.TwoDropBeast) > 0)
					{
						Keep("VS Aggro + coin: Keep threedrops with onedrops and twodropbeasts", Cards.AnimalCompanion, Cards.RatPack, Cards.Bearshark, 
							Cards.StitchedTracker, Cards.EaglehornBow);
					}
					if (Kept(Defs.OneDrops) > 0 && Kept(Defs.TwoDropBeast) == 0)
					{
						Keep("VS Aggro + coin: Keep Companion with onedrops && no twodropbeasts", Cards.AnimalCompanion);
						if (Kept(Defs.ThreeDrops) == 0)
						{
							Keep("VS Aggro + coin: Keep Bearshark with onedrops && no twodropbeasts", Cards.Bearshark);
						}
						if (Kept(Defs.ThreeDrops) == 0)
						{
							Keep("VS Aggro + coin: Keep RatPack with onedrops && no twodropbeasts", Cards.RatPack);
						}
						if (Kept(Defs.ThreeDrops) == 0)
						{
							Keep("VS Aggro + coin: Keep Stitched Tracker with onedrops && no twodropbeasts", Cards.StitchedTracker);
						}
					}
					if (Kept(Defs.OneDropBeast) > 0 && Kept(Defs.TwoDropBeast) > 0 && Kept(Defs.ThreeDropBeast) > 0)
					{
						Keep("VS Aggro + coin: Keep Houndmaster with one-, two- and threedropbeast", Cards.Houndmaster);
					}
				}
				if (!coin)
				{
					if (Kept(Defs.TwoDropBeast) == 0)
					{
						Keep("VS Aggro + coin: Keep Golakka", Cards.GolakkaCrawler);
						if (Kept(Defs.TwoDropBeast) == 0)
						{
							Keep("VS Aggro + coin: Keep a Hyena", Cards.ScavengingHyena);
						}
					}
					if (Kept(Defs.OneDropBeast) > 0)
					{
						Keep("VS Aggro: Keep double Razormaws and twodropbeasts with onedropbeasts", Cards.CracklingRazormaw, Cards.CracklingRazormaw,
							Cards.KindlyGrandmother, Cards.ScavengingHyena, Cards.GolakkaCrawler);
					}
					if (Kept(Defs.OneDropBeast) == 0 && Kept(Defs.OneDrops) > 0)
					{
						Keep("VS Aggro: Keep Grandmother and Golakka Crawler with onedrops", Cards.KindlyGrandmother, Cards.GolakkaCrawler);
					}
					if (Kept(Defs.OneDrops) > 0 && Kept(Defs.TwoDropBeast) > 0)
					{
						Keep("VS Aggro: Keep threedrops with onedrops and twodropbeasts", Cards.AnimalCompanion, Cards.RatPack, Cards.Bearshark,
							Cards.EaglehornBow, Cards.StitchedTracker);
					}
				}
			}

			/// VS CONTROL
			if (Control(opponentClass))
			{
				Keep("-> Always keep vs control", Cards.Alleycat, Cards.Alleycat, Cards.FireFly, Cards.FireFly, Cards.FieryBat, Cards.BloodsailCorsair, 
					Cards.HungryCrab, Cards.CracklingRazormaw, Cards.KindlyGrandmother, Cards.JeweledMacaw);
				if (coin)
				{
					if (Kept(Defs.TwoDropBeast) == 0)
					{
						Keep("VS Control + coin: Keep a Hyena", Cards.ScavengingHyena);
						if (Kept(Defs.TwoDropBeast) == 0)
						{
							Keep("VS Control + coin: Keep Golakka", Cards.GolakkaCrawler);
						}
					}
					if (Kept(Defs.OneDropBeast) > 0)
					{
						Keep("VS Control + coin: Keep double Razormaws and twodropbeasts with onedropbeasts", Cards.CracklingRazormaw, Cards.CracklingRazormaw,
							Cards.GolakkaCrawler, Cards.ScavengingHyena, Cards.KindlyGrandmother);
					}
					if (Kept(Defs.OneDropBeast) == 0 && Kept(Defs.OneDrops) > 0)
					{
						Keep("VS Control + coin: Keep Golakka and Grandmother with onedrops", Cards.GolakkaCrawler, Cards.KindlyGrandmother);
					}
					if (Kept(Defs.OneDrops) > 0 && Kept(Defs.TwoDropBeast) > 0)
					{
						Keep("VS Control + coin: Keep threedrops with onedrops and twodropbeasts", Cards.AnimalCompanion, Cards.RatPack, Cards.Bearshark);
					}
					if (Kept(Defs.OneDrops) > 0 && Kept(Defs.TwoDropBeast) == 0 && !TargetRemoval(opponentClass))
					{
						Keep("VS Control + coin: Keep Companion with onedrops && no twodropbeasts", Cards.AnimalCompanion);
						if (Kept(Defs.ThreeDrops) == 0)
						{
							Keep("VS Control + coin: Keep Bearshark with onedrops && no twodropbeasts", Cards.Bearshark);
						}
						if (Kept(Defs.ThreeDrops) == 0)
						{
							Keep("VS Control + coin: Keep RatPack with onedrops && no twodropbeasts", Cards.RatPack);
						}
						if (Kept(Defs.ThreeDrops) == 0)
						{
							Keep("VS Control + coin: Keep StitchedTracker with onedrops && no twodropbeasts", Cards.StitchedTracker);
						}
					}
					if (Kept(Defs.OneDrops) > 0 && Kept(Defs.TwoDropBeast) == 0 && TargetRemoval(opponentClass))
					{
						Keep("VS Control(TargetRemoval) + coin: Keep Bearshark with onedrops && no twodropbeasts", Cards.Bearshark);
						if (Kept(Defs.ThreeDrops) == 0 || Kept(Defs.ThreeDropBeast) > 0)
						{
							Keep("VS Control(TargetRemoval) + coin: Keep Companion with onedrops && no twodropbeasts", Cards.AnimalCompanion);
						}
						if (Kept(Defs.ThreeDrops) == 0)
						{
							Keep("VS Control(TargetRemoval) + coin: Keep RatPack with onedrops && no twodropbeasts", Cards.RatPack);
						}
						if (Kept(Defs.ThreeDrops) == 0)
						{
							Keep("VS Control(TargetRemoval) + coin: Keep StitchedTracker with onedrops && no twodropbeasts", Cards.StitchedTracker);
						}
					}
					if (Kept(Defs.OneDropBeast) > 0 && Kept(Defs.TwoDropBeast) > 0 && Kept(Defs.ThreeDropBeast) > 0)
					{
						Keep("VS Control + coin: Keep Houndmaster with one-, two- and threedropbeast", Cards.Houndmaster);
					}
					if (Kept(Defs.OneDropBeast) > 0 && Kept(Defs.ThreeDropBeast) > 1)
					{
						Keep("VS Control + coin: Keep Houndmaster with onedropbeast and 2x threedropbeast", Cards.Houndmaster);
					}
				}
				if (!coin)
				{
					if (Kept(Defs.TwoDropBeast) == 0)
					{
						Keep("VS Control: Keep a Hyena", Cards.ScavengingHyena);
						if (Kept(Defs.TwoDropBeast) == 0)
						{
							Keep("VS Control: Keep Golakka", Cards.GolakkaCrawler);
						}
					}
					if (Kept(Defs.OneDropBeast) > 0)
					{
						Keep("VS Control: Keep double Razormaws and twodropbeasts with onedropbeasts", Cards.CracklingRazormaw, Cards.CracklingRazormaw,
							Cards.GolakkaCrawler, Cards.ScavengingHyena, Cards.KindlyGrandmother);
					}
					if (Kept(Defs.OneDropBeast) == 0 && Kept(Defs.OneDrops) > 0)
					{
						Keep("VS Control: Keep Golakka with onedrops", Cards.GolakkaCrawler, Cards.KindlyGrandmother);
					}
					if (Kept(Defs.OneDrops) > 0 && Kept(Defs.TwoDropBeast) > 0)
					{
						Keep("VS Control: Keep threedrops with onedrops and twodropbeasts", Cards.AnimalCompanion, Cards.RatPack, Cards.Bearshark, Cards.StitchedTracker);
					}
				}
			}

			/// VS PRIEST
			if (Priest(opponentClass))
			{
				Keep("-> Always keep vs Priest", Cards.Alleycat, Cards.Alleycat, Cards.FireFly, Cards.FireFly, Cards.FieryBat, Cards.BloodsailCorsair, 
				Cards.HungryCrab, Cards.CracklingRazormaw, Cards.JeweledMacaw);
				if (coin)
				{
					if (Kept(Defs.TwoDropBeast) == 0)
					{
						Keep("VS Priest + coin: Keep a Hyena", Cards.ScavengingHyena);
						if (Kept(Defs.TwoDropBeast) == 0)
						{
							Keep("VS Priest + coin: Keep Golakka", Cards.GolakkaCrawler);
						}
					}
					if (Kept(Defs.OneDropBeast) > 0)
					{
						Keep("VS Priest + coin: Keep double Razormaws and twodropbeasts with onedropbeasts", Cards.CracklingRazormaw, Cards.CracklingRazormaw,
							Cards.ScavengingHyena, Cards.GolakkaCrawler, Cards.KindlyGrandmother);
					}
					if (Kept(Defs.OneDropBeast) == 0 && Kept(Defs.OneDrops) > 0)
					{
						Keep("VS Priest + coin: Keep Golakka and Grandmother with onedrops", Cards.GolakkaCrawler, Cards.KindlyGrandmother);
					}
					if (Kept(Defs.OneDrops) > 0 && Kept(Defs.TwoDropBeast) > 0)
					{
						Keep("VS Priest + coin: Keep threedrops with onedrops and twodropbeasts", Cards.AnimalCompanion, Cards.RatPack, Cards.Bearshark, Cards.StitchedTracker);
					}
					if (Kept(Defs.OneDrops) > 0 && Kept(Defs.TwoDropBeast) == 0)
					{
						Keep("VS Priest + coin: Keep Bearshark with onedrops && no twodropbeasts", Cards.Bearshark);
						if (Kept(Defs.ThreeDrops) == 0 || Kept(Defs.ThreeDropBeast) > 0)
						{
							Keep("VS Priest + coin: Keep Companion with onedrops && no twodropbeasts", Cards.AnimalCompanion);
						}
						if (Kept(Defs.ThreeDrops) == 0)
						{
							Keep("VS Priest + coin: Keep RatPack with onedrops && no twodropbeasts", Cards.RatPack);
						}
						if (Kept(Defs.ThreeDrops) == 0 || Kept(Defs.ThreeDropBeast) > 0)
						{
							Keep("VS Priest + coin: Keep StitchedTracker with onedrops && no twodropbeasts", Cards.StitchedTracker);
						}
					}
					if (Kept(Defs.OneDropBeast) > 0 && Kept(Defs.TwoDropBeast) > 0 && Kept(Defs.ThreeDropBeast) > 0)
					{
						Keep("VS Priest + coin: Keep Houndmaster with one-, two- and threedropbeast", Cards.Houndmaster);
					}
				}
				if (!coin)
				{
					if (Kept(Defs.TwoDropBeast) == 0)
					{
						Keep("VS Priest: Keep a Hyena", Cards.ScavengingHyena);
						if (Kept(Defs.TwoDropBeast) == 0)
						{
							Keep("VS Priest: Keep Golakka", Cards.GolakkaCrawler);
						}
					}
					if (Kept(Defs.OneDropBeast) > 0)
					{
						Keep("VS Priest: Keep double Razormaws and twodropbeasts with onedropbeasts", Cards.CracklingRazormaw, Cards.CracklingRazormaw,
							Cards.ScavengingHyena, Cards.GolakkaCrawler, Cards.KindlyGrandmother);
					}
					if (Kept(Defs.OneDropBeast) == 0 && Kept(Defs.OneDrops) > 0)
					{
						Keep("VS Priest: Keep Golakka and Grandmother with onedrops", Cards.GolakkaCrawler, Cards.KindlyGrandmother);
					}
					if (Kept(Defs.OneDrops) > 0 && Kept(Defs.TwoDropBeast) > 0)
					{
						Keep("VS Priest: Keep threedrops with onedrops and twodropbeasts", Cards.AnimalCompanion, Cards.RatPack, Cards.Bearshark, Cards.StitchedTracker);
					}
				}
			}

			PrintLog();
            return _keep;
        }
		private bool Aggro(Card.CClass cClass)
		{
			return cClass == Card.CClass.DRUID || cClass == Card.CClass.WARRIOR || cClass == Card.CClass.ROGUE || cClass == Card.CClass.SHAMAN || cClass == Card.CClass.HUNTER
				|| cClass == Card.CClass.PALADIN;
		}
		private bool Control(Card.CClass cClass)
		{
			return cClass == Card.CClass.MAGE || cClass == Card.CClass.WARLOCK;
		}
		// Yup, priest is on it's on because of potion of madness on Grandmother...
		private bool Priest(Card.CClass cClass)
		{
			return cClass == Card.CClass.PRIEST;
		}
		private bool TargetRemoval(Card.CClass cClass)
		{
			return cClass == Card.CClass.PRIEST || cClass == Card.CClass.MAGE;
		}
		//private bool Pirates(Card.CClass cClass)
		//{
		//	return cClass == Card.CClass.DRUID || cClass == Card.CClass.WARRIOR || cClass == Card.CClass.ROGUE || cClass == Card.CClass.SHAMAN || cClass == Card.CClass.HUNTER;
		//}

		/// END OF MULLIGAN RULES
		#region END
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
            _log = "\r\n---Midrange_Hunter_1.0---";
        }
#endregion
	}
}
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
    public class PriestMulligan : MulliganProfile
    {
        
/// SECTION ZERO: Name your Groups (Types)	
		
        private enum Defs
        {
            GoodRemoval
        }
			/// Leave be
			#region Leave
		/// DO NOT CHANGE ANYTHING BETWEEN THIS AND 'START OF MULLIGAN RULES'

		private string _log = "\r\n---HighlanderPriestMulligan---"; 
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
				_myDeck = new List<Card.Cards> { Cards.AzureDrake, Cards.SirFinleyMrrgglton, /*fill the rest, doesn't need to be 30. As far as Mulligan Tester is concerned you can even put 1000 cards in here. You will only change these cards for testing purposes if your logic depends on something unique inside of your deck. Such as: different logic if SirFinley is present.*/ };
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

			/// START OF MULLIGAN RULES
			#endregion

			/// SECTION ONE: Groups (Types)

			Define(Defs.GoodRemoval, Cards.PotionofMadness, Cards.SpiritLash);

			/// SECTION TWO: Cards we always keep, vs all classes:

            Keep("-> We always keep this", Cards.Kazakus); 
			
			/// SECTION THREE: Cards we want to keep vs specific classes:
			
			// Keep Removal and Tar Creeper vs Aggro
            if (Aggro(opponentClass))
			{
				Keep("-> Always keep vs aggro", Cards.PotionofMadness, Cards.SpiritLash, Cards.TarCreeper, Cards.Doomsayer);
			}
			// Keep value cards vs Control
			else if (!Aggro(opponentClass) && opponentClass != Card.CClass.MAGE)
			{
				Keep("-> Always keep vs control", Cards.ShadowVisions, Cards.ElisetheTrailblazer, Cards.ShadowWordDeath, Cards.CuriousGlimmerroot, Cards.RazatheChained);
			}
			else if (!Aggro(opponentClass))
			{
				Keep("-> Always keep vs mage", Cards.ShadowVisions, Cards.ElisetheTrailblazer, Cards.CuriousGlimmerroot, Cards.RazatheChained);
			}

			/// SECTION FOUR: advanced mulligan rules:

			// Keep Holy Smite and SW:Horror without better options
			if (Aggro(opponentClass) && Kept(Defs.GoodRemoval) == 0)
			{
				Keep("-> Keep Holy Smite and SW: horror without better options", Cards.HolySmite, Cards.ShadowWordHorror);
			}

			// Keep SW:Pain with Holy Smite if you don't have SW:Horror
			if (_keep.Contains(Cards.HolySmite) && _keep.Contains(Cards.ShadowWordHorror)) 
			{
				Keep("-> Keep SW: Pain with Smite", Cards.ShadowWordPain);
			}

			// Keep the DeathKnight vs Control with no better options
			if (!Aggro(opponentClass) && !_keep.Contains(Cards.ShadowVisions) && !_keep.Contains(Cards.ElisetheTrailblazer) && _keep.Contains(Cards.ShadowWordDeath) 
				&& _keep.Contains(Cards.CuriousGlimmerroot))
			{
				Keep("-> Keep DK vs control with no better options", Cards.ShadowreaperAnduin);
			}

			// Keep Pint Sized Potion with SW:Horror vs Aggro
			if (Aggro(opponentClass) && _choices.Contains(Cards.ShadowWordHorror) || _keep.Contains(Cards.ShadowWordHorror) && _choices.Contains(Cards.PintSizePotion))
			{
				Keep("-> Keep pint sized potion with SW: Horror", Cards.PintSizePotion);
			}

			// Keep Radiant Elemental and PW:Shield if offered both vs Aggro
			if (Aggro(opponentClass) && _choices.Contains(Cards.RadiantElemental) && _choices.Contains(Cards.PowerWordShield))
			{
				Keep("-> Keep Radiant Elemental and PW: Shield if offered both", Cards.RadiantElemental, Cards.PowerWordShield);
			}

			// Keep Bloodmage Thalnos with Spirit Lash vs Aggro
			if (Aggro(opponentClass) && _keep.Contains(Cards.SpiritLash))
			{
				Keep("-> Keep Thalnos with SpiritLash", Cards.BloodmageThalnos);
			}
			
            PrintLog();
            return _keep;
        }

		private static bool Aggro(Card.CClass cClass)
		{
			return cClass == Card.CClass.WARRIOR || cClass == Card.CClass.PALADIN || cClass == Card.CClass.DRUID || cClass == Card.CClass.SHAMAN 
				|| cClass == Card.CClass.HUNTER;
		}

		/// DONT CHANGE ------------------------------------------------------------------------------------------------------------------------------------

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
            _log = "\r\n---HighlanderPriestMulligan---";
        }
    }
}
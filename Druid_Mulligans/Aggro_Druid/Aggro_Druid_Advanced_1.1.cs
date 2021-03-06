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
	/// Tips
	/*
		//		in front of text means it is a comment and does nothing
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
	
	public class Druid : MulliganProfile
    {
        
/// SECTION ZERO: Name your Groups (Types)	
		
        private enum Defs
        {
            OneDrops,
            TwoDrops,
            ThreeDrops,
			OneDropMinions,
        }
		/// Unnecessary
#region Unnecessary
		/// DO NOT CHANGE ANYTHING BETWEEN THIS AND 'START OF MULLIGAN RULES'

		private string _log = "\r\n---Aggro_Druid_Advanced_1.1 Mulligan---"; 
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
			// Combine cards in groups (types) if you want, like OneDrops, TwoDrops, Threedrops (you can create other groups/types in Section Zero)

			Define(Defs.OneDrops, Cards.Innervate, Cards.BloodsailCorsair, Cards.EnchantedRaven, Cards.FireFly);
			Define(Defs.TwoDrops, Cards.DireWolfAlpha, Cards.DruidoftheSwarm);
			Define(Defs.ThreeDrops, Cards.CryptLord, Cards.ViciousFledgling);
			Define(Defs.OneDropMinions, Cards.BloodsailCorsair, Cards.EnchantedRaven, Cards.FireFly);
			int minionCount = 0; // Minion Count begins
			/// SECTION TWO: Cards we always keep, vs all classes:

			Keep("-> We always keep this", Cards.Innervate, Cards.BloodsailCorsair, Cards.EnchantedRaven, Cards.FireFly, Cards.MarkoftheLotus); 
			
			/// SECTION THREE: Cards we want to keep vs specific classes:
			
            switch (opponentClass)
            {
                
				case Card.CClass.DRUID:
						
					Keep("-> keep vs Druid", Cards.ArchmageAntonidas);
					
					break;		
							
            }
///minionCount
			#region minionCount
			// 2x EnchantedRaven == 2, 1x EnchantedRaven == 1
			if (_keep.Contains(Cards.EnchantedRaven))
			{
				if (_keep.Contains(Cards.EnchantedRaven) && _choices.Contains(Cards.EnchantedRaven))
				{
					Keep("->Keep both EnchantedRavens", Cards.EnchantedRaven, Cards.EnchantedRaven);
					minionCount = minionCount + 2;
				}
				else
					if (_keep.Contains(Cards.EnchantedRaven) && !_choices.Contains(Cards.EnchantedRaven))
					{
						minionCount = minionCount + 1;
					}
			}
			
			// 2x BloodsailCorsair == 3, 1x BloodsailCorsair == 2
			if (_keep.Contains(Cards.BloodsailCorsair))
			{
				if (_keep.Contains(Cards.BloodsailCorsair) && _choices.Contains(Cards.BloodsailCorsair))
				{
					Keep("->Keep both BloodsailCorsairs", Cards.BloodsailCorsair, Cards.BloodsailCorsair);
					minionCount = minionCount + 3;
				}
				else
					if (_keep.Contains(Cards.BloodsailCorsair) && !_choices.Contains(Cards.BloodsailCorsair))
					{
						minionCount = minionCount + 2;
					}
			}
			
			// 2x FireFly == 4, 1x FireFly == 2
			if (_keep.Contains(Cards.FireFly))
			{
				if (_keep.Contains(Cards.FireFly) && _choices.Contains(Cards.FireFly))
				{
					Keep("->Keep both FireFlys", Cards.FireFly, Cards.FireFly);
					minionCount = minionCount + 4;
				}			
				else
					if (_keep.Contains(Cards.FireFly) && !_choices.Contains(Cards.FireFly))
					{
						minionCount = minionCount + 2;
					}
				
			}
			#endregion

			/// SECTION FOUR: advanced rules:
			if (_keep.Contains(Cards.Innervate))
			{
				Keep("-> with Innervate, keep Dr.3", Cards.ViciousFledgling);
			}
			
			if (_keep.Contains(Cards.EnchantedRaven))
			{
				Keep("-> keep Mark with EnchantedRaven", Cards.MarkofYShaarj);
			}
			
			if (minionCount >=1)
			{
				Keep("-> keep Druid of the swarm with 1 or more OneDrops", Cards.DruidoftheSwarm);
			}
			
			if (minionCount >= 2)
			{
				Keep("-> keep two Druid of the Swarm with 2 or more 1-drops", Cards.DruidoftheSwarm, Cards.DruidoftheSwarm);
			}
			
			if (minionCount >= 3)
			{
				Keep("-> keep with 2 or more 1-drops", Cards.DireWolfAlpha, Cards.DireWolfAlpha, Cards. DruidoftheSwarm, Cards.DruidoftheSwarm);
			}
			
			if (minionCount >= 4)
			{
				Keep("-> keep power of the wild with 4 or more OneDrops", Cards.PoweroftheWild);
			}
			
			if ((Kept(Defs.TwoDrops)) >=1 && minionCount >= 2)
			{
				Keep("-> keep with good curve", Cards.CryptLord, Cards.ViciousFledgling);
			}
			
			
/// END OF MULLIGAN RULES
						
/// DO NOT CHANGE ANYTHING BELOW (except change Aggro_Druid_Advanced_1.1 to the name of the mulligan profile in 4th line from the bottom)
			
			
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
            _log = "\r\n---Aggro_Druid_Advanced_1.1 Mulligan---";
        }
    }
}
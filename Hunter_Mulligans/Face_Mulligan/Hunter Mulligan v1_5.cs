using System;
using System.Collections.Generic;
using System.Linq;
using SmartBot.Database;
using SmartBot.Plugins.API;

namespace SmartBot.Mulligan
{
    [Serializable]
    public class HunterMulligan : MulliganProfile
    {

        private static string _name = "Hunter V 1.5";
        private string _log = "\r\n---"+ _name +"---\r\n";

        private List<Card.Cards> _choices;

        private readonly List<Card.Cards> _keep = new List<Card.Cards>();

        private readonly Dictionary<Defs, List<Card.Cards>> _defsDictionary = new Dictionary<Defs, List<Card.Cards>>();

        private enum Defs
        {
            One,
            Two,
			Tree
        }

        public List<Card.Cards> HandleMulligan(List<Card.Cards> choices, Card.CClass opponentClass, Card.CClass ownClass)
        {
            _choices = choices;
            bool coin = _choices.Contains(Card.Cards.GAME_005);
            if (coin)
            {
                _choices.Remove(Card.Cards.GAME_005);
                _keep.Add(Card.Cards.GAME_005);
            }

            Define(Defs.One, 
				Cards.BloodsailCorsair, 
				Cards.Alleycat, 
				Cards.HungryCrab, 
				Cards.FieryBat);

            Define(Defs.Two, 
				Cards.GolakkaCrawler, 
				Cards.CracklingRazormaw, 
				Cards.KindlyGrandmother, 
				Cards.DireWolfAlpha, 
				Cards.ScavengingHyena);
			
			Define(Defs.Tree, 
				Cards.Bearshark, 
				Cards.EaglehornBow, 
				Cards.AnimalCompanion, 
				Cards.StitchedTracker,
				Cards.UnleashtheHounds);

            Keep("Always kept", 
				Cards.KindlyGrandmother,
				Cards.Alleycat,
				Cards.Alleycat,
				Cards.HungryCrab, 
				Cards.CracklingRazormaw,
				Cards.BloodsailCorsair,
				Cards.BloodsailCorsair,
				Cards.FieryBat,
				Cards.ScavengingHyena);

            
            //1, 2
            if (Kept(Defs.One) > 0)
            {
                if (Kept(Defs.Two) == 0)
                {
                    Keep("--- Keep 1 drop into 2 drop", Cards.GolakkaCrawler, Cards.DireWolfAlpha);
                }
            }
			
			if (_choices.Contains(Cards.ScavengingHyena))
			{
				Keep("--- Keep Scavenging into Unleash the Hounds combo ---", Cards.UnleashtheHounds);
			}

            //Coin Crab gaming
            else if(IsPirateClass(opponentClass))
            {
                Keep("--- Keep Golakka into Coin vs pirate class", Cards.GolakkaCrawler);
            }
			
			if (IstargetingClass(opponentClass) && coin)
			{
				Keep("",Cards.Bearshark);
			}
			

            //1, 2, 3 I can count
            if (Kept(Defs.One) > 0 && Kept(Defs.Two) > 0 && Kept(Defs.Tree) == 0) 
            {
            	if (Kept(Defs.Tree) == 0)
                {
					Keep("--- Keep 1, 2, 3 drop curve ---",  Cards.Bearshark);
				}
				if (Kept(Defs.Tree) == 0)
                {
					Keep("--- Keep 1, 2, 3 drop curve ---", Cards.EaglehornBow);
				}
				if (Kept(Defs.Tree) == 0)
                {
					Keep("--- Keep 1, 2, 3 drop curve ---", Cards.AnimalCompanion);
				}
				if (Kept(Defs.Tree) == 0)
                {
					Keep("--- Keep 1, 2, 3 drop curve", Cards.StitchedTracker);
				}
			}

            //1 into 3 with coin
            if (Kept(Defs.One) > 0 && coin)
            {
            	if (Kept(Defs.Tree) == 0)
                {
					Keep("--- Keep 1 into coined 3 drop ---",  Cards.Bearshark);
				}
				if (Kept(Defs.Tree) == 0)
                {
					Keep("--- Keep 1 into coined 3 drop ---", Cards.EaglehornBow);
				}
				if (Kept(Defs.Tree) == 0)
                {
					Keep("--- Keep 1 into coined 3 drop ---", Cards.AnimalCompanion);
				}
				if (Kept(Defs.Tree) == 0)
                {
					Keep("--- Keep 1 into coined 3 drop ---", Cards.StitchedTracker);
				}
			}
			
			if (Kept(Defs.One) > 0 && Kept(Defs.Two) == 0 && coin)
            {
                if (Kept(Defs.Tree) == 0)
                {
					Keep("--- Keep 1 into coined 3 drop ---",  Cards.Bearshark);
				}
				if (Kept(Defs.Tree) == 0)
                {
					Keep("--- Keep 1 into coined 3 drop ---", Cards.EaglehornBow);
				}
				if (Kept(Defs.Tree) == 0)
                {
					Keep("--- Keep 1 into coined 3 drop ---", Cards.AnimalCompanion);
				}
				if (Kept(Defs.Tree) == 0)
                {
					Keep("--- Keep 1 into coined 3 drop ---", Cards.StitchedTracker);
				};
            }



            PrintLog();
            return _keep;
        }

        private bool IsPirateClass(Card.CClass cClass)
        {
            return cClass == Card.CClass.DRUID || cClass == Card.CClass.WARRIOR || cClass == Card.CClass.ROGUE || cClass == Card.CClass.SHAMAN || cClass == Card.CClass.HUNTER;
        }
		private bool IstargetingClass(Card.CClass cClass)
        {
            return cClass == Card.CClass.PRIEST || cClass == Card.CClass.MAGE;
        }

        /// <summary>
        /// Method to fill in his _keep list
        /// </summary>
        /// <param name="reason">Why?</param>
        /// <param name="cards">List of cards he wants to add</param>
        private void Keep(string reason, params Card.Cards[] cards)
        {
            var count = true;
            string str = "--- Keep: ";
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
            str += "\r\nBecause: " + reason;
            AddLog(str);
        }

        /// <summary>
        /// Defines a list of cards as a certain type on card
        /// </summary>
        /// <param name="type">The type of card that you want to define these cards as</param>
        /// <param name="cards">List of cards you want to define as the given type</param>
        private void Define(Defs type, params Card.Cards[] cards)
        {
            _defsDictionary[type] = cards.ToList();
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
            Bot.Log(_log+ "\r\n\r\n---"+ _name + "---");
            _log = "\r\n---"+ _name +"---\r\n";
        }
    }
}
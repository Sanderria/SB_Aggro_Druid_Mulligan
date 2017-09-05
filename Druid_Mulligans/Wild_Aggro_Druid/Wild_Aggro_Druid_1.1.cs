using System;
using System.Collections.Generic;
using System.Linq;
using SmartBot.Database;
using SmartBot.Plugins.API;

#region PatchNotes
/// v1.0, 
	/// Release

/// v1.1, 
	/// Fixed LivingRoots under MarkoftheLotus, buffs. 
	/// Will now keep GolakkaCrawler over HauntedCreeper vs Pirates. 
	/// FireFly and BloodsailCorsair will count as double 1 under double 1 double 2.
	/// 
#endregion

namespace SmartBot.Mulligan
{
    [Serializable]
    public class Druid : MulliganProfile
    {
        private static string _name = "Wild_Aggro_Druid_1.1";
        private string _log = "\r\n---"+ _name +"---\r\n";

        private List<Card.Cards> _choices;

        private readonly List<Card.Cards> _keep = new List<Card.Cards>();

        private readonly Dictionary<Defs, List<Card.Cards>> _defsDictionary = new Dictionary<Defs, List<Card.Cards>>();

        private enum Defs
        {
            One,
            Two
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

            Define(Defs.One, Cards.FireFly, Cards.BloodsailCorsair, Cards.EnchantedRaven, Cards.HungryCrab, Cards.LivingRoots,
				Cards.SirFinleyMrrgglton);
            Define(Defs.Two, Cards.GolakkaCrawler, Cards.DruidoftheSwarm, Cards.DireWolfAlpha, Cards.PoweroftheWild, 
				Cards.HauntedCreeper);
			
            Keep("Always kept", Cards.Innervate, 
				Cards.FireFly, Cards.FireFly, 
				Cards.BloodsailCorsair, Cards.BloodsailCorsair, 
				Cards.EnchantedRaven, Cards.EnchantedRaven, 
				Cards.HungryCrab,
				Cards.LivingRoots,
				Cards.SirFinleyMrrgglton);

			// Vate
            if (_keep.Contains(Cards.Innervate))
            {
				// Vate + Flappy
                Keep("Keep Innervate with Fledgling", Cards.ViciousFledgling);

                //2. Vate
                if (_choices.Contains(Cards.Innervate))
                {
                    //2. Vate + Hydra 
                    if (_choices.Contains(Cards.BittertideHydra))
                    {
                        Keep("Keep double Innervate with Hydra", Cards.Innervate, Cards.BittertideHydra);
                    }
                    //2. Vate + 2. Flappy
                    if (_choices.Contains(Cards.ViciousFledgling))
                    {
                        Keep("Keep double Innervate with double Fledgling", Cards.Innervate, Cards.ViciousFledgling);
                    }
                }
            }

            //1, 2 if enemy class != IsPirateClass
            if (Kept(Defs.One) > 0 && !IsPirateClass(opponentClass))
            {
                Keep("Keep 1 drop into 2 drop", Cards.HauntedCreeper);

				if (Kept(Defs.Two) == 0)
				{
					Keep("Keep 1 drop into 2 drop", Cards.GolakkaCrawler);

					if (Kept(Defs.Two) == 0)
					{
						Keep("Keep 1 drop into 2 drop", Cards.DruidoftheSwarm);

						if (Kept(Defs.Two) == 0)
						{
							Keep("Keep 1 drop into 2 drop", Cards.PoweroftheWild);
						}
					}
				}
            }
			//1, 2 if enemy class == IsPirateClass
			else if (Kept(Defs.One) > 0 && IsPirateClass(opponentClass))
			{
				Keep("Keep 1 drop into 2 drop", Cards.GolakkaCrawler);

				if (Kept(Defs.Two) == 0)
				{
					Keep("Keep 1 drop into 2 drop", Cards.HauntedCreeper);

					if (Kept(Defs.Two) == 0)
					{
						Keep("Keep 1 drop into 2 drop", Cards.DruidoftheSwarm);

						if (Kept(Defs.Two) == 0)
						{
							Keep("Keep 1 drop into 2 drop", Cards.PoweroftheWild);
						}
					}
				}
			}

			//Coin Crab gaming
			else if (IsPirateClass(opponentClass) && coin)
			{
				Keep("Keep Coin into Golakka vs pirate classes", Cards.GolakkaCrawler);
			}

			//Double 1 double 2
			if (Kept(Defs.One) > 1 || _keep.Contains(Cards.FireFly) || _keep.Contains(Cards.BloodsailCorsair)
				&& _keep.Contains(Cards.GolakkaCrawler) || _keep.Contains(Cards.HauntedCreeper))
			{
				Keep("Keep 1 + 1 + 2 + 2", Cards.DruidoftheSwarm, Cards.HauntedCreeper, 
					Cards.PoweroftheWild); //Power here is unnecessary
			}

            //1, 2, 3 I can count
            if (Kept(Defs.One) > 0 && Kept(Defs.Two) > 0)
            {
                Keep("Keep 1, 2, 3 drop curve", Cards.TarCreeper, Cards.ViciousFledgling);
            }

            //1, coin flappy
            if (Kept(Defs.One) > 0 && coin)
            {
                Keep("Keep 1 into Coin and Fledgling", Cards.ViciousFledgling);
            }

            //MarkoftheLotus, buffs
            if (_keep.Contains(Cards.FireFly) || _keep.Contains(Cards.BloodsailCorsair) || Kept(Defs.One) > 1)
            {
                Keep("Keep buffs with minions", Cards.MarkoftheLotus, Cards.DireWolfAlpha, Cards.PoweroftheWild);

            }
			else
			{
				if (_keep.Contains(Cards.LivingRoots) && !coin)
				{
					Keep("Keep buffs with minions", Cards.MarkoftheLotus, Cards.DireWolfAlpha, Cards.PoweroftheWild);
				}
			}

            //Beast synergy
            if (_keep.Contains(Cards.EnchantedRaven) || _keep.Contains(Cards.GolakkaCrawler) 
				|| _keep.Contains(Cards.HungryCrab) || _keep.Contains(Cards.DireWolfAlpha) 
				|| _keep.Contains(Cards.PoweroftheWild) || _keep.Contains(Cards.HauntedCreeper))
            {
                Keep("Keep Mark with beasts", Cards.MarkofYShaarj);
            }

            if (IsDoomsayerClass(opponentClass))
            {
                Keep("Keep Alchemist vs doomsayer classes", Cards.CrazedAlchemist);
            }

            PrintLog();
            return _keep;
        }

        private bool IsPirateClass(Card.CClass cClass)
        {
            return cClass == Card.CClass.DRUID || cClass == Card.CClass.WARRIOR || cClass == Card.CClass.ROGUE || cClass == Card.CClass.SHAMAN;
        }

        private bool IsDoomsayerClass(Card.CClass cClass)
        {
            return cClass == Card.CClass.MAGE || cClass == Card.CClass.PRIEST || cClass == Card.CClass.WARLOCK;
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
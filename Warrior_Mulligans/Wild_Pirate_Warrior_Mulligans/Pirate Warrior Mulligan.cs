using System;
using System.Collections.Generic;
using System.Linq;
using SmartBot.Database;
using SmartBot.Plugins.API;

namespace SmartBot.Mulligan
{
    [Serializable]
    public class PirateWarriorMulligan : MulliganProfile
    {
        private static string _name = "Wild_Pirate_Warrior_v1.0";
        private string _log = "\r\n---" + _name + "---\r\n";

        private List<Card.Cards> _choices;

        private readonly List<Card.Cards> _keep = new List<Card.Cards>();

        private readonly Dictionary<Defs, List<Card.Cards>> _defsDictionary = new Dictionary<Defs, List<Card.Cards>>();

        private enum Defs
        {
            One,
            Two,
            EarlyWeapons
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

            Define(Defs.One, Cards.NZothsFirstMate, Cards.SouthseaDeckhand, Cards.SirFinleyMrrgglton);
            Define(Defs.Two, Cards.BloodsailRaider, Cards.FieryWarAxe, Cards.ShipsCannon);
            Define(Defs.EarlyWeapons, Cards.NZothsFirstMate, Cards.FieryWarAxe, Cards.Upgrade);

            Keep("Always kept", Cards.NZothsFirstMate, Cards.Upgrade, Cards.FieryWarAxe, Cards.BloodsailRaider, Cards.SouthseaDeckhand);

            if (IsPirateClass(opponentClass))
            {
                Keep("Keep Golakka vs pirate classes", Cards.GolakkaCrawler);

                if (opponentClass == Card.CClass.WARRIOR)
                {
                    Keep("Keep 2nd Golakka vs pirate warrior", Cards.GolakkaCrawler);
                }

            }

            if (_keep.Contains(Cards.NZothsFirstMate))
            {
                Keep("Keep 2nd Deckhand with FirstMate", Cards.SouthseaDeckhand);
            }

            if (Kept(Defs.EarlyWeapons) > 0 && (_keep.Contains(Cards.BloodsailRaider) || Kept(Defs.One) > 0))
            {
                Keep("Keep Cultist with early weapon and early pirate", Cards.BloodsailCultist, Cards.BloodsailCultist);
            }

            if (coin && Kept(Defs.One) == 0)
            {
                Keep("Keep Coin and Raider into Raider", Cards.BloodsailRaider);
            }

            if (_keep.Contains(Cards.FieryWarAxe))
            {
                Keep("Keep Corsair with Waraxe", Cards.DreadCorsair, Cards.DreadCorsair);
            }

            if (Kept(Defs.Two) == 0 && HasChoice(Defs.One) >= 3 - Kept(Defs.One))
            {
                PrioKeep("Keep 1 drop into 1 drop and 1 drop", 3 - Kept(Defs.One), Cards.SouthseaDeckhand, Cards.SouthseaDeckhand, Cards.NZothsFirstMate, Cards.NZothsFirstMate);
            }

            if (Kept(Defs.One) >= 3 || Kept(Defs.One) > 0 && Kept(Defs.Two) > 0)
            {
                Keep("Keep 3 drops with good curve", Cards.SouthseaCaptain, Cards.FrothingBerserker, Cards.FrothingBerserker, Cards.SouthseaCaptain);
            }

            PrintLog();
            return _keep;
        }

        private bool IsPirateClass(Card.CClass cClass)
        {
            return cClass == Card.CClass.DRUID || cClass == Card.CClass.WARRIOR || cClass == Card.CClass.ROGUE || cClass == Card.CClass.SHAMAN;
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

        private void PrioKeep(string reason, int numb, params Card.Cards[] cards)
        {
            if (numb > cards.Length) numb = cards.Length;

            var toKeep = new List<Card.Cards>();

            int count = 0;
            int index = 0;

            while (count < numb && index < cards.Length)
            {
                var card = cards[index];
                index++;
                if (_choices.Contains(card))
                {
                    count++;
                    toKeep.Add(card);
                }
            }

            Keep(reason, toKeep.ToArray());
        }

        private void AddLog(string s)
        {
            _log += "\r\n" + s;
        }

        private void PrintLog()
        {
            Bot.Log(_log + "\r\n\r\n---" + _name + "---");
            _log = "\r\n---" + _name + "---\r\n";
        }
    }
}
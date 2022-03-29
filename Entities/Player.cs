using System;
using System.Collections.Generic;

namespace SimplifiedBlackjack
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RiskAversion { get; set; }
        public List<Card> Cards { get; set; }
        public bool IsDealer { get; set; }
        public PlayerStatusEnum Status { get; set; }

        public int TotalHandValue()
        {
            var sum = 0;
            this.Cards.ForEach(x => sum += x.Value);
            return sum;
        }

        public PlayerStatusEnum CheckPlayerStatus()
        {
            if (TotalHandValue() > 21 && !IsDealer)
                return PlayerStatusEnum.Bust;
            else if (TotalHandValue() == 21)
                return PlayerStatusEnum.Blackjack;
            else if((21 - TotalHandValue()) < RiskAversion)
                return PlayerStatusEnum.Stand;
            return PlayerStatusEnum.Hit;
        }
    }

}

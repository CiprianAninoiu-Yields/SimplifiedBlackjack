using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SimplifiedBlackjack
{
    public static class Program
    {
        private static Random rng = new Random();

        public static void Main(string[] args)
        {
            var gameEnded = false;

            var players = GeneratePLayers();

            var deck = GenerateDeck();

            /////  Initial Phase  ////
            // 1. Deck shuffle
            deck = ShuffleDeck(deck);

            var deckIndex = 0;
            // 2. Give initial cards to players
            players.ForEach(player => {
                player.Cards.AddRange(new List<Card>() { deck[deckIndex], deck[deckIndex + 1] });
                deckIndex += 2;

                if (player.TotalHandValue() > 21)
                    player.Status = PlayerStatusEnum.Bust;
                else if (player.TotalHandValue() == 21)
                {
                    player.Status = PlayerStatusEnum.Blackjack;
                    gameEnded = true;
                }
            });

            gameEnded = CheckGameEnded(players);

            // 3. Dealing cards untill game is over
            while (gameEnded == false) {

                players.FindAll(x => x.Status == PlayerStatusEnum.Hit).ForEach(player => {

                    player.Status = player.CheckPlayerStatus();

                    if (player.Status == PlayerStatusEnum.Blackjack)
                        gameEnded = true;

                    else if (player.Status == PlayerStatusEnum.Hit) {
                        player.Cards.Add(deck[deckIndex]);
                        deckIndex += 1;
                    }
                });

                gameEnded = CheckGameEnded(players);
            }

            var dealer = players.Find(x => x.IsDealer);

            // 4. Compute game Results
            players.FindAll(x => !x.IsDealer).ForEach(p => {
                var dif = p.TotalHandValue() - dealer.TotalHandValue();
                if (p.Status == PlayerStatusEnum.Stand)
                    if(dif == 0)
                        p.Status = PlayerStatusEnum.Push;
                    else if(dif > 0)
                        p.Status = PlayerStatusEnum.Win;
                    else if (dif < 0)
                        p.Status = PlayerStatusEnum.Lose;
            });

            GenerateGameResults(players);

            
        }

        public static void GenerateGameResults(List<Player> players)
        {
            var dealer = players.Find(x => x.IsDealer);

            FileStream fs = new FileStream(".\\..\\..\\..\\GameResults\\Results.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            Console.SetOut(sw);
            Console.WriteLine("//////////////////// Game Results ///////////////////////");
            Console.WriteLine("\n");
            Console.WriteLine("  <<DEALER>>");
            Console.WriteLine(DisplayPlayerHand(dealer));

            players.FindAll(x => !x.IsDealer).ForEach(player =>
            {
                Console.WriteLine(" " + player.Name + "(" + player.Status.ToString() + "):");
                Console.WriteLine(DisplayPlayerHand(player));
            });

            Console.WriteLine("////////////////////////////////////////////////////////");
            sw.Close();
        }

        public static string DisplayPlayerHand(Player player)
        {
            var result = "";

            player.Cards.ForEach(card => {
                result = result + "* " + card.Value + " of " + card.Suit + "\n";
            });

            result = result + " - Total Value: " + player.TotalHandValue() + "\n\n";

            return result;
        }

        public static bool CheckGameEnded(List<Player> players)
        {
            var activePlayersCound = 0;
            players.ForEach(p =>
            {
                if (p.Status == 0)
                    activePlayersCound += 1;
            });

            if (activePlayersCound > 0)
                return false;
            return true;
        }

        public static List<Player> GeneratePLayers() {
            return new List<Player>() {
                new Player() {
                    Id = 1,
                    Name = "Adam",
                    RiskAversion = 3,
                    IsDealer = false,
                    Cards = new List<Card>(),
                    Status = PlayerStatusEnum.Hit
                },
                new Player() {
                    Id = 2,
                    Name = "Rowan",
                    RiskAversion = 4,
                    IsDealer = false,
                    Cards = new List<Card>(),
                    Status = PlayerStatusEnum.Hit
                },
                new Player() {
                    Id = 3,
                    Name = "Alan",
                    RiskAversion = 3,
                    IsDealer = false,
                    Cards = new List<Card>(),
                    Status = PlayerStatusEnum.Hit
                },
                new Player() {
                    Id = 4,
                    Name = "Ellie",
                    RiskAversion = 4,
                    IsDealer = false,
                    Cards = new List<Card>(),
                    Status = PlayerStatusEnum.Hit
                },
                new Player() {
                    Id = 4,
                    Name = "Rob",
                    RiskAversion = 4,
                    IsDealer = true,
                    Cards = new List<Card>(),
                    Status = PlayerStatusEnum.Hit
                }
            };
        }
        public static List<Card> GenerateDeck() {

            var deck = new List<Card>();

            for (var i = 0; i <= 3; i++)
                for (var j = 1; j <= 13; j++)
                    deck.Add(new Card()
                    {
                        Value = j == 1 ? 11 : j >= 10 ? 10 : j,
                        Suit = (SuitEnum)i
                    });

            return deck;
        }
        public static List<Card> ShuffleDeck(List<Card> deck)
        {
            int n = deck.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Card value = deck[k];
                deck[k] = deck[n];
                deck[n] = value;
            }
            return deck;
        }
    }
}

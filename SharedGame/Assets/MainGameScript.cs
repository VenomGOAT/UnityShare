using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameScript : MonoBehaviour
{

    public class Player
    {
        public int PlayerNo;
        public List<Card> Cards = new List<Card>();

        public Player(int PlayerNo)
        {
            this.PlayerNo = PlayerNo;
        }

    }

    public class Card
    {
        public string name;
        public string rarity;
        public int value;

        public Card(string name, string rarity, int value)
        {
            this.name = name;
            this.rarity = rarity;
            this.value = value;
        }
    }

    private List<Card> Deck = new List<Card>();
    private List<Player> Players = new List<Player>();

    void Start()
    {
        BuildDeck();
        ShuffleDeck();

        Players.Add(new Player(1));
        Players.Add(new Player(2));

        StartRound();

        
    }

    void BuildDeck()
    {
        Deck.Clear();

        Deck.Add(new Card("Flour", "Common", 1));
        Deck.Add(new Card("Sugar", "Common", 1));
        Deck.Add(new Card("Butter", "Common", 1));

        Deck.Add(new Card("Jam", "Uncommon", 2));
        Deck.Add(new Card("Honey", "Uncommon", 3));
        Deck.Add(new Card("Chocolate", "Uncommon", 3));
        Deck.Add(new Card("Cream", "Uncommon", 3));

        Deck.Add(new Card("Caramel", "Rare", 4));
        Deck.Add(new Card("PeanutButter", "Rare", 4));
        Deck.Add(new Card("Marshmallow", "Rare", 5));

        Deck.Add(new Card("WhiteChocolate", "Legendary", 10));

        Deck.Add(new Card("RoseWater", "Exotic", 15));

        Deck.Add(new Card("AntiMatter", "Galactic", 20));

        //Guys add more cards if you want and move the values/raritys/names around if you disagree with them -> but dont add too many cards because we wont have time to design pixel art for all of them
    }

    void ShuffleDeck()
    {
        for (int i = 0; i < Deck.Count; i++)
        {
            Card temp = Deck[i];
            int rand = Random.Range(i, Deck.Count);
            Deck[i] = Deck[rand];
            Deck[rand] = temp;
        }
    }

    void StartRound()
    {

        foreach (var p in Players)
        {
            p.Cards.Clear();
        }

        foreach (var p in Players)
        {
            DrawCard(p);
            DrawCard(p);
        }
    }

    public void DrawCardButtonPressed()
    {
        DrawCard(Players[0]);
        GetComponent<AudioSource>().Play();
    }

    void DrawCard(Player player)
    {
        if (Deck.Count == 0) return;

        Card card = Deck[0];
        Deck.RemoveAt(0);

        player.Cards.Add(card);
        Debug.Log("Player " + player.PlayerNo + " drew: " + card.name);
    }

    public void EndRound()
    {
        Player HighestPlayer = null;
        int BestScore = -99;

        foreach (var p in Players)
        {
            int score = EvaluateCards(p.Cards);

            if (score > BestScore)
            {
                HighestPlayer = p;
                BestScore = score;
            }

            else if (score == BestScore)
            {
                HighestPlayer = null;
                //tie situation - currently set as nobody wins but should be changed
            } 
        }

        if(HighestPlayer != null)
        {
            Debug.Log("Player" + HighestPlayer.PlayerNo + " Wins the game with score " + BestScore);
        }
        else
        {
            Debug.Log("No winner this round");
        }

        int EvaluateCards(List<Card> Cards)
        {
            int score = 0;
            foreach(var card in Cards) score += card.value;
            //calculates basic value

            bool hasJam = Cards.Exists(c => c.name == "Jam");
            bool hasPeanutButter = Cards.Exists(c => c.name == "PeanutButter");

            if(hasJam && hasPeanutButter) score += 10; //PB+J combo

            //add bonus points for combos -> we can add more combos 

            return score;
        }
    }
}

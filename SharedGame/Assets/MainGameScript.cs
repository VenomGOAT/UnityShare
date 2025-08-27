using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainGameScript : MonoBehaviour
{


    public class Player
    {
        public int PlayerNo;
        public List<Card> Cards = new List<Card>();
        public List<Card> Oven = new List<Card>(); //Added an oven for each player
        public Player(int PlayerNo)
        {
            this.PlayerNo = PlayerNo;
        }

    }

    public class Card
    {
        public string name;
        public string rarity;
        public Card(string name, string rarity, int value)
        {
            this.name = name;
            this.rarity = rarity;
            this.value = value;
        }
    }
    public class Cookie : Card
    {
        public Card[] Ingredients = new Card[3]; 
        public Cookie(string name, string rarity , int value , Card[]Ingredients)
        {
            this.name = name;
            this.rarity = rarity;
            this.value = value;
            this.Ingredients = Ingredients;
        }
    }

    private List<Card> Deck = new List<Card>();
    private List<Player> Players = new List<Player>();

    //Added an alternate shuffling system
    private Card[] CommonCards = { new Card("Flour", "Common", 1), new Card("Sugar", "Common", 1), new Card("Butter", "Common", 1) };
    private Card[] UncommonCards = { new Card("Jam", "Uncommon", 2), new Card("Honey", "Uncommon", 2), new Card("Chocolate", "Uncommon", 2), new Card("Cream", "Uncommon", 2) };
    private Card[] RareCards = { new Card("Caramel", "Rare", 4), new Card("PeanutButter", "Rare", 4), new Card("Marshmallow", "Rare", 4) };
    private Card[] LegendaryCards = { new Card("WhiteChocolate", "Legendary", 10), new Card("PistachioCream", "Legendary", 10), };
    private Card[] WildCards = { new Card("Sprinkles", "Wild", 0), new Card("MilkDunk", "Wild", 0), new Card("Salt", "Wild", 0), new Card("BurntEdge", "Wild", 0), new Card("CookieMonster", "Wild", 0) };
    private Card[] Cookies = { new Cookie("BalancedBiscuit", "Basic", ,{CommonCards[0], CommonCards[1], CommonCards[2]}) ,new Cookie("" , "Cookie" , 20)new Cookie("BalancedBiscuit" , "Cookie" , 20)new Cookie("BalancedBiscuit" , "Cookie" , 20)
    
    
    [Header("UI References")]
    public GameObject cardPrefab;
    public Transform PlayerHandPanel;

    private List<GameObject> UICardObjects = new List<GameObject>();

    void UpdateHandUI(Player player)
    {
        foreach (var obj in UICardObjects)
        {
            Destroy(obj);
        }
        UICardObjects.Clear();

        foreach (var card in player.Cards)
        {
            GameObject newCard = Instantiate(cardPrefab, PlayerHandPanel);
            newCard.name = card.name;
            UICardObjects.Add(newCard);
        }
    }


    void Start()
    {
        BuildDeck();
        Players.Add(new Player(1));
        Players.Add(new Player(2));

        StartRound();
        UpdateHandUI(Players[0]);


    }

    void BuildDeck()
    {
        Deck.Clear();

        for (int i = 1; i <= 20; i++)
        {
            Deck.Add(GenCard());
        }
    }

    //deck is built by randomly adding a card.
    /* Common has 40% chance
     * Uncommon has 30% chance
     * Rare has 15% chance
     * Legendary 10% chance
     * Wild is 5% chance
     */
    Card GenCard()
    {
        int rand = Random.Range(1, 101);
        Card CardGenerated;
        if (rand <= 40)
        {
            CardGenerated = CommonCards[Random.Range(0, CommonCards.Length)];
        }
        else if (rand <= 70)
        {
            CardGenerated = UncommonCards[Random.Range(0, UncommonCards.Length)];
        }
        else if (rand <= 85)
        {
            CardGenerated = RareCards[Random.Range(0, RareCards.Length)];
        }
        else if (rand <= 95)
        {
            CardGenerated = LegendaryCards[Random.Range(0, LegendaryCards.Length)];

        }
        else
        {
            CardGenerated = WildCards[Random.Range(0, WildCards.Length)];
        }

        return CardGenerated;
    }


    //ShuffleDeck currently not being used. We can make an ability for it, like shuffling the deck or something
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
        if (Deck.Count > 0 && Players[0].Cards.Count < 7)
        {
            DrawCard(Players[0]);
            UpdateHandUI(Players[0]);
            GetComponent<AudioSource>().Play();
        }

    }

    void DrawCard(Player player)
    {
        Card card = Deck[0];
        Deck.RemoveAt(0);

        player.Cards.Add(card);
        Debug.Log("Player " + player.PlayerNo + " drew: " + card.name);
    }
    
    public void BakeButtonPressed()
    {
        Bake(Players[0]);
        UpdateHandUI(Players[0]);

    }
    void Bake(Player player)
    {
        
        //First the user clicks bake
        //Then selects the three cards, if it is a set they can bake it
        //If not then invalid and choose a valid pair
        //We then update UI so that the cards are removed and in following turns they can remove the cookie
        
    }

    //We have drawing card capabilities
    //Now we need to play card , bake

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

        if (HighestPlayer != null)
        {
            Debug.Log("Player " + HighestPlayer.PlayerNo + " Wins the game with score " + BestScore);
        }
        else
        {
            Debug.Log("No winner this round");
        }


        int EvaluateCards(List<Card> Cards)
        {
            int score = 0;
            foreach (var card in Cards) score += card.value;
            //calculates basic value

            bool hasJam = Cards.Exists(c => c.name == "Jam");
            bool hasPeanutButter = Cards.Exists(c => c.name == "PeanutButter");

            if (hasJam && hasPeanutButter) score += 10; //PB+J combo

            //add bonus points for combos -> we can add more combos 

            return score;
        }
    }
}

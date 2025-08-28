using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq; 
using static MainGameScript;

public class MainGameScript : MonoBehaviour
{
    public static List<int> SelectedCardIndexes = new List<int>();
    public class Player
    {
        public int PlayerNo;
        public List<Card> Cards = new List<Card>();
        public List<Cookie> Oven = new List<Cookie>(); //Added an oven for each player
        public Player(int PlayerNo)
        {
            this.PlayerNo = PlayerNo;
        }

    }

    public class Card
    {
        public string name;
        public string rarity;
        public int value { get; set; }

        public Card(string name, string rarity, int value)
        {
            this.name = name;
            this.rarity = rarity;
            this.value = value;
        }
    }
    public class Cookie : Card
    {

        public Card[] Ingredients { get; set; }
        
        public Cookie(string name, string rarity, int value) : base(name, rarity, value)
        {
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

    private Cookie[] Cookies = {
            new Cookie("BalancedBiscuit", "Low",0),
            new Cookie("RoyalBiscuit", "Medium",0),
            new Cookie("PBJ", "Medium",0),
            new Cookie("PistachioDelight","High",0)
    };

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

        for (int i = 0; i < player.Cards.Count; i++)
        {
            var card = player.Cards[i];
            GameObject newCard = Instantiate(cardPrefab, PlayerHandPanel);
            newCard.name = card.name;

            ToggleScript toggleScript = newCard.GetComponent<ToggleScript>();
            toggleScript.CardIndex = i;

            UICardObjects.Add(newCard);
        }
    }


    void Start()
    {
        ClickableScript.CanInteract = false;
        BuildCookies();
        BuildDeck();
        Players.Add(new Player(1));
        Players.Add(new Player(2));

        StartRound();
        UpdateHandUI(Players[0]);

    }

    void Update()
    {
        if (SelectedCardIndexes.Count == 1 && Players[0].Cards[SelectedCardIndexes[0]].rarity == "Wild")
        {
            Debug.Log($"Wild Card Selected");
        }
        else if (SelectedCardIndexes.Count >= 2)
        {
            (bool IsRecipe, Cookie CookieToBake) = CheckRecipe(SelectedCardIndexes, Players[0].Cards);
            if (IsRecipe)
            {
                Debug.Log($"{CookieToBake.name} can be baked");
                ClickableScript.CanInteract = true;
            }
        }
    }

    (bool,Cookie) CheckRecipe(List<int> Indexes , List<Card> Cards)
    {
        
        foreach(Cookie cookie in Cookies)
        {
            int IngredientsFound = 0;
            Card[] Ingredients = cookie.Ingredients;
            foreach(int index in Indexes)
            {
                Card Ingredient = Cards[index];
                if (Ingredients.Contains(Ingredient))
                {
                    IngredientsFound++;
                }
            }
            if(IngredientsFound == Ingredients.Length && IngredientsFound == Indexes.Count)
            {
                return (true, cookie);
            }
        }

        return (false, null);
    }
    void BuildCookies()
    {
        Cookies[0].Ingredients = new Card[] { CommonCards[0],CommonCards[1],CommonCards[2]};
        Cookies[1].Ingredients = new Card[] { UncommonCards[1],UncommonCards[3]};
        Cookies[2].Ingredients = new Card[] { RareCards[1], UncommonCards[0]};
        Cookies[3].Ingredients = new Card[] { LegendaryCards[1],UncommonCards[2],CommonCards[0]};

        foreach (var cookie in Cookies)
        {
            int CookieValue = 0;
            int Multiplier = 0;
            foreach(var ingredient in cookie.Ingredients)
            {
                CookieValue += ingredient.value;
            }
            CookieValue /= cookie.Ingredients.Length;

            switch (cookie.rarity)
            {
                case "Low":
                    Multiplier = 4;
                    break;
                case "Medium":
                    Multiplier = 6;
                    break;
                case "High":
                    Multiplier = 8;
                    break;
            }
            
            CookieValue *= Multiplier;
            
            cookie.value = CookieValue;

            Debug.Log($"Cookie : {cookie.name} , Value : {CookieValue}");
        }
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
        else if (rand <=70)
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
        foreach (int index in SelectedCardIndexes)
        {
            if (index >= 0 && index < player.Cards.Count)
            {
                player.Cards.RemoveAt(index);
            }
        }
        SelectedCardIndexes.Clear();
        
        /*Debug.Log($"{CookieToBake.name} is now being baked in the oven");
        SelectedCardIndexes.Clear();
        player.Oven.Add(CookieToBake);*/
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

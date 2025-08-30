using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq; 
using static MainGameScript;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEditor.ShaderKeywordFilter;
using UnityEditor.Experimental.GraphView;

public class MainGameScript : MonoBehaviour
{
    public static List<int> SelectedCardIndexes = new List<int>();
    public static List<int> SelectedCookiesIndexes = new List<int>();
    public static Card SprinkleCardConv;
    public int round = 1;
    public int CookieInOvenClicked = 0;

    public class Player
    {
        public int PlayerNo;
        public List<Card> Cards = new List<Card>();
        public List<Cookie> Oven = new List<Cookie>(); //Added an oven for each player
        public List<Cookie> CanBake = new List<Cookie>();
        public bool MilkDunkActive = false;
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
        public int RoundsBaked { get; set; }
        
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
    private Card[] WildCards = { new Card("Sprinkles", "Wild" , 0) , new Card("MilkDunk", "Wild" , 0), new Card("BurntEdge","Wild",0), new Card("Salt","Wild",0),new Card("CookieMonster", "Wild", 0) };

    private Cookie[] Cookies = {
            new Cookie("BalancedBiscuit", "Low",0),
            new Cookie("RoyalBiscuit", "Medium",0),
            new Cookie("PBJ", "Medium",0),
            new Cookie("PistachioDelight","High",0)
    };

    
    public GameObject cardPrefab;

    [Header("Cookie Prefabs")]

    public GameObject BBPrefab;
    public GameObject RBPrefab;
    public GameObject PBPrefab;
    public GameObject PDPrefab;

    private Dictionary<string, GameObject> CookiePrefabMap;

    [Header("UI References")]
    public Transform PlayerHandPanel;
    public Transform PlayerOvenPanel;
    public Button AbilityButton;

    private List<GameObject> UICardObjects = new List<GameObject>();


    void UpdateHandUI(Player player)
    {
        MainGameScript.SelectedCardIndexes.Clear();

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

    private List<GameObject> UIOvenObjects = new List<GameObject>();

    void UpdateOvenUI(Player player)
    {
        foreach (var obj in UIOvenObjects)
        {
            Destroy(obj);
        }
        UIOvenObjects.Clear();

        for (int i = 0; i < player.Oven.Count; i++)
        {
            var cookie = player.Oven[i];
            if (CookiePrefabMap.TryGetValue(cookie.name, out GameObject prefab))
            {
                GameObject newcookie = Instantiate(prefab, PlayerOvenPanel);
                newcookie.name = cookie.name;
                UIOvenObjects.Add(newcookie);
            }
            else
            {
                Debug.Log($"No prefab found for cookie");
            }

        }
    }


    void Start()
    {
        CookiePrefabMap = new Dictionary<string, GameObject>()
        {
            { "BalancedBiscuit", BBPrefab },
            { "RoyalBiscuit", RBPrefab },
            { "PBJt", PBPrefab },
            { "PistachioDelight", PDPrefab },
            
        };

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
        if (Players[0].Oven.Count != 0)
        {
            foreach(var cookie in UIOvenObjects)
            {
                CookieToogleUI.toggle.interactable = false;
            }
        }

        Players[0].CanBake.Clear();
        if (SelectedCookiesIndexes.Count > 0)
        {
            foreach (var card in UICardObjects)
            {
                ToggleScript.toggle.interactable = false;
            }
        }
        else
        {
            foreach (var card in UICardObjects)
            {
                ToggleScript.toggle.interactable = true;
            }
        }

        if (SelectedCardIndexes.Count == 1)
        {
            int index = SelectedCardIndexes[0];

            if (index >= 0 && index < Players[0].Cards.Count)
            {
                if (Players[0].Cards[index].rarity == "Wild")
                {
                    //Debug.Log($"Wild Card Selected");

                }
            }
            else
            {
                
                SelectedCardIndexes.Clear();
            }
        }
        else if (SelectedCardIndexes.Count >= 2)
        {
            List<Card> IngredientsSelected = new List<Card>();
            foreach (int index in SelectedCardIndexes)
            {
                IngredientsSelected.Add(Players[0].Cards[index]);
            }

            (bool IsRecipe, Cookie CookieToBake) = CheckRecipe(IngredientsSelected);
            if (IsRecipe)
            {
                Players[0].CanBake.Add(CookieToBake);
                ClickableScript.CanInteract = true;
                //Debug.Log($"Can bake {Players[0].CanBake[0].name}");
            }
        }
        else
        {
            if (Players[0].Oven.Count != 0)
            {
                foreach (var cookie in UIOvenObjects)
                {
                    CookieToogleUI.toggle.interactable = true;
                }
            }

            ClickableScript.CanInteract = false;
        }

        
    }

    (bool,Cookie) CheckRecipe(List<Card> IngredientsSelected)
    {
        
        foreach(Cookie cookie in Cookies)
        {
            int IngredientsFound = 0;
            Card[] Ingredients = cookie.Ingredients;
            foreach(Card Ingredient in IngredientsSelected)
            {
                if (Ingredients.Contains(Ingredient))
                {
                    IngredientsFound++;
                }
            }
            if(IngredientsFound == Ingredients.Length && IngredientsFound == IngredientsSelected.Count)
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

        for (int i = 1; i <= 50; i++)
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
    void AIMakesATurn()
    {
        //put the AI in here, current system is a dummy, used just for testing. 
        (bool HasCookie, Cookie AICookie) = AICanMakeCookie(Players[1].Cards);
        Debug.Log($"AI can make cookie {HasCookie} , {AICookie}");
        if (HasCookie)
        {
            Players[1].CanBake.Add(AICookie);
            Bake(Players[1]);
        }
        else if (Players[1].Cards.Count < 8)
        {
            DrawCard(Players[1]);
        }
        else
        {
            DeleteCard(Players[1], 0);
        }
        round++;
    }

    (bool CanMakeCookie , Cookie AICookie) AICanMakeCookie(List<Card> Cards)
    {
        foreach(Cookie cookie in Cookies)
        {
            int IngredientsFound = 0;
            foreach(Card Ingredient in cookie.Ingredients)
            {
                if(Cards.Contains(Ingredient)) IngredientsFound++;
            }
            if(IngredientsFound == cookie.Ingredients.Length)
            {
                return (true, cookie);
            }
        }

        return (false, null);
    }
    public void DrawCardButtonPressed()
    {
        if (Deck.Count > 0 && Players[0].Cards.Count < 7)
        {
            DrawCard(Players[0]);
            UpdateHandUI(Players[0]);
            GetComponent<AudioSource>().Play();
        }
        //If a player draws card but hand is full it skips their go. It should not let ai go, it should tell the user  to play a card or do something else 
        AIMakesATurn();
    }

    void DrawCard(Player player)
    {
        if (Deck.Count > 0)
        {
            Card card = Deck[0];
            Deck.RemoveAt(0);

            player.Cards.Add(card);
            Debug.Log("Player " + player.PlayerNo + " drew: " + card.name);
        }
        else
        {
            EndRound();
        }

        
    }

    public void TakeOutButtonPressed()
    {
        TakeOutOven(Players[0]);
        UpdateHandUI(Players[0]);
        UpdateOvenUI(Players[0]);
    }

    public void BakeButtonPressed()
    {
        GetComponent<AudioSource>().Play();
        Bake(Players[0]);

        UpdateHandUI(Players[0]);
        AIMakesATurn();

    }

    void Bake(Player player)
    {

        player.Oven.Add(player.CanBake[0]);


        if (player.PlayerNo == 1)
        {
            foreach (int index in SelectedCardIndexes)
            {
                DeleteCard(Players[0], index);
            }
            SelectedCardIndexes.Clear();
        }
        else
        {
            foreach (Card Ingredient in player.CanBake[0].Ingredients)
            {
                player.Cards.Remove(Ingredient);
            }
        }

        player.Oven.Last().RoundsBaked = round;
        player.CanBake.Clear();
        Debug.Log($"{player.Oven.Last()} is now being baked in the oven for player {player.PlayerNo}");

        if (player.PlayerNo == 1)
        {
            UpdateOvenUI(player);
        }
    }

    void TakeOutOven(Player player)
    {
        if (player.PlayerNo == 1)
        {
            foreach (int index in SelectedCookiesIndexes)
            {
                Cookie CookieInOven = player.Oven[index];
                CookieInOven.RoundsBaked = round - CookieInOven.RoundsBaked;
                CookieInOven.value = (int)(CookieInOven.value * (1 + (double)(CookieInOven.RoundsBaked / 100)));
                player.Cards.Add(CookieInOven);
                player.Oven.RemoveAt(index);
            }

            UpdateOvenUI(player);
        }
        else
        {
            foreach(Cookie cookie in player.Oven)
            {
                if(round -  cookie.RoundsBaked > 50)
                {
                    cookie.value = (int)(cookie.value * (1 + (double)(cookie.RoundsBaked / 100)));
                    player.Cards.Add(cookie);
                    player.Oven.Remove(cookie);
                }
            }
        }
    }
    public void UseAbilityButtonPressed()
    {
        GetComponent<AudioSource>().Play();
        if (SelectedCardIndexes.Count == 1)
        {
            int index = SelectedCardIndexes[0];
            Card SelectedCard = Players[0].Cards[index];

            if (SelectedCard.rarity == "Wild")
            {
                UseAbility(Players[0], SelectedCard);
            }
            else
            {
                DeleteCard(Players[0], index);
            }
        }
        UpdateHandUI(Players[0]);
        AIMakesATurn();
    }
    void UseAbility(Player player, Card WildCard)
    {
        switch (WildCard.name)
        {
            case "Sprinkles":
                Debug.Log("Sprinkles Ability");
                Sprinkles(player);
                break;

            case "MilkDunk":
                Debug.Log("MilkDunk Ability");
                MilkDunk(player);
                break;

            case "CookieMonster":
                Debug.Log("CookieMonster Ability");
                CookieMonster(player);
                break;
            case "Salt":
                Debug.Log("Salt ability");
                Salt(player);
                break;
            default:
                Debug.Log($"{WildCard.name} -> ability not implemented yet");
                break;
        }
        player.Cards.Remove(WildCard);

    }

    void Salt(Player player)
    {
        int OtherPlayerNo = 0;

        for (int i = 0; i < player.Cards.Count; i++)
        {
            if (player.Cards[i].name == "Salt")
            {
                player.Cards.RemoveAt(i);
            }
        }

        if (player.PlayerNo == 1)
        {
            OtherPlayerNo = 1;
        }

        Players[OtherPlayerNo].Cards.RemoveAt(Random.Range(0,Players[OtherPlayerNo].Cards.Count));
        UpdateHandUI(Players[0]);
    }

    void Sprinkles(Player player)
    {
        //User will click which one they want and then confirm then it will change to that
        player.Cards.Add(SprinkleCardConv);
        //Remove Sprinkles Card
    }
    void MilkDunk(Player player)
    {
        player.MilkDunkActive = true;
    }

    void CookieMonster(Player player)
    {
        int OtherPlayerNo = 0;

        for (int i = 0; i < player.Cards.Count; i++)
        {
            if (player.Cards[i].name == "CookieMonster")
            {
                player.Cards.RemoveAt(i);
            }
        }

        if (player.PlayerNo == 1)
        {
            OtherPlayerNo = 1;
        }

        


        if (Players[OtherPlayerNo].Oven.Count == 0)
        {
            Debug.Log($"Oooooo, unlucky. No cookie to steal");
        }
        else if (Players[OtherPlayerNo].MilkDunkActive)
        {
            Debug.Log($"Player {Players[OtherPlayerNo].PlayerNo} has MilkDunk and protected from the Cookie Monster");
            Players[OtherPlayerNo].MilkDunkActive = false;
        }
        else
        {

            int CookieIndexGen = Random.Range(0, Players[OtherPlayerNo].Oven.Count);

            Cookie CookieTaken = Players[OtherPlayerNo].Oven[CookieIndexGen];

            Debug.Log($"{CookieTaken.name} has been stolen");

            CookieTaken.RoundsBaked = round - CookieTaken.RoundsBaked;
            CookieTaken.value = (int)(CookieTaken.value * (1 + (double)(CookieTaken.RoundsBaked / 100)));

            player.Cards.Add(CookieTaken);
            Players[OtherPlayerNo].Oven.RemoveAt(CookieIndexGen);

            Debug.Log($"Player {Players[OtherPlayerNo].PlayerNo}'s {CookieTaken.name}({CookieTaken.value}) has been taken");
        }

        
        UpdateOvenUI(Players[0]);
        UpdateHandUI(Players[0]);

    }

    void DeleteCard(Player player, int index)
    {
        if (index >= 0 && index < player.Cards.Count)
        {
            player.Cards.RemoveAt(index);
        }

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
            Debug.Log("It is the tie. Both players have gotten the same score");
        }


        int EvaluateCards(List<Card> Cards)
        {
            int score = 0;
            foreach (var card in Cards) score += card.value;
            //calculates basic value    
            return score;
        }
    }
}

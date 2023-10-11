using System;
using System.Collections.Generic;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;

class Program
{
    static List<Player> players = new List<Player>();
    static IFirebaseClient client;

    static void Main(string[] args)
    {
        IFirebaseConfig ifc = new FirebaseConfig()
        {
            AuthSecret = "AIzaSyBs67LbVVFcxCEJZkhYpuwQGbEPKJlleX8",
            BasePath = "https://les1-9b58c-default-rtdb.europe-west1.firebasedatabase.app"
        };
        client = new FireSharp.FirebaseClient(ifc);

        RegisterPlayers();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Виберіть опцію:");
            Console.WriteLine("1. Вхід");
            Console.WriteLine("2. Реєстрація");
            Console.WriteLine("0. Вихід");

            int choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    Login();
                    break;
                case 2:
                    Register();
                    break;
                case 0:
                    return;
                default:
                    Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                    break;
            }
        }
    }

    static void RegisterPlayers()
    {
        players.Add(new Player("user1", "password1", 1000));
        players.Add(new Player("user2", "password2", 1000));
    }

    static void Register()
    {
        Console.Clear();
        Console.Write("Введіть нове ім'я користувача: ");
        string username = Console.ReadLine();

        Console.Write("Введіть пароль: ");
        string password = Console.ReadLine();

        var player = new Player(username, password, 1000);

        players.Add(player);

        SetResponse response = client.Set($"Players/{username}", player);
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            Console.WriteLine("Ви успішно зареєстровані! Натисніть будь-яку клавішу, щоб продовжити...");
        }
        else
        {
            Console.WriteLine("Помилка реєстрації. Спробуйте ще раз.");
        }

        Console.ReadKey();
    }

    static void Login()
    {
        Console.Clear();
        Console.Write("Введіть ім'я користувача: ");
        string username = Console.ReadLine();

        Console.Write("Введіть пароль: ");
        string password = Console.ReadLine();

        FirebaseResponse response = client.Get($"Players/{username}");

        if (response.Body == "null")
        {
            Console.WriteLine("Користувача не знайдено. Спробуйте ще раз.");
        }
        else
        {
            Player player = response.ResultAs<Player>();
            if (player.Password == password)
            {
                PlayGame(player);
            }
            else
            {
                Console.WriteLine("Невірний пароль. Спробуйте ще раз.");
            }
        }

        Console.ReadKey();
    }

    static void PlayGame(Player player)
    {
        Random random = new Random();

        while (player.Money > 0)
        {
            Console.Clear();
            Console.WriteLine($"Ваш баланс: {player.Money} грн");

            Console.Write("Ваша ставка (або 0 для виходу): ");
            
            int bet = Convert.ToInt32(Console.ReadLine());

            if (bet == 0)
                break;

            if (bet > player.Money)
            {
                Console.WriteLine("У вас недостатньо грошей для такої ставки!");
                Console.ReadKey();
                continue;
            }

            int dice1 = random.Next(1, 7);
            int dice2 = random.Next(1, 7);

            Console.WriteLine($"Вам випало {dice1} і {dice2}");

            if (dice1 + dice2 == 7)
            {
                player.Money += bet;
                Console.WriteLine($"Ви виграли {bet} грн!");
            }
            else
            {
                player.Money -= bet;
                Console.WriteLine($"Ви програли {bet} грн.");
            }

            Console.ReadKey();
        }

        Console.WriteLine("Гра закінчена. До побачення!");
        Console.ReadKey();    
    }
}

class Player
{
    public string Username { get; set; }
    public string Password { get; set; }
    public int Money { get; set; }

    public Player(string username, string password, int money)
    {
        Username = username;
        Password = password;
        Money = money;
    }
}

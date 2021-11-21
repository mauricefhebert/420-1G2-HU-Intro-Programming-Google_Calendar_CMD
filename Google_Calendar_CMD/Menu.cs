using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google_Calendar_CMD;

namespace Google_Calendar_CMD
{
    class Menu
    {
        public static string credentialsPath = "client_secret_1064559383118-apihr6eqfg01kn0aqiql42pvio2rph4c.apps.googleusercontent.com.json";

        public static void AfficherMenu()
        {
            Console.WriteLine($"-----MENU-----");
            Console.WriteLine($"1 - Ajouter un evenement");
            Console.WriteLine($"2 - Supprimer un evenement");
            Console.WriteLine($"3 - Afficher les evenements");
            Console.WriteLine($"4 - Rechercher un evenement");
            Console.WriteLine($"5 - Changer de calendrier");
            Console.WriteLine($"6 - Quitter");
        }

        public static void EndMenu()
        {
            Console.WriteLine("\n---MENU FINAL---");
            Console.WriteLine($"1 - Menu principal");
            Console.WriteLine($"2 - Quitter");
            Console.Write("Que voulez-vous faire?: ");
            string menuFianlChoix = Console.ReadLine();
            switch (menuFianlChoix)
            {
                case "1":
                    Console.Clear();
                    AfficherMenu();
                    SelectionMenu();
                    break;
                case "2":
                    Environment.Exit(0);
                    break;
                default:
                    Console.Clear();
                    Console.WriteLine("Choix Invalid veuillez selection parmi la liste");
                    EndMenu();
                    break;
            }
        }

        public static void SelectionMenu()
        {
            GoogleCalendar gCalendar = new GoogleCalendar(credentialsPath);
            Console.Write($"Fait un choix parmi la list suivente: ");
            string userChoice = Console.ReadLine();

            switch (userChoice)
            {
                case "1":
                    Console.Clear();
                    gCalendar.CreateEvent();
                    EndMenu();
                    break;
                case "2":
                    Console.Clear();
                    gCalendar.DeleteEvent();
                    EndMenu();
                    break;
                case "3":
                    Console.Clear();
                    gCalendar.ShowUpCommingEvent();
                    EndMenu();
                    break;
                case "4":
                    Console.Clear();
                    gCalendar.SearchEvent();
                    EndMenu();
                    break;
                case "5":
                    Console.Clear();
                    gCalendar.SelectCalendar();
                    EndMenu();
                    break;
                case "6":
                    Environment.Exit(0);
                    break;
                default:
                    Console.Clear();
                    AfficherMenu();
                    Console.WriteLine("Choix Invalid veuillez selection parmi la liste");
                    SelectionMenu();
                    break;
            }
        }
    }
}

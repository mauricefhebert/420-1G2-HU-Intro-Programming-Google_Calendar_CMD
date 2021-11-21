using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Google_Calendar_CMD
{
    class GoogleCalendar
    {
        public static string[] Scopes = { CalendarService.Scope.Calendar };
        public static string ApplicationName = "CalendarConsole";
        private string CredentialsPath = string.Empty;
        //This will replace the "primary" calendar by a variable of the user choice
        public static string calendarIdSaveFile = "./calenderID.txt";
        private string CalendarInUseId = File.ReadAllText(calendarIdSaveFile);
        private bool ShowEventStayOn = true;
        public GoogleCalendar(string credentialsPath)
        {
            CredentialsPath = credentialsPath;
        }
        //CREATE EVENT
        public void CreateEvent()
        {
            UserCredential credential = GetCredential(UserRole.Admin);
            CalendarService service = GetService(credential);
            
            Console.Write("Entrée un titre: ");
            string titre = Console.ReadLine();
            Console.Write("Entrée une date de début (yyyy-mm-dd): ");
            string dateDebut = Console.ReadLine().Trim();
            if(!dateDebut.Contains("-"))
            {
                Console.WriteLine("Format de date invalide veuillez suivre le format yyyy-mm-dd incluent les trait");
                Console.Write("Entrée une date de début (yyyy-mm-dd): ");
                dateDebut = Console.ReadLine().Trim();
            }
            Console.Write("Entrée une date de fin (yyyy-mm-dd): ");
            string dateFin = Console.ReadLine().Trim();
            if (!dateFin.Contains("-"))
            {
                Console.WriteLine("Format de date invalide veuillez suivre le format yyyy-mm-dd incluent les trait (-)");
                Console.Write("Entrée une date de fin (yyyy-mm-dd): ");
                dateFin = Console.ReadLine().Trim();
            }
            Console.Write("Entrée une heurs de début (hh:mm): ");
            string heursDebut = Console.ReadLine().Trim();
            if (!heursDebut.Contains(":"))
            {
                Console.WriteLine("Format d'heurs invalide veuillez suivre le format hh:mm incluent le deux point (:)");
                Console.Write("Entrée une heurs de début (hh:mm): ");
                heursDebut = Console.ReadLine().Trim();
            }
            Console.Write("Entrée une heurs de fin (hh:mm): ");
            string heursFin = Console.ReadLine().Trim();
            if (!heursFin.Contains(":"))
            {
                Console.WriteLine("Format d'heurs invalide veuillez suivre le format hh:mm incluent le deux point (:)");
                Console.Write("Entrée une heurs de fin (hh:mm): ");
                heursFin = Console.ReadLine().Trim();
            }
            Console.Write("Entrée une description: ");
            string description = Console.ReadLine().Trim();

            Event newEvent = new Event()
            {
                Summary = titre,
                Description = description,
                Start = new EventDateTime()
                {
                    DateTime = DateTime.Parse($"{dateDebut}T{heursDebut}:00"),
                },
                End = new EventDateTime()
                {
                    DateTime = DateTime.Parse($"{dateFin}T{heursFin}:00"),
                },
            };

            string calendarId = CalendarInUseId;

            newEvent = service.Events.Insert(newEvent, calendarId).Execute();
            Console.WriteLine($"{newEvent.HtmlLink}");
        }
        //DELETE EVENT
        public void DeleteEvent()
        {
            ShowEventStayOn = false;
            UserCredential credential = GetCredential(UserRole.Admin);
            CalendarService service = GetService(credential);
            ShowUpCommingEvent();
            Console.Write("Veuillez copier/coller le 'ID' de l'évenement a supprimer: ");
            string idASupprimer = Console.ReadLine().Trim();

            Console.WriteLine("Étez-vous certain de vouloir supprimer cette evenement? o/n: ");
            string ConfirmationSuppresion = Console.ReadLine().ToLower().Trim();
            if(ConfirmationSuppresion == "o")
            {
                service.Events.Delete(CalendarInUseId, idASupprimer).Execute();
            }
        }

        //SHOW EVENT
        public void ShowUpCommingEvent()
        {
            UserCredential credential = GetCredential(UserRole.User);
            CalendarService service = GetService(credential);

            // Define parameters of request
            EventsResource.ListRequest request = service.Events.List(CalendarInUseId);
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = request.Execute();

            // Print upcomming events
            Console.WriteLine($"ÉVENEMENT A VENIR\n");
            if (events.Items != null && events.Items.Count > 0)
            {
                foreach (var eventItem in events.Items)
                {
                    string date = eventItem.Start.DateTime.ToString();
                    string description = eventItem.Description;
                    if (string.IsNullOrEmpty(date))
                    {
                        date = eventItem.Start.Date;
                    }
                    if(!string.IsNullOrEmpty(description))
                    {
                        description = $"Description : {eventItem.Description}\n";
                    }

                    Console.WriteLine($"Titre: {eventItem.Summary}\nDate: {date}\nHeurs de début: \nHeurs de fin: \n{description}ID: {eventItem.Id}\n");
                }
            }
            else
            {
                Console.WriteLine("No Upcoming Event.");
            }
            if(ShowEventStayOn == true)
            {
                Console.Write("Appuyer sur entrée pour continue...");
                Console.ReadLine();
            }
        }
        //SELECT CALENDAR
        public void SelectCalendar()
        {
            ShowEventStayOn = false;
            UserCredential credential = GetCredential(UserRole.Admin);
            CalendarService service = GetService(credential);
            String pageToken = null;
            do
            {
                CalendarList calendarList = service.CalendarList.List().Execute();

                List<CalendarListEntry> items = (List<CalendarListEntry>)calendarList.Items;

                foreach (CalendarListEntry calendarListEntry in items)
                {
                    Console.WriteLine($"Calendar Title: {calendarListEntry.Summary}\nCalendar ID: {calendarListEntry.Id}\n");

                }
                pageToken = calendarList.NextPageToken;
            } while (pageToken != null);

            Console.Write("Veuillez copier/coller le 'ID' du calendrier a utiliser: ");
            string calendrierUtiliser = Console.ReadLine().Trim();
            File.WriteAllText(calendarIdSaveFile, calendrierUtiliser);
            if (ShowEventStayOn == true)
            {
                Console.Write("Appuyer sur entrée pour continue...");
                Console.ReadLine();
            }
        }


        /*--------------------------------------------------------------------------------*/
        //Service and API Call
        private CalendarService GetService(UserCredential credential)
        {
            // Creat Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            return service;
        }

        private UserCredential GetCredential(UserRole userRole)
        {
            UserCredential credential;
            using (var stream =
                new FileStream(CredentialsPath, FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                Scopes,
                userRole.ToString(),
                CancellationToken.None,
                new FileDataStore(credPath, true)).Result;
            }

            return credential;
        }
    }
}
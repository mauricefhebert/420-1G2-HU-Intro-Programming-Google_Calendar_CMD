﻿using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;

namespace Google_Calendar_CMD
{
    class GoogleCalendar
    {
        public static string[] Scopes = { CalendarService.Scope.Calendar };
        public static string ApplicationName = "CalendarConsole";

        private string CredentialsPath = string.Empty;

        public GoogleCalendar(string credentialsPath)
        {
            CredentialsPath = credentialsPath;
        }

        //CREATE EVENT
        public void CreateEvent()
        {
            UserCredential credential = GetCredential(UserRole.Admin);
            CalendarService service = GetService(credential);

            Event newEvent = new Event()
            {
                Summary = "LIFO Event",
                Start = new EventDateTime() { DateTime = new DateTime(2018, 10, 20) },
                End = new EventDateTime() { DateTime = new DateTime(2018, 10, 21) },
            };

            string calendarId = "primary";

            newEvent = service.Events.Insert(newEvent, calendarId).Execute();
            Console.WriteLine($"{newEvent.HtmlLink}");
        }

        //SHOW EVENT
        public void ShowUpCommingEvent()
        {
            UserCredential credential = GetCredential(UserRole.User);

            // Creat Google Calendar API service.
            CalendarService service = GetService(credential);

            // Define parameters of request
            EventsResource.ListRequest request = service.Events.List("primary");
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
                    if (String.IsNullOrEmpty(date))
                    {
                        date = eventItem.Start.Date;
                    }
                    //Console.WriteLine("{0} ({1})", eventItem.Summary, when);
                    Console.WriteLine($"Titre: {eventItem.Summary}\nDate: {date}\nHeurs de début: \nHeurs de fin: \n");
                }
            }
            else
            {
                Console.WriteLine("Nothing.");
            }
            Console.ReadLine();
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
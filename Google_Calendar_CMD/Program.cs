using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google_Calendar_CMD;

namespace Google_Calendar_CMD
{
    class Program
    {

        static void Main(string[] args)
        {
            if (!File.Exists(GoogleCalendar.calendarIdSaveFile))
            {
                File.WriteAllText(GoogleCalendar.calendarIdSaveFile, "primary");
            }
            Menu.AfficherMenu();
            Menu.SelectionMenu();
        }
    }
}

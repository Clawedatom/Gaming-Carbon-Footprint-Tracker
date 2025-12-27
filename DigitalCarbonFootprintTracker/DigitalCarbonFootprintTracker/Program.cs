using DigitalCarbonFootprintTracker;
using System;
using System.Diagnostics;

internal static class Program
{
    static SaveManager saveManager;

    #region Main Loop Methods
    static void Main(string[] args)
    {
        bool running = true;

        saveManager = new SaveManager(Path.Combine(Directory.GetCurrentDirectory(), "SessionData.txt"));


        Console.WriteLine("Gaming Carbon Footprint Tracker (Prototype)");
        Console.WriteLine("Track gaming sessions to increase awareness of digital carbon impact.");
        Console.WriteLine("Press ENTER to continue...");
        Console.ReadLine();
        Console.Clear();

        while (running)
        {
            running = MainMenu();
        }

        Console.WriteLine("Tracker closed. Thank you for using the prototype.");
    }


    static bool MainMenu()
    {
        int optionCount = 6;
        Console.WriteLine("=== Main Menu ===");
        Console.WriteLine("(1) Start Gaming Session");
        Console.WriteLine("(2) View Most Recent Session");
        Console.WriteLine("(3) View Session Summaries");
        Console.WriteLine("(4) View Analytics");
        Console.WriteLine("(5) Sustainability Tips");
        Console.WriteLine("(6) Exit Tracker");

        Console.WriteLine("--------------------------------");
        //sus challenge
        Console.WriteLine("Todays sustainability challenge:");
        Console.WriteLine(GetTodaysChallenge());

        int option = Input_GetOption(optionCount);
        Console.Clear();

        switch (option)
        {
            case 1:
            Option_StartSession();
            break;

            case 2:
            Option_MostRecent();
            break;

            case 3:
            Option_DisplaySummaries();
            break;
            
            case 4:
            Option_ViewAnalytics();
            break;

            case 5:
            Option_DisplaySustainabilityTips();
            break;

            case 6:
            Option_CloseMenu();
            return false;
        }

        ReturnToMenu();

        return true;
    }
    #endregion

    #region Option Methods
    static void ReturnToMenu()
    {
        Console.WriteLine("Press ENTER to return to menu.");
        Console.WriteLine("..");
        Console.ReadLine();
        Console.Clear();
    }

    static void Option_StartSession()
    {
        Console.WriteLine("=== Start Gaming Session ===");


        //get device type
        DeviceType type = Input_GetDeviceType();
        //get game name
        string gameName = Input_GetGameName();
        //get target duration in hours 
        float targetDuration = Input_GetTargetDuration(); // to see if went over or under

        Console.WriteLine("Starting Gaming session playing {0} for {1} hrs.", gameName, targetDuration);

        SessionSummary summary = GameSession(gameName, targetDuration, type);

        Console.WriteLine("Gaming Session has Ended");

        DoContinue();

        Console.WriteLine("Game Session Summary:");
        Console.WriteLine(summary.ToString());

        Console.WriteLine("Saving...");
        saveManager.SaveNewSummary(summary);

        //end

    }

    static void Option_MostRecent()
    {
        Console.WriteLine("=== Most Recent Session ===");

        string recent = saveManager.GetRecentSession();

        if (recent == "") // nothing recent
        {
            Console.WriteLine("No Gaming Sessions Saved.");
            return;
        }
        Console.WriteLine("Recent Session Summary");
        Console.WriteLine(saveManager.GetRecentSession());
    }

    static void Option_DisplaySummaries()
    {
        Console.WriteLine("=== All Session Summaries ===");

        List<SessionSummary> summaries = saveManager.GetAllSessions();

        int page = 0;//refers to ssession summary page
        while (page < summaries.Count)
        {
            Console.WriteLine("Session Summary {0}.", page + 1);

            Console.WriteLine(summaries[page].ToString());
            page++;

            DoContinue();
        }

    

    }

    static void Option_ViewAnalytics()
    {
        Console.WriteLine("===== Analytics =====");

        List<SessionSummary> sessions = saveManager.GetAllSessions();

        if (sessions.Count == 0)
        {
            Console.WriteLine("No session data available.");
            return;
        }

        AnalyticsManager analytics = new AnalyticsManager(sessions);

        

        Console.WriteLine($"Total Sessions: {sessions.Count}");

        Console.WriteLine($"Total Play Time: {analytics.GetTotalPlayTime():0.00} hours");

        Console.WriteLine($"Average Session Length: {analytics.GetAverageSessionLength():0.00} hours");

        Console.WriteLine($"Total Carbon Footprint: {analytics.GetTotalCarbon():0.000} kg CO2");

        Console.WriteLine("\nCarbon by Device:");
        foreach (var kvp in analytics.GetCarbonByDevice())
        {
            Console.WriteLine($"- {kvp.Key}: {kvp.Value:0.000} kg CO2");
        }
    }



    static void Option_DisplaySustainabilityTips()
    {
        Console.WriteLine("=== Sustainability Tips ===");

        string tip = GetSustainTip();
        
        Console.WriteLine(tip);




    }

    static void Option_CloseMenu()
    {
        Console.WriteLine("=== Closing Menu ===");

        
    }
    #endregion


    #region Sustainability

    static string GetSustainTip()
    {
        string tipsPath = Path.Combine(Directory.GetCurrentDirectory(),"SustainabilityTips.txt");

        string[] tips = File.ReadAllLines(tipsPath);
        Random rng = new Random();
        string tip = tips[rng.Next(tips.Length)];
        return tip;
    }
    static string GetTodaysChallenge()
    {
        string path = Path.Combine(Directory.GetCurrentDirectory(),"SustainabilityChallenges.txt");

        if (!File.Exists(path))
            return "No sustainability challenges available.";

        string[] challenges = File.ReadAllLines(path);

        if (challenges.Length == 0)
            return "No sustainability challenges available.";

        int dayIndex = DateTime.Today.DayOfYear;
        int challengeIndex = dayIndex % challenges.Length;

        return challenges[challengeIndex];
    }


    #endregion

    #region Input Methods
    static void DoContinue()
    {
        Console.WriteLine("Press ENTER to continue");
        Console.WriteLine("...");
        Console.ReadLine();
        Console.Clear();
    }

    static int Input_GetOption(int optionCount)
    {
        bool isValid = false;
        int intInput = 0;

        while (!isValid)
        {
            try
            {
                string inpt = Console.ReadLine();

                bool isInt = int.TryParse(inpt, out intInput);

                if (isInt && intInput <= optionCount && intInput > 0)
                {
                    isValid = true;
                    Console.WriteLine("Selected Option {0}.", intInput);
                }
                else
                {
                    throw new Exception("Not a valid option. Please Enter A Number Between 1 - "+ optionCount + " And Try Again");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        DoContinue();

        return intInput;
    }

    static string Input_GetGameName()
    {
        string name = "";
        bool isValid = false;

        while (!isValid)
        {
            try
            {
                Console.WriteLine("Enter the game you are playing during the session.");
                name = Console.ReadLine();

                if (string.IsNullOrEmpty(name))
                {
                    throw new Exception("Invalid game name.");
                }

                Console.WriteLine("Game Name: {0}", name);
                isValid = true;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        DoContinue();

        return name;
    }

    static float Input_GetTargetDuration()
    {
        float targetDuration = 0.0f;
        bool isValid = false;

        while (!isValid)
        {
            try
            {
                Console.WriteLine("Enter how long you aim to play for, in hours e.g 1.5 hrs = 90 mins.");
                string inpt = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(inpt))
                {
                    throw new Exception("Invalid time.");
                }

                
                if (!float.TryParse(inpt, out targetDuration))
                {
                    throw new Exception("Please enter a valid number.");
                }

                if (targetDuration <= 0)
                {
                    throw new Exception("Time must be greater than zero.");
                }

                Console.WriteLine("Target Duration: {0} hours", targetDuration);
                isValid = true;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        DoContinue();

        return targetDuration;
    }

    static DeviceType Input_GetDeviceType()
    {
        bool isValid = false;
        DeviceType type = DeviceType.Desktop;

        while (!isValid)
        {
            try
            {
                Console.WriteLine("Enter the type of device you will play on:");

                // Dynamically list all device types
                var deviceValues = Enum.GetValues<DeviceType>();
                for (int i = 0; i < deviceValues.Length; i++)
                {
                    Console.WriteLine($"({i + 1}) {deviceValues[i]}");
                }

                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    throw new Exception("Input cannot be empty.");
                }

                // Try parse as number
                if (int.TryParse(input, out int choice))
                {
                    if (choice >= 1 && choice <= deviceValues.Length)
                    {
                        type = deviceValues[choice - 1];
                        isValid = true;
                    }
                    else
                    {
                        throw new Exception("Number out of range.");
                    }
                }
                else
                {
                    // Try parse as enum name
                    if (Enum.TryParse<DeviceType>(input, true, out DeviceType device))
                    {
                        type = device;
                        isValid = true;
                    }
                    else
                    {
                        throw new Exception("Invalid device type name.");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        Console.WriteLine("Device Type: {0},  {1} kWh/hour", type, SessionSummary.GetkWhFromDevice(type));
        DoContinue(); 
        return type;
    }

    #endregion

    #region Session Methods
    private static SessionSummary GameSession(string name, float targetDuration, DeviceType type)
    {
        SessionSummary sessionSummary = new SessionSummary(name, targetDuration, type);

        Console.WriteLine("Session started. Press ENTER to stop.");

        Stopwatch stopwatch = Stopwatch.StartNew();

        var inputTask = Task.Run(() => Console.ReadLine());

        while (!inputTask.IsCompleted)
        {
            TimeSpan t = stopwatch.Elapsed;

            Console.Write($"\rTimer: {t.Hours:D2}:{t.Minutes:D2}:{t.Seconds:D2}");

            Thread.Sleep(250); 
        }

        stopwatch.Stop();

        Console.WriteLine(); 

        float sessionHours = (float)stopwatch.Elapsed.TotalHours;
        sessionSummary.OnSessionEnd(sessionHours);

        return sessionSummary;
    }
    #endregion
}

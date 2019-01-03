using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MySecrets.App.Domains;
using MySecrets.App.UiRenderer;

namespace MySecrets.App.StateManager
{
    

    
    public static class StateKeeper
    {
        
        private static readonly object StateLockObject = new object();
        private static bool _stateHasToBeSynced;

        public static void StateHasToBeSynced()
        {
            lock (StateLockObject)
                _stateHasToBeSynced = true;
        }


 
        private static SettingsModel GetSettingsModel()
        {
            var file = Environment.GetEnvironmentVariable("HOME").AddLastCharIfNotExists('/') + ".my-secrets";

            var settingsJson = File.ReadAllText(file);

            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<SettingsModel>(settingsJson);

            if (result.File.StartsWith('~'))
                result.File = result.File.Replace("~", Environment.GetEnvironmentVariable("HOME"));


            return result;
        }
        
        
        public static void SaveState()
        {
            var stateModel = new StateModel
            {
                Notebooks = NotebooksRepository.Get(),
                Pages = PagesRepository.Get(),
                Content = PageContentRepository.GetAll()
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(stateModel);

            var settingsModel = GetSettingsModel();

            var dataToSave = json.Encrypt(settingsModel);
            
            File.WriteAllBytes(settingsModel.File, dataToSave);
            
            UiNotifications.Notify(ConsoleColor.Red, ConsoleColor.Black, "Saved to Disk...", 5);

            Task.Run(()=>dataToSave.SaveToBlobAsync(settingsModel));

        }


        private static void ReadNewPassPhrase()
        {
            while (true)
            {
                Console.Write("Enter passphrase: ");
                var passphrase = Console.ReadLine();
                Console.Write("Enter passphrase again: ");
                var passphraseAgain = Console.ReadLine();

                if (passphrase == passphraseAgain)
                {                    
                    Encrypter.PassPhrase = Encoding.UTF8.GetBytes(passphrase);
                    return;
                }
                
                Console.WriteLine("Passphrase are not the same. Please try again...");
            } 
        }

        private static void CreateNew(SettingsModel settingsModel)
        {
            Console.WriteLine(settingsModel.File+" is not exist. To create new one - please enter passphrase");

            ReadNewPassPhrase();
            
            SaveState();
            
            Console.WriteLine("State file created. Press any key");
            Console.ReadKey();

        }

        private static string EnterPassword()
        {
            var result = "";
            while (true)
            {
                var key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        return result;
                    
                    case ConsoleKey.Backspace:
                    {
                        if (result.Length > 0)
                            result = result.Substring(0, result.Length - 1);
                        break;
                    }
                }

                result += key.KeyChar;

            }
        }

        private static void LoadFromFile(SettingsModel settingsModel)
        {
            while (true)
            {

                Console.Write("Enter passphrase: ");
                var passPhrase = EnterPassword();
                Encrypter.PassPhrase = Encoding.UTF8.GetBytes(passPhrase);
                
                try
                {
                    var encryptedData = File.ReadAllBytes(settingsModel.File);

                    var json = encryptedData.Decrypt(settingsModel);

                    var stateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<StateModel>(json);
            
                    NotebooksRepository.Init(stateModel.Notebooks);
                    PagesRepository.Init(stateModel.Pages);
                    PageContentRepository.Init(stateModel.Content);
                    return;
                }
                catch (Exception)
                {
                    Console.WriteLine("Invalid passphrase. Please try again...");
                }
                
            }

        }
        
        public static void LoadState()
        {
            var settingsModel = GetSettingsModel();

            if (!File.Exists(settingsModel.File))
                CreateNew(settingsModel);
            else
                LoadFromFile(settingsModel);
        }


        public static void CheckIfStateHasToBeSaved()
        {
            lock (StateLockObject)
            {
                if (!_stateHasToBeSynced)
                    return;
                
                SaveState();

                _stateHasToBeSynced = false;
            }
            
        }
        
    }
}
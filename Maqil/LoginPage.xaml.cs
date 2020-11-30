using AE.Net.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Maqil
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public class UserData
    {
        public string Login;
        public string Pass;
        public string imap;
        public string smtp;
    }
    public sealed partial class LoginPage : Page
    {
        public static List<UserData> user;
        public static XmlSerializer xml2 = new XmlSerializer(typeof(List<UserData>));
        public static string str;
        public static StorageFile file;
        public static StorageFolder folder = ApplicationData.Current.LocalFolder;

        public LoginPage()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            this.InitializeComponent();

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string imapsite = "";
            string stmpsite = "";
            try
            {
                ImapClient ic = new ImapClient("imap.mail.ru", LoginTextBox.Text.ToString(), Passwordbx.Password.ToString(), AuthMethods.Login, 993, true);
                imapsite = "imap.mail.ru";
                stmpsite = "smtp.mail.ru";
            }
            catch
            {
                try
                {
                    ImapClient ic = new ImapClient("imap.gmail.com", LoginTextBox.Text.ToString(), Passwordbx.Password.ToString(), AuthMethods.Login, 993, true);
                    imapsite = "imap.gmail.com";
                    stmpsite = "smtp.gmail.com";
                }
                catch
                {
                    try
                    {
                        ImapClient ic = new ImapClient("imap.yandex.ru", LoginTextBox.Text.ToString(), Passwordbx.Password.ToString(), AuthMethods.Login, 993, true);
                        imapsite = "imap.yandex.ru";
                        stmpsite = "smtp.yandex.ru";
                    }
                    catch
                    {
                        try
                        {
                            ImapClient ic = new ImapClient("imap.rambler.ru", LoginTextBox.Text.ToString(), Passwordbx.Password.ToString(), AuthMethods.Login, 993, true);
                            imapsite = "imap.rambler.ru";
                            stmpsite = "smtp.rambler.ru";
                        }
                        catch
                        {
                            ContentDialog prog = new ContentDialog()
                            {
                                Title = "Ошибка",
                                Content = $"Произошла ошибка: Проверьте вводимые данные",
                                PrimaryButtonText = "OK"
                            };
                            await prog.ShowAsync();
                        }
                    }
                }
            }
            if (imapsite != "")
            {
                file = await folder.CreateFileAsync("UserData.xml", CreationCollisionOption.OpenIfExists);
                Stream stream2 = await file.OpenStreamForWriteAsync();
                UserData us = new UserData()
                {
                    Login = LoginTextBox.Text,
                    Pass = Passwordbx.Password,
                    imap = imapsite,
                    smtp = stmpsite,
                };
                user.Add(us);
                xml2.Serialize(stream2, user);
                stream2.Close();
                this.Frame.Navigate(typeof(MainPage), user);

            }

        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //-------------------------------------------------------------------------------------Десериализация юзеров
            file = await folder.CreateFileAsync("UserData.xml", CreationCollisionOption.OpenIfExists);
            str = await FileIO.ReadTextAsync(file);

            var stream = await file.OpenStreamForReadAsync();
            if (str.Length != 0) user = (List<UserData>)xml2.Deserialize(stream);
            else user = new List<UserData>();

            stream.Close();
            //-------------------------------------------------------------------------------------Проверка, залогинен ли кто лии нет, или это был переход с настроек
            if ((string)e.Parameter == "" && user.Count != 0)
                this.Frame.Navigate(typeof(MainPage), user);

            //-------------------------------------------------------------------------------------Фон
            string filename = "";
            IReadOnlyList<StorageFile> storageFiles = await folder.GetFilesAsync();
            foreach (StorageFile eeeeeclass in storageFiles)
            {
                if (eeeeeclass.Name.Contains(".jpg"))
                    filename = eeeeeclass.Name;
            }

            if (filename != "")
                gridbg.ImageSource = new BitmapImage(new Uri($@"{folder.Path}\{filename}"));
            else
                gridbg.ImageSource = new BitmapImage(new Uri("ms-appx:///Background/background.jpg"));
        }
    }
}

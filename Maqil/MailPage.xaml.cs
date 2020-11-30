using AE.Net.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Maqil
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MailPage : Page
    {
        static MailMessage[] mm;
        static ImapClient ic;
        static string navName;

        public List<UserData> user;
        public int id;
        public XmlSerializer xml2 = new XmlSerializer(typeof(List<UserData>));
        public XmlSerializer xml = new XmlSerializer(typeof(int));
        public static string str;
        public StorageFolder folder = ApplicationData.Current.LocalFolder;
        public MailPage()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            this.InitializeComponent();
        }

        private async void ListMail_Loaded(object sender, RoutedEventArgs e)
        {
            //----------------------------------------------------------------------------------------------Десериализация юзера 
            var file = await folder.CreateFileAsync("UserData.xml", CreationCollisionOption.OpenIfExists);
            str = await FileIO.ReadTextAsync(file);

            var stream = await file.OpenStreamForReadAsync();
            if (str.Length != 0) user = (List<UserData>)xml2.Deserialize(stream);
            else user = new List<UserData>();

            stream.Close();
            //----------------------------------------------------------------------------------------------Десериализация айдишника 

            file = await folder.CreateFileAsync("ID.xml", CreationCollisionOption.OpenIfExists);
            str = await FileIO.ReadTextAsync(file);

            var stream1 = await file.OpenStreamForReadAsync();
            if (str.Length != 0) id = (int)xml.Deserialize(stream1);
            else id = 0;

            stream1.Close();

            try
            {
                ic = new ImapClient(user[id].imap, user[id].Login, user[id].Pass, AuthMethods.Login, 993, true);
                var mailbox = ic.ListMailboxes("", "*");

                string[] mailboxes = new string[3];
                if (user[id].imap == "imap.mail.ru")
                    mailboxes = new string[] { "Отправленные", "Черновики", "Корзина", };
                if (user[id].imap == "imap.gmail.com")
                    mailboxes = new string[] { "[Gmail]/Отправленные", "[Gmail]/Черновики", "[Gmail]/Корзина", };
                if (user[id].imap == "imap.yandex.ru")
                    mailboxes = new string[] { "Исходящие", "Черновики", "Удаленные", };
                if (user[id].imap == "imap.rambler.ru")
                    mailboxes = new string[] { "SentBox", "DraftBox", "Trash", };

                if (navName == "Исходящие")
                    ic.SelectMailbox(mailboxes[0]);
                if (navName == "Черновик")
                    ic.SelectMailbox(mailboxes[1]);
                if (navName == "Корзина")
                    ic.SelectMailbox(mailboxes[2]);
                if (navName == "Входящие")
                    ic.SelectMailbox("INBOX");
                if (navName == "")
                    ic.SelectMailbox("INBOX");


                mm = ic.GetMessages(0, ic.GetMessageCount(), false);
                foreach (MailMessage mail in mm)
                {
                    ListBoxItem message = new ListBoxItem();
                    message.Content = mail.Subject;
                    message.RightTapped += (s, ev) =>
                    {
                        MenuFlyout flyout = new MenuFlyout();

                        MenuFlyoutItem answer = new MenuFlyoutItem();
                        answer.Text = "Ответить";
                        flyout.Items.Add(answer);
                        answer.Click += (send, eve) =>
                        {
                            var frame = this.Frame as Frame;
                            frame.Navigate(typeof(EditPage), mail.From.Address.ToString());
                            //ic.DeleteMessage(mail);
                            //ic.Expunge();
                            //var frame = this.Frame as Frame;
                            //frame.Navigate(typeof(MailPage));
                        };

                        FlyoutBase.SetAttachedFlyout(message, flyout);
                        FlyoutBase.ShowAttachedFlyout((FrameworkElement)s);
                    };
                    ListMail.Items.Add(message);
                }

                ic.Dispose();
            }
            catch (Exception ex)
            {
                ContentDialog prog = new ContentDialog()
                {
                    Title = "Ошибка",
                    Content = $"Произошла ошибка: {ex.Message}",
                    PrimaryButtonText = "OK"
                };
                try
                {
                    await prog.ShowAsync();
                }
                catch { };
            }
        }

        private readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ("review", typeof(Review))
        };

        private void ListMail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var frame = this.Frame as Frame;
            frame.Navigate(typeof(Review), mm[ListMail.SelectedIndex]);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e != null)
                navName = (string)e.Parameter;
        }
    }
}

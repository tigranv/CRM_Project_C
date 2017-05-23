using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using System.Net.Http.Formatting;

namespace TestWpfAppForCRM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly HttpClient _client = new HttpClient();
        List<Contact> partnersList;
        List<EmailList> emailListsList;
        List<Contact> emailListPartners;


        public MainWindow()
        {
            InitializeComponent();
        }

        private async void GetAllPartnersBt_Click(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();

            await client.GetAsync(@"http://localhost:56217/api/contacts")
                .ContinueWith(response =>
                {
                    if (response.Exception != null)
                    {
                        MessageBox.Show(response.Exception.Message);
                    }
                    else
                    {
                        HttpResponseMessage message = response.Result;
                        string responseText = message.Content.ReadAsStringAsync().Result;

                        JavaScriptSerializer jss = new JavaScriptSerializer();
                        partnersList = jss.Deserialize<List<Contact>>(responseText);

                        Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        (Action)(() =>
                        {
                            MyGridParters.ItemsSource = partnersList;
                        }));
                    }
                });
        }

        private async void MouseClickOnDatagrid(object sender, MouseButtonEventArgs e)
        {
            HttpClient client = new HttpClient();
            string id = partnersList[MyGridParters.SelectedIndex].ContactId.ToString();
            string url = string.Format(@"http://localhost:56217/api/contacts?id={0}", Uri.EscapeDataString(id));

            await client.GetAsync(url)
                .ContinueWith(response =>
                {
                    if (response.Exception != null)
                    {
                        MessageBox.Show(response.Exception.Message);
                    }
                    else
                    {
                        HttpResponseMessage message = response.Result;
                        string responseText = message.Content.ReadAsStringAsync().Result;

                        JavaScriptSerializer jss = new JavaScriptSerializer();
                        Contact person = jss.Deserialize<Contact>(responseText);

                        string mailingLists = "";
                        foreach (var item in person.EmailLists)
                        {
                            mailingLists += item.EmailListName + ",  ";
                        }

                        Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        (Action)(() =>
                        {
                            PartnerInfo.Text = $"Partner name - { person.FullName}\nCompany - {person.CompanyName}\nMailing Lists is {mailingLists} ";
                        }));
                    }
                });
        }

        private void UpdateSelectedbutton_Click(object sender, RoutedEventArgs e)
        {
            if (MyGridParters.SelectedItems.Count != 1) return;
            HttpClient client = new HttpClient();

            Contact p = new Contact()
            {
                ContactId = partnersList[MyGridParters.SelectedIndex].ContactId,
                FullName = PartnerNameTextbox.Text,
                CompanyName = CompanyNameTextbox.Text,
                Country = CountryNameTextbox.Text,
                Position = PositionTextbox.Text,
                Email = EmailTextbox.Text,
            };

            HttpResponseMessage response = client.PutAsync(@"http://localhost:56217/api/contacts", p, new JsonMediaTypeFormatter()).Result;
            string message = response.Content.ReadAsStringAsync().Result;
            JavaScriptSerializer jss = new JavaScriptSerializer();
        }

        private void Createbt_Click(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();

            Contact p = new Contact()
            {
                FullName = PartnerNameTextbox.Text,
                CompanyName = CompanyNameTextbox.Text,
                Country = CountryNameTextbox.Text,
                Position = PositionTextbox.Text,
                Email = EmailTextbox.Text,
            };

            HttpResponseMessage response = client.PostAsync(@"http://localhost:56217/api/contacts", p, new JsonMediaTypeFormatter()).Result;
            string message = response.Content.ReadAsStringAsync().Result;
            JavaScriptSerializer jss = new JavaScriptSerializer();
        }

        private void UploadFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Deletebt_Click(object sender, RoutedEventArgs e)
        {
            if (MyGridParters.SelectedItems.Count == 1)
            {
                HttpClient client = new HttpClient();
                string id = partnersList[MyGridParters.SelectedIndex].ContactId.ToString();
                string url = string.Format(@"http://localhost:56217/api/contacts?id={0}", Uri.EscapeDataString(id));

                client.DeleteAsync(url);
            }
        }

        private async void getEmailLists_Click(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();

            await client.GetAsync(@"http://localhost:56217/api/emaillists")
                .ContinueWith(response =>
                {
                    if (response.Exception != null)
                    {
                        MessageBox.Show(response.Exception.Message);
                    }
                    else
                    {
                        HttpResponseMessage message = response.Result;
                        string responseText = message.Content.ReadAsStringAsync().Result;

                        JavaScriptSerializer jss = new JavaScriptSerializer();
                        emailListsList = jss.Deserialize<List<EmailList>>(responseText);

                        Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        (Action)(() =>
                        {
                            MyGridEmailLists.ItemsSource = emailListsList;
                        }));
                    }
                });
        }

        private async void EmailListDetails_Click(object sender, MouseButtonEventArgs e)
        {
            HttpClient client = new HttpClient();
            string id = emailListsList[MyGridEmailLists.SelectedIndex].EmailListID.ToString();
            string url = string.Format(@"http://localhost:56217/api/emaillists?id={0}", Uri.EscapeDataString(id));

            await client.GetAsync(url)
                .ContinueWith(response =>
                {
                    if (response.Exception != null)
                    {
                        MessageBox.Show(response.Exception.Message);
                    }
                    else
                    {
                        HttpResponseMessage message = response.Result;
                        string responseText = message.Content.ReadAsStringAsync().Result;

                        JavaScriptSerializer jss = new JavaScriptSerializer();
                        EmailList emailList = jss.Deserialize<EmailList>(responseText);
                        emailListPartners = null;
                        emailListPartners = emailList.Contacts.ToList();



                        Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        (Action)(() =>
                        {
                            emaillistOptionsTextBlock.Text = $"{emailList.EmailListName} - Email List Contacts";
                            MyGridListOptions.ItemsSource = emailListPartners;
                        }));
                    }
                });
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Web.Script.Serialization;
using System.Net.Http.Formatting;

namespace TestWpfAppForCRM
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _client = new HttpClient();
        List<MyContact> partnersList;
        List<MyEmailList> emailListsList;
        Dictionary<Guid, string> emailListPartners;


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
                        partnersList = jss.Deserialize<List<MyContact>>(responseText);

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
            string id = partnersList[MyGridParters.SelectedIndex].GuID.ToString();
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
                        MyContact person = jss.Deserialize<MyContact>(responseText);

                        string mailingLists = "";
                        foreach (var item in person.EmailLists)
                        {
                            mailingLists += item.Value+ ", ";
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

            MyContact p = new MyContact();
            p.GuID = partnersList[MyGridParters.SelectedIndex].GuID;
            p.EmailLists = partnersList[MyGridParters.SelectedIndex].EmailLists;
            p.FullName = PartnerNameTextbox.Text;
            p.CompanyName = CompanyNameTextbox.Text;
            p.Country = CountryNameTextbox.Text;
            p.Position = PositionTextbox.Text;
            p.Email = EmailTextbox.Text;
            p.DateInserted = partnersList[MyGridParters.SelectedIndex].DateInserted;


            HttpResponseMessage response = client.PutAsync(@"http://localhost:56217/api/contacts", p, new JsonMediaTypeFormatter()).Result;
            string message = response.Content.ReadAsStringAsync().Result;
            JavaScriptSerializer jss = new JavaScriptSerializer();
            MessageBox.Show(message);
        }

        private void Createbt_Click(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();

            MyContact p = new MyContact();
            p.FullName = PartnerNameTextbox.Text;
            p.CompanyName = CompanyNameTextbox.Text;
            p.Country = CountryNameTextbox.Text;
            p.Position = PositionTextbox.Text;
            p.Email = EmailTextbox.Text;

            HttpResponseMessage response = client.PostAsync(@"http://localhost:56217/api/contacts", p, new JsonMediaTypeFormatter()).Result;
            string message = response.Content.ReadAsStringAsync().Result;
        }

        private void UploadFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Deletebt_Click(object sender, RoutedEventArgs e)
        {
            if (MyGridParters.SelectedItems.Count == 1)
            {
                HttpClient client = new HttpClient();
                string id = partnersList[MyGridParters.SelectedIndex].GuID.ToString();
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
                        emailListsList = jss.Deserialize<List<MyEmailList>>(responseText);

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
                        MyEmailList emailList = jss.Deserialize<MyEmailList>(responseText);
                        emailListPartners = null;
                        emailListPartners = emailList.Contacts;
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

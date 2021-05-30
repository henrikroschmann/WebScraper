using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WebScraper.Scraper.Models;
using GroupItem = WebScraper.Scraper.Models.GroupItem;

namespace WebScraper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {

            var output = new List<Scraper.Models.Scraper>();

            // Get links from page 
            var stockLinks = new Scraper.Scraper(new Scraper.Models.Scraper()
            {
                Url = "https://epaccess.penser.se/",
                Scope = new Scope()
                {
                    From = "<div class=\"submenu-items\">",
                    To = "</div> <!-- .submenu-items -->"
                },
                Groups = new List<GroupItem>
                {
                    new()
                    {
                        From = "<a",
                        To = "</a",
                        GroupItemType = GroupItemType.Loop,
                        Extraction = new GroupItem()
                        {
                            From = "href=\"",
                            To = "\""
                        }
                    }
                }
            });
            var resultStockLinks = stockLinks.GetData();

            // Get elements from links in first result
            foreach (var rGroup in resultStockLinks.Groups)
            {
                foreach (var rule in rGroup.Extraction.Result.Select(res => new Scraper.Models.Scraper()
                {
                    Url = res + "analys/",
                    Scope = new Scope()
                    {
                        From = "<table class=\"reports-list\">",
                        To = "</table"
                    },
                    Groups = new List<GroupItem>()
                    {
                        new()
                        {
                            From = "<tr class=\"report-row\"",
                            To = "</tr>",
                            GroupItemType = GroupItemType.Loop,
                            Extraction = new GroupItem()
                            {
                                From = "<td>",
                                To = "</td>",
                                GroupItemType = GroupItemType.Loop
                            }
                        }
                    }
                }).Select(exRule => new Scraper.Scraper(exRule)).Select(_a => _a.GetData()))
                {
                    output.Add(rule);
                }

                Console.WriteLine(rGroup.Extraction.Result);
            }
        }

        private void LogOutput_TextChanged(object sender, TextChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void MenuFileExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void NewRecipe_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
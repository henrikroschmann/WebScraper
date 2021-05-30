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
            var toScraper = new Scraper.Models.Scraper()
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
            };
            var a = new Scraper.Scraper(toScraper);
            var result = a.Scrape();
            foreach (var rGroup in result.Groups)
            {
                foreach (var result2 in rGroup.Extraction.Result.Select(res => new Scraper.Models.Scraper()
                {
                    Url = res + "analys/",
                    Scope = new Scope()
                    {
                        From = "<table class=\"reports-list\">",
                        To = "</table"
                    },
                    Groups = new List<GroupItem>()
                    {
                        new GroupItem
                        {
                            From = "<tr class=\"report-row\"",
                            To = "</tr>",
                            GroupItemType = GroupItemType.Loop,
                            Extraction = new GroupItem()
                            {
                                From = "href=\"",
                                To = "\"",
                                GroupItemType = GroupItemType.Loop
                            }
                        }
                    }
                }).Select(anal => new Scraper.Scraper(anal)).Select(_a => _a.Scrape()))
                {
                    Console.WriteLine();
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WebScraper.Scraper;
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
            Logger.WriteEvent += WriteEventHandler;
            Logger.WriteLine("Logger initialized");
        }

        #region LogInformation

        /// <summary>
        ///     Write to the logger
        ///     To handle event from other treads we need dispatching.
        /// </summary>
        /// <param name="message"></param>
        private void WriteEventHandler(string message)
        {
            LogOutput.Dispatcher.BeginInvoke(new Action(() => LogOutput.AppendText(message)));
        }

        /// <summary>
        ///     Text change event for to show the most recent log
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogOutput_TextChanged(object sender, TextChangedEventArgs e)
        {
            LogOutput.ScrollToEnd();
            LogOutput.UpdateLayout();
        }

        #endregion LogInformation

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
                output.AddRange(rGroup.Extraction.Result.Select(res => new Scraper.Models.Scraper()
                {
                    Url = res + "analys/",
                    Scope = new Scope() {From = "<table class=\"reports-list\">", To = "</table"},
                    Groups = new List<GroupItem>()
                    {
                        new()
                        {
                            From = "<tr class=\"report-row\"", To = "</tr>", GroupItemType = GroupItemType.Loop,
                            Extraction = new GroupItem()
                                {From = "<td>", To = "</td>", GroupItemType = GroupItemType.Loop}
                        }
                    }
                }).Select(exRule => new Scraper.Scraper(exRule)).Select(x => x.GetData()));
                Console.WriteLine();
            }
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
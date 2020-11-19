using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Vertex.Scraper.Demo
{
    class Program
    {
        static void Main(string[] args)
        {

            using (IWebDriver driver = new ChromeDriver())
            {
                Console.Clear();
                Console.WriteLine("https://vrtx.wd5.myworkdayjobs.com/vertex_careers");
                driver.Navigate().GoToUrl("https://vrtx.wd5.myworkdayjobs.com/vertex_careers");
                bool loaded = false;
                while (!loaded)
                {
                    try
                    {
                        driver.FindElement(By.Id("wd-FieldSet-NO_METADATA_ID-uid1-label"));
                        loaded = true;
                    }
                    catch (Exception ex)
                    {

                    }
                }

                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                driver.Manage().Window.Maximize();

                for (int i = 1; i <= 7; i++)
                {
                    js.ExecuteScript("window.scrollBy(0," + 1000 * i + ")");
                    Thread.Sleep(3000);
                }


                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(driver.PageSource);

                var listing = doc.DocumentNode.SelectSingleNode("//*[@id=\"wd-FacetedSearchResultList-facetSearchResultList.newFacetSearch.Report_Entry\"]/div[2]/ul");
                if (listing != null)
                {
                    var links = listing.ChildNodes.Where(x => x.Name == "li");
                    Console.WriteLine("Total jobs: "+links.Count());
                    foreach (var li in links)
                    {
                        var sub = new HtmlDocument();
                        sub.LoadHtml(li.InnerHtml);

                        var titleNode = sub.DocumentNode.SelectSingleNode("/div[1]/div[1]/div[1]/ul[1]/li[1]/div[1]/div[1]/div[1]/div[1]");
                        if (titleNode != null)
                            Console.WriteLine(titleNode.InnerText);

                        var postLink = sub.DocumentNode.SelectSingleNode("/div[1]/div[1]/div[1]/ul[1]/li[1]/div[1]");
                        if (postLink != null)
                        {
                            var eleId = postLink.Attributes.FirstOrDefault(x => x.Name == "id").Value;
                            new Actions(driver)
    .KeyDown(Keys.Control)
    .Click(driver.FindElement(By.Id(eleId)))
    .KeyUp(Keys.Control)
    .Perform();
                            //  js.ExecuteScript("document.getElementById(\'" + eleId + "\').dispatchEvent(new MouseEvent(\"click\", {ctrlKey: true}));");

                            driver.SwitchTo().Window(driver.WindowHandles.Last());

                            //Wait for page to load
                            loaded = false;
                            while (!loaded)
                            {
                                try
                                {
                                    driver.FindElement(By.Id("richTextArea.jobPosting.title-input"));
                                    loaded = true;
                                }
                                catch (Exception)
                                {

                                }

                            }

                            sub.LoadHtml(driver.PageSource);
                            try
                            {
                                var descNode = sub.DocumentNode.SelectSingleNode("//div[@id='richTextArea.jobPosting.jobDescription-input--uid8-input']").InnerText;
                                Console.WriteLine(HttpUtility.UrlDecode(descNode));
                            }
                            catch { }

                            driver.Close();
                            driver.SwitchTo().Window(driver.WindowHandles.First());
                        }


                    }
                }

            }
        }
    }
}

using CefSharp;
using CefSharp.OffScreen;
using CefSharp.WinForms;
using Serilog;
using SharedDomain.Enums;
using SharedDomain.ObjectValues;
using SharedDomainService.Interfaces;
using System.Windows.Forms;

namespace BrowserForm
{
    public partial class SmartCrawlerForm : Form
    {
        private readonly ICrawlServerHandler _serverHandler;
        private readonly ICrawlClientHandler _clientHandler;
        private bool isloaded = false;
        Serilog.Core.Logger _logger;
        public SmartCrawlerForm(ICrawlServerHandler serverHandler, ICrawlClientHandler clientHandler)
        {
            InitializeComponent();
            _serverHandler = serverHandler;
            _clientHandler = clientHandler;
            ContextMenuStrip cm = new ContextMenuStrip();
            cm.Items.Add("Load DigiKey", null, new EventHandler(LoadDigiKey));
            this.ContextMenuStrip = cm;

            _logger = new LoggerConfiguration()
    .WriteTo.File("logApp.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();


        }

        private async void LoadDigiKey(object? sender, EventArgs e)
        {
            await browser.LoadUrlAsync("https://digikey.com/").ConfigureAwait(true);
        }

        private async void Form1_Load(object sender, EventArgs e)
        {

            Task.Run(async () =>
            {
                 
                _serverHandler.StartService(async (a, b) =>
                {
                    _logger.Information($"CrawlRequestCommandType:{a.CrawlRequestCommandType}  Script:{a.Script}   Interval:{a.Interval}");
                    if (a == null)
                    {
                        b.SendMessage(new SharedDomain.ObjectValues.CrawlResponseStatus() { CrawlResponseStatusType = SharedDomain.Enums.CrawlResponseStatusType.Exception, Message = "Json Is Not Valid" });
                        return;
                    }
                    try
                    {
                        switch (a.CrawlRequestCommandType)
                        {
                            case SharedDomain.Enums.CrawlRequestCommandType.LoadUrlAsync:
                                await browser.LoadUrlAsync(a.Script).ConfigureAwait(true);
                                b.SendMessage(new SharedDomain.ObjectValues.CrawlResponseStatus() { CrawlResponseStatusType = SharedDomain.Enums.CrawlResponseStatusType.Ok, Address = browser.Address });
                                break;
                            case SharedDomain.Enums.CrawlRequestCommandType.WaitForSelector:
                                await browser.WaitForSelectorAsync(".domain-suggest", TimeSpan.FromMilliseconds(1000 * a.Interval)).ConfigureAwait(true);
                                await Task.Delay(1_000).ConfigureAwait(true);
                                b.SendMessage(new SharedDomain.ObjectValues.CrawlResponseStatus() { CrawlResponseStatusType = SharedDomain.Enums.CrawlResponseStatusType.Ok, Address = browser.Address });
                                break;
                            case SharedDomain.Enums.CrawlRequestCommandType.ExecuteScript:
                                await Task.Delay(500);
                                browser.ExecuteScriptAsync(a.Script);
                                b.SendMessage(new SharedDomain.ObjectValues.CrawlResponseStatus() { CrawlResponseStatusType = SharedDomain.Enums.CrawlResponseStatusType.Ok, Address = browser.Address });
                                break;
                            case SharedDomain.Enums.CrawlRequestCommandType.WaitForNavigation:
                                await browser.WaitForNavigationAsync(TimeSpan.FromMilliseconds(1000 * a.Interval));
                                b.SendMessage(new SharedDomain.ObjectValues.CrawlResponseStatus() { CrawlResponseStatusType = SharedDomain.Enums.CrawlResponseStatusType.Ok, Address = browser.Address });
                                break;


                            case SharedDomain.Enums.CrawlRequestCommandType.ScrollToBotttom:


                                // Set the number of steps and the delay between each step
                                int numSteps = 18;
                                int delayBetweenSteps = 2200;

                                for (int i = 0; i < numSteps; i++)
                                {
                                    // Calculate the scroll position based on the step
                                    double scrollPercentage = (double)i / (numSteps - 1);
                                    var hight = await GetDocumentScrollHeight();
                                    int scrollTo = (int)(scrollPercentage * hight);

                                    // Execute JavaScript to scroll to the calculated position
                                    browser.ExecuteScriptAsync($"window.scrollTo(0, {scrollTo});");

                                    // Pause for a short duration to simulate a slow scroll
                                    await Task.Delay(delayBetweenSteps);
                                }

                                b.SendMessage(new SharedDomain.ObjectValues.CrawlResponseStatus() { CrawlResponseStatusType = SharedDomain.Enums.CrawlResponseStatusType.Ok, Address = browser.Address });

                                break;



                            case SharedDomain.Enums.CrawlRequestCommandType.GetContent:
                                try
                                {

                                    if (browser.GetMainFrame() != null && !string.IsNullOrEmpty(browser.Address))
                                    {
                                        _logger.Information($"Start Create Task Is Browser Loaded: {isloaded}");

                                        int timeout = 1000 * 60 * 5;
                                        string content = string.Empty;
                                        var task =await Task.Factory.StartNew(async () =>
                                        {
                                            int tryCount = 0;

                                            //while (!isloaded)
                                            while (string.IsNullOrEmpty(content))
                                            {
                                                _logger.Information($"Start Create Task Is Browser HasContent: {!string.IsNullOrEmpty(content)}  TryCount: {tryCount}");
                                                tryCount++;
                                                if (tryCount > 3)
                                                {
                                                    _logger.Information("No Contetnt");

                                                    content = "No Content";
                                                    break;
                                                }

                                                var response = await browser.EvaluateScriptAsync("document.documentElement.outerHTML", TimeSpan.FromSeconds(30));

                                                if (response.Success && response.Result != null)
                                                {
                                                    content = response.Result.ToString();

                                                    _logger.Information($"Start Create Task Is Browser response.Success: {response.Success}  response.Result != null {response.Result != null}");

                                                    if (!string.IsNullOrEmpty(content))
                                                        break;
                                                }
                                                await Task.Delay(1000 * 5);
                                                _logger.Information("Start Reload"); browser.Reload();
                                                await Task.Delay(1000 * 60 * 1);
                                                _logger.Information("End Reload");
                                            }
                                        }, TaskCreationOptions.LongRunning);
                                        if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
                                        {
                                            _logger.Information($"Success Task With Content: {!string.IsNullOrEmpty(content)}");
                                            // task completed within timeout
                                        }
                                        else
                                        {
                                            content = "No Content";
                                            _logger.Information($"TimeOut Task With Content: {!string.IsNullOrEmpty(content)}");
                                        }


                                        if (!string.IsNullOrEmpty(content) && content != "No Content" && !string.IsNullOrEmpty(a.Script))
                                        {
                                            _logger.Information($"Start Get Element");
                                            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                                            doc.LoadHtml(content);
                                            Task.Delay(1000).GetAwaiter().GetResult();
                                            var htmlNode = doc.GetElementbyId(a.Script);
                                            if (htmlNode != null)
                                                content = htmlNode.OuterHtml;
                                            else
                                                content = "Table Not Found";

                                            _logger.Information($"Element Fetch");
                                        }
                                        _logger.Information($"Start  Send Message 0");
                                        b.SendMessage(new SharedDomain.ObjectValues.CrawlResponseStatus() { CrawlResponseStatusType = SharedDomain.Enums.CrawlResponseStatusType.Ok, Content = content, Address = browser.Address });
                                        _logger.Information($"Message Sended 0");
                                        // browser.GetMainFrame().GetSource(new ContentVisitor(b, a.Script, browser));

                                    }
                                    else
                                    {
                                        _logger.Information($"Start  Send Message 1");
                                        b.SendMessage(new SharedDomain.ObjectValues.CrawlResponseStatus() { CrawlResponseStatusType = SharedDomain.Enums.CrawlResponseStatusType.Ok, Content = string.Empty, Address = string.Empty });
                                        _logger.Information($"Message Sended 1");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.Information($"Start  Send Message 2 With Exception {ex.Message}");
                                    b.SendMessage(new SharedDomain.ObjectValues.CrawlResponseStatus() { CrawlResponseStatusType = SharedDomain.Enums.CrawlResponseStatusType.Exception, Content = string.Empty, Address = string.Empty, Message = ex.Message });
                                    _logger.Information($"Message Sended 2");
                                }
                                break;

                            case SharedDomain.Enums.CrawlRequestCommandType.GetAddress:
                                b.SendMessage(new SharedDomain.ObjectValues.CrawlResponseStatus() { CrawlResponseStatusType = SharedDomain.Enums.CrawlResponseStatusType.Ok, Address = browser.Address });
                                break;


                            case SharedDomain.Enums.CrawlRequestCommandType.EvaluateScriptAsync:
                                var result = await browser.EvaluateScriptAsync(a.Script);
                                b.SendMessage(new SharedDomain.ObjectValues.CrawlResponseStatus() { CrawlResponseStatusType = SharedDomain.Enums.CrawlResponseStatusType.Ok, EvaluateScriptStatus = result.Success, EvaluateScriptResult = result.Result });
                                break;



                            default:
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        b.SendMessage(new SharedDomain.ObjectValues.CrawlResponseStatus() { CrawlResponseStatusType = SharedDomain.Enums.CrawlResponseStatusType.Exception, Content = String.Empty, Message = ex.Message });
                    }
                });

            });

             

            //        Task.Run(async () =>
            //        {
            //            await _clientHandler.SendCommand("127.0.0.1", 8070, new CrawlRequestCommand() { CrawlRequestCommandType = CrawlRequestCommandType.LoadUrlAsync, Script = "https://digikey.com/" }, null);
            //            await _clientHandler.SendCommand("127.0.0.1", 8070, new CrawlRequestCommand() { CrawlRequestCommandType = CrawlRequestCommandType.WaitForSelector, Script = ".domain-suggest", Interval = 60 }, null);

            //            await _clientHandler.SendCommand("127.0.0.1", 8070, new CrawlRequestCommand() { CrawlRequestCommandType = CrawlRequestCommandType.ExecuteScript, Script = @"document.getElementsByClassName('domain-suggest')[0].querySelector('.dk-modal__close').click();" }, null);

            //            await _clientHandler.SendCommand("127.0.0.1", 8070, new CrawlRequestCommand() { CrawlRequestCommandType = CrawlRequestCommandType.ExecuteScript, Script = @"document.querySelector("".cookie-notice .secondary.button"").click();" }, null);

            //            await _clientHandler.SendCommand("127.0.0.1", 8070, new CrawlRequestCommand() { CrawlRequestCommandType = CrawlRequestCommandType.ExecuteScript, Script = @"document.querySelector('#header__storage > div > div.flymenu__nav-bar > div > ul > li:nth-child(1) > a').click();" }, null);

            //            await _clientHandler.SendCommand("127.0.0.1", 8070, new CrawlRequestCommand() { CrawlRequestCommandType = CrawlRequestCommandType.ExecuteScript, Script = @"document.querySelector('a[href$=""590""]').click();" }, null);
            //            await _clientHandler.SendCommand("127.0.0.1", 8070, new CrawlRequestCommand() { CrawlRequestCommandType = CrawlRequestCommandType.WaitForNavigation, Interval = 60 }, null);
            //            await _clientHandler.SendCommand("127.0.0.1", 8070, new CrawlRequestCommand() { CrawlRequestCommandType = CrawlRequestCommandType.GetContent }, (r) =>
            //            {
            //                var jj = r.Content;
            //            });


            //            int i = 0;
            //            for (int page = 2; page < 927; page++)
            //            {
            //                i++;
            //                if (i >= 20)
            //                {
            //                    i = 0;
            //                    await Task.Delay(1000 * 60 * 5);
            //                }
            //                await _clientHandler.SendCommand("127.0.0.1", 8070, new CrawlRequestCommand() { CrawlRequestCommandType = CrawlRequestCommandType.ExecuteScript, Script = $@"document.querySelector('[data-testid=""btn-page-{page}""]').click();" }, null);
            //                await Task.Delay(2000);
            //            }
            //        });
        }

        private void RefreshJob()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var url = browser.Address;
                    if (string.IsNullOrEmpty(url))
                    {
                        Task.Delay(1000).GetAwaiter().GetResult();
                        continue;
                    }
                    Task.Delay(1000 * 60 * 5).GetAwaiter().GetResult();

                    if (browser.Address == url)
                        browser.Invoke(() => { browser.Refresh(); });
                }

            });
        }
        private void GetContent()
        {

        }
        private async Task<int> GetDocumentScrollHeight()
        {
            // Execute JavaScript to get the actual scroll height of the document
            var result = await browser.EvaluateScriptAsync("Math.max( document.body.scrollHeight, document.documentElement.scrollHeight, document.body.offsetHeight, document.documentElement.offsetHeight, document.body.clientHeight, document.documentElement.clientHeight );");

            // Parse the result as an integer
            if (result.Success && result.Result != null && int.TryParse(result.Result.ToString(), out int scrollHeight))
            {
                return scrollHeight;
            }

            return 0;
        }

        private void browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            isloaded = !e.IsLoading;
        }
    }
}

public class ContentVisitor : IStringVisitor
{
    private readonly IMessageHandler _messageHandler;
    private readonly CefSharp.WinForms.ChromiumWebBrowser _chromiumWebBrowser;
    private readonly string _script;
    // private bool hasContent = false;
    public ContentVisitor(IMessageHandler messageHandler, string script,
CefSharp.WinForms.ChromiumWebBrowser chromiumWebBrowser)
    {
        _messageHandler = messageHandler;
        _chromiumWebBrowser = chromiumWebBrowser;
        _script = script;
        //Task.Run(async () =>
        //{
        //    while (!hasContent)
        //    {
        //      await  Task.Delay(1000);


        //    }
        //});
    }
    public void Dispose()
    {

    }

    public void Visit(string content)
    {
        //  hasContent = true;
        if (!string.IsNullOrEmpty(content) && !string.IsNullOrEmpty(_script))
        {

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(content);
            Task.Delay(1000).GetAwaiter().GetResult();
            var htmlNode = doc.GetElementbyId(_script);
            if (htmlNode != null)
                content = htmlNode.OuterHtml;

        }
        _messageHandler.SendMessage(new SharedDomain.ObjectValues.CrawlResponseStatus() { CrawlResponseStatusType = SharedDomain.Enums.CrawlResponseStatusType.Ok, Content = content, Address = _chromiumWebBrowser.Address });
    }
}
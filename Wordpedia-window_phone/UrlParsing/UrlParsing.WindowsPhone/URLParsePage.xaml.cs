using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿에 대한 설명은 http://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace UrlParsing
{
    /// <summary>
    /// 자체에서 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class URLParsePage : Page
    {
        private String targetURL;
        private HtmlDocument htmlData = null;

        public URLParsePage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// 이 페이지가 프레임에 표시되려고 할 때 호출됩니다.
        /// </summary>
        /// <param name="e">페이지에 도달한 방법을 설명하는 이벤트 데이터입니다.
        /// 이 매개 변수는 일반적으로 페이지를 구성하는 데 사용됩니다.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: 여기에 표시할 페이지를 준비합니다.

            // TODO: 응용 프로그램에 여러 페이지가 포함된 경우
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed 이벤트에 등록하여
            // 하드웨어 뒤로 단추를 처리하는지 확인하십시오.
            // 일부 템플릿에서 제공하는 NavigationHelper를 사용할 경우
            // 이 이벤트가 자동으로 처리됩니다

            btn_Next.Click += btnNextClick;
            btn_Reset.Click += btnResetClick;

            try
            {
                targetURL = e.Parameter as String;
                // 여기서 HTML코드를 가져올수 없다. DOM이 Load되지 않은 상태이기때문에.
                refreshWebView();
            }
            catch (System.FormatException ex) // for UriFormatException
            {
                Debug.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// 프레임의 페이지가 더 이상 활성 페이지가 아닐 때 호출됩니다.
        /// </summary>
        /// <param name="e">이벤트 데이터를 포함하는 개체입니다.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Unregister the current page as a share source.
        }

        private async void btnNextClick(object sender, RoutedEventArgs e)
        {
            List<String> htmlList = new List<String>();
            htmlData.LoadHtml(await web_ContentView.InvokeScriptAsync("eval", new string[] { "document.documentElement.outerHTML;" }));

            foreach (HtmlNode node in htmlData.DocumentNode.Descendants("clicked"))
            {
                htmlList.Add(node.ParentNode.InnerHtml);
            }

            Frame.Navigate(typeof(TestPage), htmlList);
        }

        private void btnResetClick(object sender, RoutedEventArgs e)
        {
            refreshWebView();
        }

        private async void webViewContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            htmlData = new HtmlDocument();
            htmlData.LoadHtml(await sender.InvokeScriptAsync("eval", new string[] { "document.documentElement.outerHTML;" }));

            foreach (HtmlNode node in htmlData.DocumentNode.Descendants("a"))
            {
                node.Attributes.Remove("href");
            }

            web_ContentView.NavigateToString(htmlData.DocumentNode.OuterHtml);
            web_ContentView.DOMContentLoaded -= webViewContentLoaded;
            web_ContentView.DOMContentLoaded += webViewLocalContentLoaded;
        }

        private async void webViewLocalContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            await web_ContentView.InvokeScriptAsync("eval", new String[] {
                "var elements=document.getElementsByTagName('div');" +
                "for (var i=0; i<elements.length;i++) {" +
                    "var div = elements[i];" +
                    "div.clicked = 'false';" +
                    "if (div.addEventListener) {" +
                        "div.addEventListener('click', clickFunction, false);" +
                    "} else {" +
                        "div.attachEvent('onclick', clickFunction);" +
                    "}" +
                "}" +
                "function clickFunction(e){" +
                    "var clicked = document.createElement('clicked');" +
                    "e.target.appendChild(clicked);" +
                    "e.target.clicked='true';" +
                    "e.target.style.backgroundColor='#444444';" +
                    "window.external.notify('Hello');" +
                 "}"
            });
        }

        private void webViewScriptNotify(object sender, NotifyEventArgs args)
        {
            String str = args.Value;
        }

        private void refreshWebView()
        {
            web_ContentView.Navigate(new Uri(targetURL, UriKind.Absolute));
            web_ContentView.NavigationCompleted += webViewNavigationCompleted;
            web_ContentView.DOMContentLoaded += webViewContentLoaded;
            web_ContentView.ScriptNotify += webViewScriptNotify;
        }

        private void webViewNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            
        }
    }
}

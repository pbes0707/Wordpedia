using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿에 대한 설명은 http://go.microsoft.com/fwlink/?LinkID=390556에 나와 있습니다.

namespace UrlParsing
{
    /// <summary>
    /// 자체에서 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class TestPage : Page
    {
        public TestPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 이 페이지가 프레임에 표시되려고 할 때 호출됩니다.
        /// </summary>
        /// <param name="e">페이지에 도달한 방법을 설명하는 이벤트 데이터입니다.
        /// 이 매개 변수는 일반적으로 페이지를 구성하는 데 사용됩니다.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            List<String> htmlList = e.Parameter as List<String>;

            tb_HTML.Text = "";
            foreach (String htmlContent in htmlList)
            {
                /*tb_HTML.Text += htmlContent;
                tb_HTML.Text += "\n";*/
                Regex regex = new Regex("<[^>]*>", RegexOptions.IgnoreCase);
                String result = regex.Replace(htmlContent, "");
                result = result.Replace("\n", "");
                if (!result.Trim().Equals("") && !tb_HTML.Text.Contains(result))
                {
                    tb_HTML.Text += result;
                    tb_HTML.Text += "\n";
                }
            }
        }
    }
}

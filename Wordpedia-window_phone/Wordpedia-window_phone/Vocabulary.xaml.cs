using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SQLite;

namespace Wordpedia_window_phone
{
    public sealed partial class Vocabulary : Page
    {
        public Vocabulary()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            kind t = (kind)e.Parameter;

            switch(t.Spec)
            {
                case 0: // 이미 제작된 단어장을 불러올 때
                    {
                        break;
                    }
                case 1: // 새로운 단어장을 Create 할 때
                    {
                        ////////////////////////////String Split/////////////////////////////
                        string lowerString = t.Text.ToLower();
                        string[] separators = { ",", ".", "!", "?", ";", ":", " ", "\r", "\n", "\t" };
                        string[] words = lowerString.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                        ///////////////////////////List Create////////////////////////////////
                        List<wordData> wordList = new List<wordData>();
                        foreach (string v in words)
                        {
                            bool _flag = false;
                            if (wordList.Count != 0)
                            {
                                foreach (wordData c in wordList)
                                {
                                    if (c.Word == v)
                                    {
                                        c.Count++;
                                        _flag = true;
                                        break;
                                    }
                                }
                            }
                            if (_flag == false || wordList.Count == 0)
                                wordList.Add(new wordData(v));
                        }
                        //////////////////////Sqlite Table Create///////////////////////
                        SQLiteCommand
                        break;
                    }
            }
        }
    }

    class kind
    {
        public int Spec { get; set; }
        public String Text { get; set; }
    }
}

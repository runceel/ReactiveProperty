using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WP8.Resources;

namespace WP8
{
    public partial class MainPage : PhoneApplicationPage
    {
        // コンストラクター
        public MainPage()
        {
            InitializeComponent();

            // ApplicationBar をローカライズするためのサンプル コード
            //BuildLocalizedApplicationBar();
        }

        // ローカライズされた ApplicationBar を作成するためのサンプル コード
        //private void BuildLocalizedApplicationBar()
        //{
        //    // ページの ApplicationBar を ApplicationBar の新しいインスタンスに設定します。
        //    ApplicationBar = new ApplicationBar();

        //    // 新しいボタンを作成し、テキスト値を AppResources のローカライズされた文字列に設定します。
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // AppResources のローカライズされた文字列で、新しいメニュー項目を作成します。
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}
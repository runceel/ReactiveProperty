using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace StoreApp.Common
{
    /// <summary>
    /// NavigationManager は、ページ間のナビゲーションで使用されます。NavigationManager のコマンドを使用して、
    /// ナビゲーション操作を実行できるだけでなく、ナビゲーション操作を実行するための標準マウス ショートカット
    /// およびキーボード ショートカットを登録できます。さらに、SuspensionManger も統合されているため、
    /// ページ間で移動するときのプロセス継続時間管理および状態管理も
    /// 処理できます。
    /// </summary>
    /// <example>
    /// NavigationManager を利用するには、この 2 つの手順に従うか、
    /// BasicPage などの BlankPage 以外のページ アイテム テンプレートを使用します。
    /// 
    /// 1) ページのコンストラクター内などの場所に NaivgationHelper インスタンスを作成し、
    ///   LoadState イベントと SaveState イベントに対するコールバックを
    ///     登録します。
    /// <code>
    ///     public MyPage()
    ///     {
    ///         this.InitializeComponent();
    ///         var navigationHelper = new NavigationHelper(this);
    ///         this.navigationHelper.LoadState += navigationHelper_LoadState;
    ///         this.navigationHelper.SaveState += navigationHelper_SaveState;
    ///     }
    ///     
    ///     private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
    ///     { }
    ///     private async void navigationHelper_SaveState(object sender, LoadStateEventArgs e)
    ///     { }
    /// </code>
    /// 
    /// 2) ページがナビゲーションに追加されるたびに、そのページを登録して NavigationManager を呼び出すには、
    ///     <see cref="Windows.UI.Xaml.Controls.Page.OnNavigatedTo"/>
    ///     イベントと <see cref="Windows.UI.Xaml.Controls.Page.OnNavigatedFrom"/> イベントをオーバーライドします。
    /// <code>
    ///     protected override void OnNavigatedTo(NavigationEventArgs e)
    ///     {
    ///         navigationHelper.OnNavigatedTo(e);
    ///     }
    ///     
    ///     protected override void OnNavigatedFrom(NavigationEventArgs e)
    ///     {
    ///         navigationHelper.OnNavigatedFrom(e);
    ///     }
    /// </code>
    /// </example>
    [Windows.Foundation.Metadata.WebHostHidden]
    public class NavigationHelper : DependencyObject
    {
        private Page Page { get; set; }
        private Frame Frame { get { return this.Page.Frame; } }

        /// <summary>
        /// <see cref="NavigationHelper"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="page">ナビゲーションに使用される現在のページへの参照。
        /// この参照ではフレームの操作が許可され、ページでウィンドウ全体を使用している場合にのみ
        /// キーボードのナビゲーション要求が発生するようにすることができます。</param>
        public NavigationHelper(Page page)
        {
            this.Page = page;

            // このページがビジュアル ツリーの一部である場合、次の 2 つの変更を行います:
            // 1) アプリケーションのビューステートをページの表示状態にマップする
            // 2) キーボードおよびマウスのナビゲーション要求を処理する
            this.Page.Loaded += (sender, e) =>
            {
                // キーボードおよびマウスのナビゲーションは、ウィンドウ全体を使用する場合のみ適用されます
                if (this.Page.ActualHeight == Window.Current.Bounds.Height &&
                    this.Page.ActualWidth == Window.Current.Bounds.Width)
                {
                    // ウィンドウで直接待機するため、フォーカスは不要です
                    Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated +=
                        CoreDispatcher_AcceleratorKeyActivated;
                    Window.Current.CoreWindow.PointerPressed +=
                        this.CoreWindow_PointerPressed;
                }
            };

            // ページが表示されない場合、同じ変更を元に戻します
            this.Page.Unloaded += (sender, e) =>
            {
                Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -=
                    CoreDispatcher_AcceleratorKeyActivated;
                Window.Current.CoreWindow.PointerPressed -=
                    this.CoreWindow_PointerPressed;
            };
        }

        #region ナビゲーション サポート

        RelayCommand _goBackCommand;
        RelayCommand _goForwardCommand;

        /// <summary>
        /// Frame のナビゲーション履歴が Frame 自体で管理される場合、
        /// 戻るナビゲーション履歴の最新の項目に移動するために
        /// 戻るボタンの Command プロパティにバインドするために使用される <see cref="RelayCommand"/>。
        /// 
        /// <see cref="RelayCommand"/> は、仮想メソッド <see cref="GoBack"/>
        /// を CanExecute の実行アクションおよび <see cref="CanGoBack"/> として使用するために設定されます。
        /// </summary>
        public RelayCommand GoBackCommand
        {
            get
            {
                if (_goBackCommand == null)
                {
                    _goBackCommand = new RelayCommand(
                        () => this.GoBack(),
                        () => this.CanGoBack());
                }
                return _goBackCommand;
            }
            set
            {
                _goBackCommand = value;
            }
        }
        /// <summary>
        /// Frame のナビゲーション履歴が Frame 自体で管理される場合、
        /// 次に進むナビゲーション履歴の最新の項目に移動するために使用される <see cref="RelayCommand"/>。
        /// 
        /// <see cref="RelayCommand"/> は、仮想メソッド <see cref="GoForward"/>
        /// を CanExecute の実行アクションおよび <see cref="CanGoForward"/> として使用するために設定されます。
        /// </summary>
        public RelayCommand GoForwardCommand
        {
            get
            {
                if (_goForwardCommand == null)
                {
                    _goForwardCommand = new RelayCommand(
                        () => this.GoForward(),
                        () => this.CanGoForward());
                }
                return _goForwardCommand;
            }
        }

        /// <summary>
        /// <see cref="GoBackCommand"/> プロパティで使用される仮想メソッド。
        /// <see cref="Frame"/> を戻すことができるかどうかを判断するために使用されます。
        /// </summary>
        /// <returns>
        /// <see cref="Frame"/> のナビゲーション履歴に 1 個以上のエントリがある場合 
        /// ("戻る" 操作)、true です。
        /// </returns>
        public virtual bool CanGoBack()
        {
            return this.Frame != null && this.Frame.CanGoBack;
        }
        /// <summary>
        /// <see cref="GoForwardCommand"/> プロパティで使用される仮想メソッド。
        /// <see cref="Frame"/> を進める操作を実行できるかどうかを判断するために使用されます。
        /// </summary>
        /// <returns>
        /// <see cref="Frame"/> のナビゲーション履歴に 1 個以上のエントリがある場合 
        /// ("進む" 操作)、true です。
        /// </returns>
        public virtual bool CanGoForward()
        {
            return this.Frame != null && this.Frame.CanGoForward;
        }

        /// <summary>
        /// <see cref="GoBackCommand"/> プロパティで使用される仮想メソッド。
        /// <see cref="Windows.UI.Xaml.Controls.Frame.GoBack"/> メソッドを呼び出すために使用されます。
        /// </summary>
        public virtual void GoBack()
        {
            if (this.Frame != null && this.Frame.CanGoBack) this.Frame.GoBack();
        }
        /// <summary>
        /// <see cref="GoForwardCommand"/> プロパティで使用される仮想メソッド。
        /// <see cref="Windows.UI.Xaml.Controls.Frame.GoForward"/> メソッドを呼び出すために使用されます。
        /// </summary>
        public virtual void GoForward()
        {
            if (this.Frame != null && this.Frame.CanGoForward) this.Frame.GoForward();
        }

        /// <summary>
        /// このページがアクティブで、ウィンドウ全体を使用する場合、Alt キーの組み合わせなどのシステム キーを含む、
        /// キーボード操作で呼び出されます。ページがフォーカスされていないときでも、
        /// ページ間のキーボード ナビゲーションの検出に使用されます。
        /// </summary>
        /// <param name="sender">イベントをトリガーしたインスタンス。</param>
        /// <param name="e">イベントが発生する条件を説明するイベント データ。</param>
        private void CoreDispatcher_AcceleratorKeyActivated(CoreDispatcher sender,
            AcceleratorKeyEventArgs e)
        {
            var virtualKey = e.VirtualKey;

            // 左方向キーや右方向キー、または専用に設定した前に戻るキーや次に進むキーを押した場合のみ、
            // 詳細を調査します
            if ((e.EventType == CoreAcceleratorKeyEventType.SystemKeyDown ||
                e.EventType == CoreAcceleratorKeyEventType.KeyDown) &&
                (virtualKey == VirtualKey.Left || virtualKey == VirtualKey.Right ||
                (int)virtualKey == 166 || (int)virtualKey == 167))
            {
                var coreWindow = Window.Current.CoreWindow;
                var downState = CoreVirtualKeyStates.Down;
                bool menuKey = (coreWindow.GetKeyState(VirtualKey.Menu) & downState) == downState;
                bool controlKey = (coreWindow.GetKeyState(VirtualKey.Control) & downState) == downState;
                bool shiftKey = (coreWindow.GetKeyState(VirtualKey.Shift) & downState) == downState;
                bool noModifiers = !menuKey && !controlKey && !shiftKey;
                bool onlyAlt = menuKey && !controlKey && !shiftKey;

                if (((int)virtualKey == 166 && noModifiers) ||
                    (virtualKey == VirtualKey.Left && onlyAlt))
                {
                    // 前に戻るキーまたは Alt キーを押しながら左方向キーを押すと前に戻ります
                    e.Handled = true;
                    this.GoBackCommand.Execute(null);
                }
                else if (((int)virtualKey == 167 && noModifiers) ||
                    (virtualKey == VirtualKey.Right && onlyAlt))
                {
                    // 次に進むキーまたは Alt キーを押しながら右方向キーを押すと次に進みます
                    e.Handled = true;
                    this.GoForwardCommand.Execute(null);
                }
            }
        }

        /// <summary>
        /// このページがアクティブで、ウィンドウ全体を使用する場合、マウスのクリック、タッチ スクリーンのタップなどの
        /// 操作で呼び出されます。ページ間を移動するため、マウス ボタンのクリックによるブラウザー スタイルの
        /// 次に進むおよび前に戻る操作の検出に使用されます。
        /// </summary>
        /// <param name="sender">イベントをトリガーしたインスタンス。</param>
        /// <param name="e">イベントが発生する条件を説明するイベント データ。</param>
        private void CoreWindow_PointerPressed(CoreWindow sender,
            PointerEventArgs e)
        {
            var properties = e.CurrentPoint.Properties;

            // 左、右、および中央ボタンを使用したボタン操作を無視します
            if (properties.IsLeftButtonPressed || properties.IsRightButtonPressed ||
                properties.IsMiddleButtonPressed) return;

            // [戻る] または [進む] を押すと適切に移動します (両方同時には押しません)
            bool backPressed = properties.IsXButton1Pressed;
            bool forwardPressed = properties.IsXButton2Pressed;
            if (backPressed ^ forwardPressed)
            {
                e.Handled = true;
                if (backPressed) this.GoBackCommand.Execute(null);
                if (forwardPressed) this.GoForwardCommand.Execute(null);
            }
        }

        #endregion

        #region プロセス継続時間管理

        private String _pageKey;

        /// <summary>
        /// ナビゲーション中に渡されるコンテンツをページに入力するために、
        /// 現在のページにこのイベントを登録します。前のセッションからページを
        /// 再作成する場合は、保存状態も指定されます。
        /// </summary>
        public event LoadStateEventHandler LoadState;
        /// <summary>
        /// アプリケーションが中断されるか、ナビゲーション キャッシュから
        /// ページが破棄された場合に、現在のページに関連付けられている
        /// 状態を保持するために、現在のページにこのイベントを
        /// 登録します。
        /// </summary>
        public event SaveStateEventHandler SaveState;

        /// <summary>
        /// このページがフレームに表示されるときに呼び出されます。
        /// このメソッドは <see cref="LoadState"/> を呼び出します。ここに、すべてのページ別ナビゲーションと
        /// プロセス継続時間管理ロジックを配置してください。
        /// </summary>
        /// <param name="e">このページにどのように到達したかを説明するイベント データ。Parameter 
        /// プロパティは、表示するグループを示します。</param>
        public void OnNavigatedTo(NavigationEventArgs e)
        {
            var frameState = SuspensionManager.SessionStateForFrame(this.Frame);
            this._pageKey = "Page-" + this.Frame.BackStackDepth;

            if (e.NavigationMode == NavigationMode.New)
            {
                // 新しいページをナビゲーション スタックに追加するとき、次に進むナビゲーションの
                // 既存の状態をクリアします
                var nextPageKey = this._pageKey;
                int nextPageIndex = this.Frame.BackStackDepth;
                while (frameState.Remove(nextPageKey))
                {
                    nextPageIndex++;
                    nextPageKey = "Page-" + nextPageIndex;
                }

                // ナビゲーション パラメーターを新しいページに渡します
                if (this.LoadState != null)
                {
                    this.LoadState(this, new LoadStateEventArgs(e.Parameter, null));
                }
            }
            else
            {
                // ナビゲーション パラメーターおよび保存されたページの状態をページに渡します。
                // このとき、中断状態の読み込みや、キャッシュから破棄されたページの再作成と同じ対策を
                // 使用します
                if (this.LoadState != null)
                {
                    this.LoadState(this, new LoadStateEventArgs(e.Parameter, (Dictionary<String, Object>)frameState[this._pageKey]));
                }
            }
        }

        /// <summary>
        /// このページがフレームに表示されなくなるときに呼び出されます。
        /// このメソッドは <see cref="SaveState"/> を呼び出します。ここに、すべてのページ別ナビゲーションと
        /// プロセス継続時間管理ロジックを配置してください。
        /// </summary>
        /// <param name="e">このページにどのように到達したかを説明するイベント データ。Parameter 
        /// プロパティは、表示するグループを示します。</param>
        public void OnNavigatedFrom(NavigationEventArgs e)
        {
            var frameState = SuspensionManager.SessionStateForFrame(this.Frame);
            var pageState = new Dictionary<String, Object>();
            if (this.SaveState != null)
            {
                this.SaveState(this, new SaveStateEventArgs(pageState));
            }
            frameState[_pageKey] = pageState;
        }

        #endregion
    }

    /// <summary>
    /// <see cref="NavigationHelper.LoadState"/> イベントを処理するメソッドを表します
    /// </summary>
    public delegate void LoadStateEventHandler(object sender, LoadStateEventArgs e);
    /// <summary>
    /// <see cref="NavigationHelper.SaveState"/> イベントを処理するメソッドを表します
    /// </summary>
    public delegate void SaveStateEventHandler(object sender, SaveStateEventArgs e);

    /// <summary>
    /// ページで状態の読み込みを試行するときに必要なイベント データを保持するために使用されるクラス。
    /// </summary>
    public class LoadStateEventArgs : EventArgs
    {
        /// <summary>
        /// このページが最初に要求されたとき、<see cref="Frame.Navigate(Type, Object)"/> 
        /// に渡されたパラメーター値。
        /// </summary>
        public Object NavigationParameter { get; private set; }
        /// <summary>
        /// 前のセッションでこのページによって保存された状態の
        /// ディクショナリ。ページに初めてアクセスするとき、状態は null になります。
        /// </summary>
        public Dictionary<string, Object> PageState { get; private set; }

        /// <summary>
        /// <see cref="LoadStateEventArgs"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="navigationParameter">
        /// このページが最初に要求されたとき、<see cref="Frame.Navigate(Type, Object)"/> 
        /// に渡されたパラメーター値。
        /// </param>
        /// <param name="pageState">
        /// 前のセッションでこのページによって保存された状態の
        /// ディクショナリ。ページに初めてアクセスするとき、状態は null になります。
        /// </param>
        public LoadStateEventArgs(Object navigationParameter, Dictionary<string, Object> pageState)
            : base()
        {
            this.NavigationParameter = navigationParameter;
            this.PageState = pageState;
        }
    }
    /// <summary>
    /// ページで状態の保存を試行するときに必要なイベント データを保持するために使用されるクラス。
    /// </summary>
    public class SaveStateEventArgs : EventArgs
    {
        /// <summary>
        /// シリアル化可能な状態で作成される空のディクショナリ。
        /// </summary>
        public Dictionary<string, Object> PageState { get; private set; }

        /// <summary>
        /// <see cref="SaveStateEventArgs"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="pageState">シリアル化可能な状態で作成される空のディクショナリ。</param>
        public SaveStateEventArgs(Dictionary<string, Object> pageState)
            : base()
        {
            this.PageState = pageState;
        }
    }
}

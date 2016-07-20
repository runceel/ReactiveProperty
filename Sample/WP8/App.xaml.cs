using System;
using System.Diagnostics;
using System.Resources;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WP8.Resources;

namespace WP8
{
    public partial class App : Application
    {
        /// <summary>
        /// Phone アプリケーションのルート フレームへの容易なアクセスを提供します。
        /// </summary>
        /// <returns>Phone アプリケーションのルート フレームです。</returns>
        public static PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Application オブジェクトのコンストラクターです。
        /// </summary>
        public App()
        {
            // キャッチできない例外のグローバル ハンドラーです。
            UnhandledException += Application_UnhandledException;

            // 標準 XAML の初期化
            InitializeComponent();

            // Phone 固有の初期化
            InitializePhoneApplication();

            // 言語表示の初期化
            InitializeLanguage();

            // デバッグ中にグラフィックスのプロファイル情報を表示します。
            if (Debugger.IsAttached)
            {
                // 現在のフレーム レート カウンターを表示します。
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // 各フレームで再描画されているアプリケーションの領域を表示します。
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // 試験的な分析視覚化モードを有効にします。
                // これにより、色付きのオーバーレイを使用して、GPU に渡されるページの領域が表示されます。
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // アプリケーションのアイドル状態の検出を無効にして、デバッガーの実行中に画面が
                // オフにならないようにします。
                // 注意: これはデバッグ モードのみで使用してください。ユーザーが電話を使用していないときに、ユーザーのアイドル状態の検出を無効にする
                // アプリケーションが引き続き実行され、バッテリ電源が消耗します。
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

        }

        // (たとえば、[スタート] メニューから) アプリケーションが起動するときに実行されるコード
        // このコードは、アプリケーションが再アクティブ化済みの場合には実行されません
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
        }

        // アプリケーションがアクティブになった (前面に表示された) ときに実行されるコード
        // このコードは、アプリケーションの初回起動時には実行されません
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
        }

        // アプリケーションが非アクティブになった (バックグラウンドに送信された) ときに実行されるコード
        // このコードは、アプリケーションの終了時には実行されません
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // (たとえば、ユーザーが戻るボタンを押して) アプリケーションが終了するときに実行されるコード
        // このコードは、アプリケーションが非アクティブになっているときには実行されません
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // ナビゲーションに失敗した場合に実行されるコード
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // ナビゲーションに失敗しました。デバッガーで中断します。
                Debugger.Break();
            }
        }

        // ハンドルされない例外の発生時に実行されるコード
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // ハンドルされない例外が発生しました。デバッガーで中断します。
                Debugger.Break();
            }
        }

        #region Phone アプリケーションの初期化

        // 初期化の重複を回避します
        private bool phoneApplicationInitialized = false;

        // このメソッドに新たなコードを追加しないでください
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // フレームを作成しますが、まだ RootVisual に設定しないでください。これによって、アプリケーションがレンダリングできる状態になるまで、
            // スプラッシュ スクリーンをアクティブなままにすることができます。
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // ナビゲーション エラーを処理します
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // 次のナビゲーションでバックスタックをクリアして、
            RootFrame.Navigated += CheckForResetNavigation;

            // 再初期化しないようにします
            phoneApplicationInitialized = true;
        }

        // このメソッドに新たなコードを追加しないでください
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // ルート visual を設定してアプリケーションをレンダリングできるようにします
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // このハンドラーは必要なくなったため、削除します
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            // アプリが 'リセット' ナビゲーションを受け取った場合、チェックする必要があります
            // ページ スタックをリセットする必要があるかどうかを確認するためのリセット要求を処理します
            if (e.NavigationMode == NavigationMode.Reset)
                RootFrame.Navigated += ClearBackStackAfterReset;
        }

        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // もう一度呼び出されないようにイベントの登録を解除します
            RootFrame.Navigated -= ClearBackStackAfterReset;

            // '新しい' (次の) ナビゲーションおよび '更新' ナビゲーションのスタックのみをクリアします
            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
                return;

            // UI の一貫性を維持するため、ページ スタック全体をクリアします
            while (RootFrame.RemoveBackEntry() != null)
            {
                ; // 何も実行しません
            }
        }

        #endregion

        // アプリのフォントと方向をそのローカライズされたリソース文字列で定義されたように初期化します。
        //
        // アプリケーションのフォントがそのサポートされる言語と一致し、各言語の FlowDirection がその従来の
        // 方向に従うようにするには、各 resx ファイルで ResourceLanguage と ResourceFlowDirection を
        // 初期化して、これらの値をそのファイルのカルチャと一致させる
        // 必要があります。例:
        //
        // AppResources.es-ES.resx
        //    ResourceLanguage の値は "es-ES" にする必要があります
        //    ResourceFlowDirection の値は "LeftToRight" にする必要があります
        //
        // AppResources.ar-SA.resx
        //     ResourceLanguage の値は "ar-SA" にする必要があります
        //     ResourceFlowDirection の値は "RightToLeft" にする必要があります
        //
        // Windows Phone アプリのローカライズの詳細については、http://go.microsoft.com/fwlink/?LinkId=262072 を参照してください。
        //
        private void InitializeLanguage()
        {
            try
            {
                // サポートされている各言語の ResourceLanguage リソース文字列で定義された
                // 表示言語と一致するようにフォントを設定します。
                //
                // 電話の表示言語がサポートされていない場合は、ニュートラル
                // 言語のフォントにフォールバックします。
                //
                // コンパイラ エラーが発生すると、ResourceLanguage がリソース ファイル
                // からなくなります。
                RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);

                // サポートされる各言語の ResourceFlowDirection リソース文字列に基づいて、
                // ルート フレームのすべての要素の FlowDirection を
                // 設定します。
                //
                // コンパイラ エラーが発生すると、ResourceFlowDirection がリソース ファイル
                // からなくなります。
                FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
                RootFrame.FlowDirection = flow;
            }
            catch
            {
                // ここで例外が発生した場合、その原因は、
                // ResourceLangauge がサポートされている言語コードに正しく設定されていないこと、
                // または ResourceFlowDirection が LeftToRight または RightToLeft 以外の
                // 値に設定されていることである可能性があります。

                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }
    }
}
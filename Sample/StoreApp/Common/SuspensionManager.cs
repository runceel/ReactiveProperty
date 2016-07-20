using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StoreApp.Common
{
    /// <summary>
    /// SuspensionManager は、グローバル セッション状態をキャプチャし、アプリケーションのプロセス継続時間管理を簡略化します。
    /// セッション状態は、さまざまな条件下で自動的にクリアされます。
    /// また、セッション間で伝達しやすく、アプリケーションのクラッシュや
    /// アップグレード時には破棄が必要な情報を格納する場合にのみ
    /// 使用する必要があります。
    /// </summary>
    internal sealed class SuspensionManager
    {
        private static Dictionary<string, object> _sessionState = new Dictionary<string, object>();
        private static List<Type> _knownTypes = new List<Type>();
        private const string sessionStateFilename = "_sessionState.xml";

        /// <summary>
        /// 現在のセッションのグローバル セッション状態へのアクセスを提供します。
        /// この状態は、<see cref="SaveAsync"/> によってシリアル化され、<see cref="RestoreAsync"/> によって復元されます。
        /// したがって、値は <see cref="DataContractSerializer"/> によってシリアル化可能で、
        /// できるだけコンパクトになっている必要があります。
        /// 文字列などの独立したデータ型を使用することを強くお勧めします。
        /// </summary>
        public static Dictionary<string, object> SessionState
        {
            get { return _sessionState; }
        }

        /// <summary>
        /// セッション状態の読み取りおよび書き込み時に <see cref="DataContractSerializer"/> に提供されるカスタムの型の一覧です。
        /// 最初は空になっています。
        /// 型を追加して、シリアル化プロセスをカスタマイズできます。
        /// </summary>
        public static List<Type> KnownTypes
        {
            get { return _knownTypes; }
        }

        /// <summary>
        /// 現在の <see cref="SessionState"/> を保存します。
        /// <see cref="RegisterFrame"/> で登録された <see cref="Frame"/> インスタンスは、現在のナビゲーション スタックも保存します。
        /// これは、アクティブな <see cref="Page"/> に状態を保存する機会を
        /// 順番に提供します。
        /// </summary>
        /// <returns>セッション状態が保存されたときに反映される非同期タスクです。</returns>
        public static async Task SaveAsync()
        {
            try
            {
                // 登録されているすべてのフレームのナビゲーション状態を保存します
                foreach (var weakFrameReference in _registeredFrames)
                {
                    Frame frame;
                    if (weakFrameReference.TryGetTarget(out frame))
                    {
                        SaveFrameNavigationState(frame);
                    }
                }

                // セッション状態を同期的にシリアル化して、共有状態への非同期アクセスを
                // 状態
                MemoryStream sessionData = new MemoryStream();
                DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<string, object>), _knownTypes);
                serializer.WriteObject(sessionData, _sessionState);

                // SessionState ファイルの出力ストリームを取得し、状態を非同期的に書き込みます
                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(sessionStateFilename, CreationCollisionOption.ReplaceExisting);
                using (Stream fileStream = await file.OpenStreamForWriteAsync())
                {
                    sessionData.Seek(0, SeekOrigin.Begin);
                    await sessionData.CopyToAsync(fileStream);
                }
            }
            catch (Exception e)
            {
                throw new SuspensionManagerException(e);
            }
        }

        /// <summary>
        /// 以前保存された <see cref="SessionState"/> を復元します。
        /// <see cref="RegisterFrame"/> で登録された <see cref="Frame"/> インスタンスは、前のナビゲーション状態も復元します。
        /// これは、アクティブな <see cref="Page"/> に状態を復元する機会を順番に提供します。
        /// ます。
        /// </summary>
        /// <returns>セッション状態が読み取られたときに反映される非同期タスクです。
        /// このタスクが完了するまで、<see cref="SessionState"/> のコンテンツには
        /// 完了します。</returns>
        public static async Task RestoreAsync()
        {
            _sessionState = new Dictionary<String, Object>();

            try
            {
                // SessionState ファイルの入力ストリームを取得します
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(sessionStateFilename);
                using (IInputStream inStream = await file.OpenSequentialReadAsync())
                {
                    // セッション状態を逆シリアル化します
                    DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<string, object>), _knownTypes);
                    _sessionState = (Dictionary<string, object>)serializer.ReadObject(inStream.AsStreamForRead());
                }

                // 登録されているフレームを保存された状態に復元します
                foreach (var weakFrameReference in _registeredFrames)
                {
                    Frame frame;
                    if (weakFrameReference.TryGetTarget(out frame))
                    {
                        frame.ClearValue(FrameSessionStateProperty);
                        RestoreFrameNavigationState(frame);
                    }
                }
            }
            catch (Exception e)
            {
                throw new SuspensionManagerException(e);
            }
        }

        private static DependencyProperty FrameSessionStateKeyProperty =
            DependencyProperty.RegisterAttached("_FrameSessionStateKey", typeof(String), typeof(SuspensionManager), null);
        private static DependencyProperty FrameSessionStateProperty =
            DependencyProperty.RegisterAttached("_FrameSessionState", typeof(Dictionary<String, Object>), typeof(SuspensionManager), null);
        private static List<WeakReference<Frame>> _registeredFrames = new List<WeakReference<Frame>>();

        /// <summary>
        /// <see cref="Frame"/> インスタンスを登録し、ナビゲーション履歴を <see cref="SessionState"/> に保存して、
        /// ここから復元できるようにします。
        /// フレームは、セッション状態管理に参加する場合、作成直後に 1 回登録する必要があります。
        /// 登録されしだい、指定されたキーに対して状態が既に復元されていれば、
        /// ナビゲーション履歴が直ちに復元されます。
        /// <see cref="RestoreAsync"/> はナビゲーション履歴も復元します。
        /// </summary>
        /// <param name="frame">ナビゲーション履歴を管理する必要があるインスタンスです
        /// <see cref="SuspensionManager"/></param>
        /// <param name="sessionStateKey">ナビゲーション関連情報を格納するのに
        /// 使用される <see cref="SessionState"/> への一意キーです。</param>
        public static void RegisterFrame(Frame frame, String sessionStateKey)
        {
            if (frame.GetValue(FrameSessionStateKeyProperty) != null)
            {
                throw new InvalidOperationException("Frames can only be registered to one session state key");
            }

            if (frame.GetValue(FrameSessionStateProperty) != null)
            {
                throw new InvalidOperationException("Frames must be either be registered before accessing frame session state, or not registered at all");
            }

            // 依存関係プロパティを使用してセッション キーをフレームに関連付け、
            // ナビゲーション状態を管理する必要があるフレームの一覧を保持します
            frame.SetValue(FrameSessionStateKeyProperty, sessionStateKey);
            _registeredFrames.Add(new WeakReference<Frame>(frame));

            // ナビゲーション状態が復元可能かどうか確認します
            RestoreFrameNavigationState(frame);
        }

        /// <summary>
        /// <see cref="SessionState"/> から <see cref="RegisterFrame"/> によって以前登録された <see cref="Frame"/> の関連付けを解除します。
        /// 以前キャプチャされたナビゲーション状態は
        /// 削除されます。
        /// </summary>
        /// <param name="frame">ナビゲーション履歴を管理する必要がなくなった
        /// 管理されます。</param>
        public static void UnregisterFrame(Frame frame)
        {
            // セッション状態を削除し、(到達不能になった弱い参照と共に) ナビゲーション状態が保存される
            // フレームの一覧からフレームを削除します
            SessionState.Remove((String)frame.GetValue(FrameSessionStateKeyProperty));
            _registeredFrames.RemoveAll((weakFrameReference) =>
            {
                Frame testFrame;
                return !weakFrameReference.TryGetTarget(out testFrame) || testFrame == frame;
            });
        }

        /// <summary>
        /// 指定された <see cref="Frame"/> に関連付けられているセッション状態のストレージを提供します。
        /// <see cref="RegisterFrame"/> で以前登録されたフレームには、
        /// グローバルの <see cref="SessionState"/> の一部として自動的に保存および復元されるセッション状態があります。
        /// 登録されていないフレームは遷移状態です。
        /// 遷移状態は、ナビゲーション キャッシュから破棄されたページを復元する場合に
        /// ナビゲーション キャッシュ。
        /// </summary>
        /// <remarks>アプリケーションは、フレームのセッション状態を直接処理するのではなく、<see cref="NavigationHelper"/> に依存して
        /// ページ固有の状態を管理するように選択できます。</remarks>
        /// <param name="frame">セッション状態が必要なインスタンスです。</param>
        /// <returns><see cref="SessionState"/> と同じシリアル化機構の影響を受ける状態の
        /// <see cref="SessionState"/>。</returns>
        public static Dictionary<String, Object> SessionStateForFrame(Frame frame)
        {
            var frameState = (Dictionary<String, Object>)frame.GetValue(FrameSessionStateProperty);

            if (frameState == null)
            {
                var frameSessionKey = (String)frame.GetValue(FrameSessionStateKeyProperty);
                if (frameSessionKey != null)
                {
                    // 登録されているフレームは、対応するセッション状態を反映します
                    if (!_sessionState.ContainsKey(frameSessionKey))
                    {
                        _sessionState[frameSessionKey] = new Dictionary<String, Object>();
                    }
                    frameState = (Dictionary<String, Object>)_sessionState[frameSessionKey];
                }
                else
                {
                    // 登録されていないフレームは遷移状態です
                    frameState = new Dictionary<String, Object>();
                }
                frame.SetValue(FrameSessionStateProperty, frameState);
            }
            return frameState;
        }

        private static void RestoreFrameNavigationState(Frame frame)
        {
            var frameState = SessionStateForFrame(frame);
            if (frameState.ContainsKey("Navigation"))
            {
                frame.SetNavigationState((String)frameState["Navigation"]);
            }
        }

        private static void SaveFrameNavigationState(Frame frame)
        {
            var frameState = SessionStateForFrame(frame);
            frameState["Navigation"] = frame.GetNavigationState();
        }
    }
    public class SuspensionManagerException : Exception
    {
        public SuspensionManagerException()
        {
        }

        public SuspensionManagerException(Exception e)
            : base("SuspensionManager failed", e)
        {

        }
    }
}

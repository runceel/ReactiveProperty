using System;

namespace System.ComponentModel
{
    // 概要:
    //     System.ComponentModel.INotifyDataErrorInfo.ErrorsChanged イベントにデータを提供します。
    public class DataErrorsChangedEventArgs : EventArgs
    {
        // 概要:
        //     System.ComponentModel.DataErrorsChangedEventArgs クラスの新しいインスタンスを初期化します。
        //
        // パラメーター:
        //   propertyName:
        //     エラーがあるプロパティの名前です。エラーがオブジェクト レベルの場合、null または System.String.Empty です。
        public DataErrorsChangedEventArgs(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        // 概要:
        //     エラーのあるプロパティの名前を取得します。
        //
        // 戻り値:
        //     エラーのあるプロパティの名前。 エラーがオブジェクト レベルの場合、null または System.String.Empty となります。
        public virtual string PropertyName { get; private set; }
    }
}
using System;
using System.Collections;

namespace System.ComponentModel
{
    // 概要:
    //     カスタムの同期検証および非同期検証サポートを提供するためにデータ エンティティ クラスに実装できるメンバーを定義します。
    public interface INotifyDataErrorInfo
    {
        // 概要:
        //     エンティティに検証エラーがあるかどうかを示す値を取得します。
        //
        // 戻り値:
        //     現在エンティティに検証エラーがある場合は true。それ以外の場合は false。
        bool HasErrors { get; }

        // 概要:
        //     プロパティまたはエンティティ全体の検証エラーが変更されたときに発生します。
        event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        // 概要:
        //     指定されたプロパティまたはエンティティ全体の検証エラーを取得します。
        //
        // パラメーター:
        //   propertyName:
        //     検証エラーを取得するプロパティの名前。または、エンティティ レベルのエラーを取得する場合は null または System.String.Empty。
        //
        // 戻り値:
        //     プロパティまたはエンティティの検証エラー。
        IEnumerable GetErrors(string propertyName);
    }
}

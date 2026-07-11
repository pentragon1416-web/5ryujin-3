# Assets/Scripts クラス概要

このドキュメントは、`Assets/Scripts` 配下にある各 C# スクリプトの役割と、どのような目的でクラスが作られているかをまとめたものです。

## 全体像

このプロジェクトのスクリプトは、大きく次の責務に分かれています。

- 盤面とゲーム進行: `Board`, `Map`, `Timer`
- 駒の選択・配置・在庫表示: `PieceCursor`, `PieceButton`, `Stock`
- 棋譜の記録・保存・読み込み: `MoveData`, `GameRecord`, `RecordManager`, `KifuManager`, `KifuListUI`, `KifuReplayContext`
- 棋譜の再生: `ReplayManager`, `ReplayPieceInfo`
- シーン遷移・外部リンク・画面調整: `Home`, `OpenURL`, `WebGLScreenFitter`

## Board.cs

`Board` はメインゲームの盤面とターン進行を管理する中心的な `MonoBehaviour` です。

主な役割は、ゲーム開始時にタイルやグリッド、駒選択ボタンを生成することです。`Start()` で 60 x 30 のタイルを作り、盤面上の区切り線を配置し、各プレイヤー用の駒ボタンを UI 上に並べます。

また、現在の手番を `public static bool turn` として保持しています。`false` が 1P、`true` が 2P として扱われ、`Change()` で手番を切り替えます。手番が変わるとターン表示の位置を動かし、`Timer.ResetCounter()` で制限時間もリセットします。

勝敗判定では、`Map.reach` に記録された各プレイヤーの到達位置を比較し、`Judge()` から `Win()` を呼び出します。`Win()` は勝敗テキストを表示し、駒カーソルを停止します。

## MagnetMap.cs / Map クラス

ファイル名は `MagnetMap.cs` ですが、中で定義されているクラス名は `Map` です。盤面上に駒を置けるかどうかを判定し、配置済みマスや磁石位置を管理するためのクラスです。

`Map` は `MonoBehaviour` ではなく通常の C# クラスです。`PieceCursor` が内部で `new Map()` して使っています。

主な管理対象は次の通りです。

- `placedTiles`: すでに埋まっているタイル座標
- `magnetMap`: 磁石の座標と、どのプレイヤー・どの駒グループに属するか
- `dragonCount`: 各プレイヤーの通常 P 駒の管理番号
- `pdict`: P 駒本体と、そのタイル座標の記録
- `reach`: タッチダウン判定用の到達位置

`Add(PieceCursor piece)` が中心メソッドです。選択中の駒について、盤面外にはみ出していないか、既存タイルと重なっていないか、自分の駒と正しく接続しているか、磁石位置が有効かを確認します。条件を満たす場合は配置済みマスや磁石情報を更新し、タッチダウン駒の場合は元の P 駒を消す処理も行います。

## PieceCursor.cs

`PieceCursor` は、プレイヤーが選択した駒をマウスやタッチに追従させ、回転・反転・配置するためのクラスです。

`Update()` では、タッチ操作がある場合は `HandleTouch()`、なければ `HandleMouse()` を呼びます。PC ではマウス位置に駒を追従させ、スクロールで回転、左クリックで配置、右クリックで反転します。スマートフォンではタッチ位置に合わせてカーソルを動かします。

`Select(int n, Stock s)` は、駒ボタンや在庫から呼ばれる選択処理です。既存の選択駒を破棄し、`pieces` リストから対応する prefab を生成します。その後、子オブジェクトの `Magnet` と `Tile` タグを集め、手番に応じた色を付けます。

`Put()` は配置処理です。現在の回転角、反転状態、座標、プレイヤー情報を取得し、`Map.Add(this)` に配置可能か判定させます。成功した場合は `RecordManager` に手を記録し、持ち駒数を減らし、駒を盤面側へ移してから手番を切り替えます。

## PieceButton.cs

`PieceButton` は UI 上の駒選択ボタンを扱うクラスです。

`Start()` で自身の `Button` コンポーネントに `ClickSelect()` を登録します。クリック時には、ボタンが現在の手番のプレイヤー用かどうかを確認し、違う場合は選択できないようにしています。

通常の駒は `Stock.Select(number, turn)` を通して選択します。一方、`number == 12` のタッチダウン駒は在庫を使わず、直接 `PieceCursor.instance.Select(number, null)` を呼び出します。

## Stock.cs

`Stock` は、各駒の残り枚数と見た目の在庫表示を管理するクラスです。

初期状態では `count = 5` で、`Start()` で同じ駒画像を 5 個生成し、ボタン上に並べます。配置が成功すると `Decrement()` が呼ばれ、残数を 1 減らし、表示中の画像も 1 つ削除します。

`Select(int n, bool turn)` は駒選択処理です。現在の手番と一致しているか、残数があるか、`PieceCursor.instance` が存在するかを確認した上で、`PieceCursor` に駒選択を依頼します。

## Timer.cs

`Timer` は手番ごとの制限時間を管理するクラスです。

`limit` は static な制限時間で、ホーム画面の選択によって変更されます。`Update()` で経過時間 `counter` を増やし、UI の `Image.fillAmount` に残り時間を反映します。

時間切れになると `passCounter` を増やし、選択中の駒を破棄して手番を切り替えます。連続パスが一定数に達した場合は `Board.instance.Judge()` を呼んで勝敗判定を行います。

棋譜再生中は `KifuReplayContext.HasKifu()` を見て制限時間を非常に長くし、表示を `ANL` に変えます。

## MoveData.cs

`MoveData` は 1 手分の棋譜データを表すシリアライズ用クラスです。

保存している情報は、手数、プレイヤー、駒の種類、回転角、反転状態、配置座標、タッチダウンかどうかです。`RecordManager.AddMove()` で作られ、`GameRecord` の `moves` に追加されます。

## GameRecord.cs

`GameRecord` は棋譜全体を表すデータクラスです。

`List<MoveData> moves` を持ち、対局中に発生したすべての手を順番に保存します。`JsonUtility` で JSON に変換できるように `[Serializable]` が付いています。

## RecordManager.cs

`RecordManager` は現在の対局の棋譜を記録・保存するクラスです。

`AddMove()` で 1 手分の `MoveData` を作成し、`record.moves` に追加します。`SaveRecord()` は現在の棋譜を `Application.persistentDataPath/record.json` に保存します。

`SaveAsKifu()` は、現在の棋譜を正式な保存棋譜として `KifuManager` に渡します。棋譜名は `KIF.001` から `KIF.999` までの連番と日付で作られます。保存時には JSON 経由で `GameRecord` をコピーし、参照共有による意図しない変更を避けています。

## KifuManager.cs

`KifuManager` は保存済み棋譜のリストを管理するクラスです。

`Awake()` で singleton として初期化され、`DontDestroyOnLoad(gameObject)` によってシーンをまたいで保持されます。保存先は `Application.persistentDataPath/kifu.json` です。

`SaveKifu()` は棋譜名と `GameRecord` を受け取り、リストに追加します。最大保存数は `MAX_KIFU = 6` で、超えた場合は古い棋譜から削除します。`Load()` は保存ファイルを読み込み、存在しない場合やデータが壊れている場合に空のリストを用意します。

## KifuListUI.cs

`KifuListUI` は保存済み棋譜の一覧画面を作るクラスです。

`Start()` で `CreateList()` を呼び、`Application.persistentDataPath/kifu.json` を読み込みます。保存された棋譜があれば、`buttonPrefab` を `parent` の下に生成し、ボタンの表示名を棋譜名にします。

各ボタンを押すと、選択した `GameRecord` を `KifuReplayContext.Set()` に渡し、`mainSceneName` のシーンへ遷移します。これにより、次のシーンで `ReplayManager` が再生対象の棋譜を取得できます。

## KifuReplayContext.cs

`KifuReplayContext` は、シーン間で再生対象の棋譜を受け渡すための static クラスです。

`selectedKifu` に `GameRecord` を保持し、`Set()`, `Clear()`, `HasKifu()` を提供します。Unity のシーン遷移では通常のオブジェクト参照が失われることがあるため、簡単な一時保存場所として作られています。

## ReplayManager.cs

`ReplayManager` は棋譜を 1 手ずつ再生するためのクラスです。

`Start()` で `KifuReplayContext` に棋譜があるか確認し、存在する場合は `loadedKifu` に受け取ります。`Next()` で 1 手進め、`Prev()` で 1 手戻し、`ResetReplay()` で最初の状態に戻します。

再生時は `RebuildReplay()` が現在の手数までの駒をすべて作り直します。`PlaceMove()` は `MoveData` をもとに prefab を生成し、座標、回転、反転、色を再現します。タッチダウン手の場合は、対象プレイヤーの開始 P 駒を消してからタッチダウン用 prefab を置きます。

## ReplayPieceInfo.cs

`ReplayPieceInfo` は、再生用の駒に駒種やプレイヤー情報を持たせるための小さな `MonoBehaviour` です。

現在のコードでは `pieceType` と `player` のフィールドのみを持っています。将来的にリプレイ中の駒を識別したり、クリック時に情報表示したりする目的で使える補助クラスです。

## Home.cs

`Home` はホーム画面からゲームを開始するためのクラスです。

`Play(int sec)` は選択された制限時間を `Timer.limit` に設定し、`MainScene` を読み込みます。`Leave()` は現在 `Debug.Log(0)` のみで、終了処理や別画面遷移はまだ実装されていません。

## OpenURL.cs

`OpenURL` は指定した URL をブラウザで開くためのクラスです。

`url` の初期値は `https://5ryujin.com` で、`Open()` が呼ばれると `Application.OpenURL(url)` を実行します。UI ボタンから公式サイトや外部ページを開く用途のクラスです。

## WebGLScreenFitter.cs

`WebGLScreenFitter` はカメラの表示領域を 16:9 に保つためのクラスです。

`Update()` で現在の画面アスペクト比を調べ、対象アスペクト比 `16:9` と比較します。縦長の場合は上下に余白が出るように、横長の場合は左右に余白が出るように `Camera.rect` を調整します。WebGL ビルドなど、ブラウザのウィンドウサイズが変わる環境で表示崩れを抑える目的があります。

## 注意点

- コメントやログ文字列の一部が文字化けしています。おそらく日本語コメントの文字コードが途中で崩れたものです。
- `MagnetMap.cs` のファイル名と中のクラス名 `Map` が一致していません。Unity では `MonoBehaviour` でなければ必須ではありませんが、把握しやすさの面では名前を合わせる余地があります。
- `Map` は `PieceCursor` 内で生成されるため、盤面状態は `PieceCursor` の寿命に依存します。
- `Board.turn`, `Board.instance`, `PieceCursor.instance`, `KifuManager.instance` など static 参照が多く、シーン遷移や初期化順の影響を受けやすい構成です。

## このプロジェクトが行うこと What This Project Does
販売管理システム Creative Vision.net のWPFクライアントを提供します
This project provides a WPF client for the sales management system Creative Vision.net.

## このプロジェクトが有益な理由 Why This Project is Beneficial
BSD 3-Clause License が適用されるオープンソースプロジェクトです。
誰でも自由に改変し使用することが可能です。
This is an open-source project licensed under the BSD 3-Clause License. Anyone is free to modify and use it.

## このプロジェクトの開発スケジュール Project Development Schedule
<pre>
2025/05/01 プロジェクト正式開始：既存CV.netに対しクライアント側をBiz/BrowserからWPFに全面移行する
2025/05/12 クローズドリポジトリ作成、プロジェクト基本構成の組み立て、一部モジュールのテスト
	開発環境：VS2022、.NET9(2024/11/12release) .NET10(2025/11予定)、WPF、MVVM
	WPF版のCvnetClientはオープンソース、将来的なCV.netフルオープンソース化を見据えた開発
	Bizクライアントライセンス不要、CV.netのシェア拡大、CV.netの開発を外部委託しやすくする、VS2022の利用
	3階層システムの開発思想の普及、基幹システム開発のハードルを下げる
2025/05/20 クローズドからGitHubリポジトリ(このプロジェクト)への移行、基本の開発方針は現行画面をそのままWPFへ
（開発予定）
	2025/06 CV.netのWPFクライアントのチーム開発開始
	2025/10 パッケージ全体の開発完了と.NET9から.NET10への対応、一部のブラッシュアップ
	2025/11 正式リリース .NET10(2025/11予定)への対応
	2025/11 CV.netのサーバ部分の開発 (CVnetClientの構造を考慮し設計)
	2026/05 CV.netフルオープンソース化公開
		(ユーザメリット)初期費用の大幅削減、開発会社依存リスクの削減
</pre>

## このプロジェクトでの技術的トピック What is This Project Technical Topic
- MVVMでのダイアログ呼び出し問題の画期的な解決
- ViewModelからViewを判断するための方法
- 3TierにおけるGUI画面とデータ分離の指針
- MVVMのViewでコードビハインドの完全排除

## このプロジェクトのメンテナンス者とコントリビューター Maintainers and Contributors of This Project
<pre>
GitHub Desktop をインストールし、自分のアカウントで連携します。
collaboratorとして参加リクエストします。
リポジトリをクローンしたあと、自分用のブランチを作成し、そのブランチ上て開発開始します。
ローカル上で作業したらcommitします。ある程度まとまったら、作業内容を共有するため、Pull requestを行います。
タスクの詳細をわかりやすく記述し、チームメンバーにレビューしてもらいます。ここで修正があれば修正作業をしcommitします。
最終確認がとれれば、mainブランチにmergeします(merge pull request)。
※mainブランチには絶対に勝手にマージしないこと！
チームの作業内容を共有するため、originブランチをfetchし、origin/mainを自分branchにマージします。
</pre>


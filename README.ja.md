# Status Rate HUD

The Long Darkの各ステータスの残量と、**現実の1秒あたりの増減速度**を表示するMODです。

[English](README.md) · [ダウンロード](https://github.com/SuzyQrious/StatusRate-Hud/releases/latest)

## 主な機能

- 体温・元気・水分・満腹・体調をパーセント表示。
- 約0.5秒ごとに実際の増減を測り、現実の1秒あたりで表示。
- 白系の文字と細いゲージ。文字サイズ・表示位置を設定可能。
- Escメニュー・インベントリなどでは非表示。
- このMODの設定タブを開いたときだけ、位置調整用プレビューを表示。
- ゲーム内の言語に合わせて19言語へ切り替え。未対応言語は英語。
- ステータスやセーブデータは変更しません。

「80.0% −0.025%/秒」は、現実の1秒で0.025パーセントポイント減る速度です。飲食・被ダメージ・時間加速も含む直近の実測値で、将来の予測ではありません。

## 必要なもの

動作確認した環境は、Windows版 The Long Dark **2.60のサバイバルモード**です。

1. [MelonLoader 0.7.3](https://github.com/LavaGang/MelonLoader)
2. [ModSettings 2.2.5](https://github.com/DigitalzombieTLD/ModSettings)
3. StatusRateHud.dll

MelonLoaderとModSettingsは別途導入が必要です。他のゲームバージョン・OS・ストーリーモードは未検証です。

## 導入方法

1. ゲームを終了します。
2. ゲームの `tld.exe` にMelonLoaderを導入し、`Mods` に `ModSettings.dll` を配置します。
3. [Releases](https://github.com/SuzyQrious/StatusRate-Hud/releases)から **StatusRateHud.dll** をダウンロードします。
4. `StatusRateHud.dll` をゲームのインストール先の `Mods` フォルダーへ入れます。

最終的な配置：`TheLongDark\Mods\StatusRateHud.dll`

Steamの「管理 → ローカルファイルを閲覧」でインストール先を開けます。
GitHubの **Source code** は開発用ソースです。導入には `StatusRateHud.dll` を使用してください。

## 設定とプレビュー

**オプション → Mod Settings → Status Rate HUD** で表示のオン／オフ・文字サイズ・横位置・縦位置を設定できます。

このMODの設定タブを選ぶとプレビューが表示され、スライダー操作が即座に反映されます。確定すると設定が保存されます。
プレビューの値は固定（実測前は75％）で、増減速度は0です。別の設定タブやEscメニューでは表示しません。

## 言語

日本語、英語、ドイツ語、ロシア語、フランス語、スペイン語、フィンランド語、韓国語、ノルウェー語、ポルトガル語（ポルトガル／ブラジル）、中国語（簡体字／繁体字）、スウェーデン語、トルコ語、オランダ語、イタリア語、ポーランド語、ウクライナ語に対応。

Windowsの言語ではなくゲーム内言語に連動します。MOD名はすべての言語で「Status Rate HUD」です。設定画面を開いたまま言語を変更した場合は、設定を開き直してください。
翻訳のネイティブ校閲と全言語での見た目の検証は未完了です。

## 更新・削除

ゲームを終了してから `Mods\StatusRateHud.dll` を差し替え、または削除します。古い版を別名でMods内に残さないでください。更新時には設定を引き継ぎます。

**1.3.0と1.3.1には読み込み不具合があるため、1.3.2以降をご利用ください。**

1.3.2は実機でユーザー確認済みです。計算と翻訳データの自動テストは、通常環境とMelonLoaderと同じ多言語処理制約下の両方で合格しています。

開発用のビルド・テスト方法は[英語README](README.md#build-from-source)、更新履歴は[CHANGELOG](CHANGELOG.md)を参照してください。


# Quick Launch System Tray
## Quick Launch System Trayについて
- [Quick Launch System Tray]は、Windows7/10の頃のQuick Launchの代りとして作成したランチャープログラムです。
- 起動するとシステムトレイ(タスクトレイ)に常駐します。  
  ![Launch画面](https://github.com/user-attachments/assets/8e1699fb-ca32-4bc5-9c74-cf7509abd6a3)
- アイコンをクリックすると、[Quick Launch]フォルダのショートカットを表示します。  
  ![Quick Launch表示](https://github.com/user-attachments/assets/a53976ed-351a-4619-9fbf-538f88e7ce55)
- [Quick Launch]フォルダの場所は、エクスプローラのアドレスで```shell:Quick Launch```と入力すると表示され  
  ```C:\Users\[user]\AppData\Roaming\Microsoft\Internet Explorer\Quick Launch```  
  となります。

## システムメニューについて
- 表示されるメニューの下の[System Menu]の内容は以下の通りです。

| 項目                     | 説明                                               |
| ------------------------ | -------------------------------------------------- |
| Open Quick Launch Folder | [Quick Launch]フォルダを開きます                   |
| Refresh                  | [Quick Launch]フォルダを読み込みなおして表示します |
| Quit                     | アプリケーションを終了します                       |

## 起動引数について
- [Quick Launch]フォルダ以外から表示したい場合はショートカットを作成して起動引数でフォルダを指定します。
- 例えばプログラムフォルダの内容を表示したい場合はショートカットの引数を以下のようにします。  
  ```QuickLaunchSystemTray.exe "C:\ProgramData\Microsoft\Windows\Start Menu\Programs"```

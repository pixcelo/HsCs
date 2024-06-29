# layered-architecture
C#

## Notes

### アーキテクチャ
どのような構成するか？
- プレゼン層: UI (Web, WinForm) 主に画面
- ドメイン層: ビジネスロジック(〇〇Serviceクラス)
- インフラ層(データアクセス層): 〇〇Repositoryクラス

各レイヤーを疎結合にしてテスト可能にしたい

プレゼン層は、差し替え可能にする

APIコントローラは名前解決してDIしたServiceクラスを操作する

Serviceクラス→RepositoryクラスをDIする

Repositoryクラス→DBやインメモリへのアクセスをDIする

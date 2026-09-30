# BD2 Infinite Gacha v0.2.4

## 简体中文

### 更新内容

- 增加目标达成提醒：Windows 通知、弹窗、声音可独立勾选，默认仅 Windows 通知。
- 增加「测试提醒」，设置自动保存，刷新期间也能调整。
- 达标停止后只提醒一次；手动停止、错误和旧结果不误报；单个提醒方式失败不影响其他方式。
- 停止条件区域整体滚动，较小窗口也能完整编辑。支持中英双语。

### 下载

| 版本 | 运行环境 | 建议 |
| --- | --- | --- |
| **Portable** | 自带 .NET，无需另装运行库 | 大多数用户 |
| **Lite** | 需要 .NET Desktop Runtime 8 x64 | 已安装桌面运行库、希望减小下载体积 |

两版功能相同，均为单 EXE，内置简体中文／English；ZIP 附带双语说明与许可证。用 `SHA256SUMS.txt` 核对下载。

### 升级

停止并关闭旧工具后打开新版，已有分组与设置保留。0.2.3 升到 0.2.4 无需重启游戏，本次未改动 Runtime5；更早组件仍按原要求升级。Windows 通知可能受系统免打扰设置影响，声音遵循系统音量及声音方案。

作者发布版免费。第三方收费不代表作者参与、背书或提供服务。[使用说明与风险提示](https://github.com/MadestSamurai/bd2-infinite-gacha/blob/main/README.md)。

## English

### Changes

- Adds target reached alerts: independently select Windows notification, Popup and Sound. Defaults to Windows notification only.
- Adds Test alerts and saved preferences that can be changed during a run.
- Alerts once after a matching result stops the run. Manual stops, errors and old results do not trigger alerts. One failed channel does not block the others.
- Makes the stop-condition panel scroll as a whole for smaller windows. Includes Chinese and English text.

### Downloads

| Build | Runtime | Recommended for |
| --- | --- | --- |
| **Portable** | Includes .NET; no separate runtime needed | Most users |
| **Lite** | Requires .NET Desktop Runtime 8 x64 | Smaller download when the desktop runtime is installed |

Both builds offer the same features in one EXE with Simplified Chinese / English. ZIPs include bilingual documentation and licenses. Verify downloads with `SHA256SUMS.txt`.

### Upgrade

Stop automation and close the old tool, then open the new version. Groups and preferences are retained. Upgrading from 0.2.3 to 0.2.4 does not require a game restart; Runtime5 is unchanged. Older component upgrades retain their existing requirements. Windows may suppress notifications in Do not disturb; audio follows system volume and sound settings.

Official releases are free. Third-party fees do not imply the author's involvement, endorsement or support. [Usage and risk notice](https://github.com/MadestSamurai/bd2-infinite-gacha/blob/main/README.en.md).

---

# BD2 Infinite Gacha v0.2.3

## 简体中文

### 更新内容

- 主窗口增加免费开源署名：GitHub MadestSamurai／B站 MadSamurai。
- 新增「来源与说明」，可查看并复制官方仓库与下载链接；随界面切换中英文。
- 统一双语 README、来源与风险说明，ZIP 附带完整说明；MIT 许可证保持不变。

### 下载

| 版本 | 运行环境 | 建议 |
| --- | --- | --- |
| **Portable** | 自带 .NET，无需另装运行库 | 大多数用户 |
| **Lite** | 需要 .NET Desktop Runtime 8 x64 | 已安装桌面运行库、希望减小下载体积 |

两版功能相同，内置简体中文／English。EXE 可独立使用；ZIP 附带双语说明与许可证。用 `SHA256SUMS.txt` 核对下载。

### 升级

停止自动操作并关闭旧工具，再打开新版。已有设置保留；本次主要更新来源与说明界面。

作者发布版免费。第三方收费不代表作者参与、背书或提供服务。[使用说明与风险提示](https://github.com/MadestSamurai/bd2-infinite-gacha/blob/main/README.md)。

## English

### Changes

- Adds free-release attribution to the main window: GitHub MadestSamurai / Bilibili MadSamurai.
- Adds About & source with selectable official repository and download links, following the selected UI language.
- Standardizes bilingual READMEs and source/risk notices, also included in ZIPs. The MIT License is unchanged.

### Downloads

| Build | Runtime | Recommended for |
| --- | --- | --- |
| **Portable** | Includes .NET; no separate runtime needed | Most users |
| **Lite** | Requires .NET Desktop Runtime 8 x64 | Smaller download when the desktop runtime is installed |

Both builds have identical features and include Simplified Chinese / English. EXEs run independently; ZIPs include bilingual documentation and licenses. Verify downloads with `SHA256SUMS.txt`.

### Upgrade

Stop automation and close the old tool, then open the new version. Existing settings are retained; this update primarily changes attribution and source information.

Official releases are free. Third-party fees do not imply the author's involvement, endorsement or support. [Usage and risk notice](https://github.com/MadestSamurai/bd2-infinite-gacha/blob/main/README.en.md).

---

# BD2 Infinite Gacha v0.2.2

## 简体中文

### 更新内容

- 卡池下拉框改为显示截止日期和时间，按新到旧排列，不再是一排相同的活动名。
- 显示「当前结果」标记和卡池编号；时间采用电脑本地时区，悬停可查看时区及游戏活动名。
- 没有提供日期的历史卡池排在末尾。切换语言或日期更新后，保留当前选择、A／B分组和停止条件。

### 下载

| 版本 | 运行环境 | 建议 |
| --- | --- | --- |
| **Portable** | 内置 .NET 运行时 | 首次使用推荐，下载即用 |
| **Lite** | 需安装 [.NET Desktop Runtime 8 x64](https://dotnet.microsoft.com/download/dotnet/8.0) | 已安装运行时，下载更小 |

适用于 Windows x64。EXE 可独立运行，ZIP 附说明与许可证；使用 `SHA256SUMS.txt` 校验下载。两版功能相同，均内置简体中文／English。

### 升级

暂停并关闭旧工具，正常重启游戏，再打开新版连接。本机设置保留；具体功能与设置迁移见上面的更新内容。

[使用说明与风险声明](https://github.com/MadestSamurai/bd2-infinite-gacha/blob/main/README.md)

## English

### Changes

- The pool selector now shows end dates and times, newest first, instead of identical event names.
- A **Current result** marker and pool IDs identify the event. Times use your computer’s local time zone; hover for the time zone and in-game name.
- Pools without a supplied end date stay at the bottom. Language switches and date updates preserve your selection, targets and stop rules.

### Downloads

| Edition | Runtime requirement | Recommended for |
| --- | --- | --- |
| **Portable** | .NET included | Most users; download and run |
| **Lite** | [.NET Desktop Runtime 8 x64](https://dotnet.microsoft.com/download/dotnet/8.0) | Smaller download if the runtime is installed |

For Windows x64. EXEs run on their own; ZIPs include documentation and licenses. Verify downloads against `SHA256SUMS.txt`. Both editions have the same features and include Simplified Chinese / English.

### Upgrade

Pause and close the old assistant, restart the game normally, then connect with the new version. Local preferences are retained; see Changes above for feature and setting migrations.

[Usage and risk disclaimer](https://github.com/MadestSamurai/bd2-infinite-gacha/blob/main/README.en.md)

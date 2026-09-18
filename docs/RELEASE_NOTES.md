# 0.2.1 · 首个公开版本 / First public release

## 中文

- 独立无限抽抽乐助手：自动读取可抽服装与当前账号破度，左右 A／B 目标列表，一键未满入 B，自定义多档停止条件。
- 默认排除升到 +5 后的溢出服装，支持按张数或不同服装计数；默认刷新间隔1000ms，可调整。
- 自动跳过抽取动画。短暂文件占用、读取失败和界面过渡可以恢复接续；关闭游戏弹窗后继续。
- 新增有体积限制的恢复／停止事件日志，方便定位偶发中断。
- 内置简体中文／English即时切换，附独立中英文README；升级保留此前分组和条件。

**升级**：关闭旧助手并正常重启游戏，再打开新版连接。

## English

- Standalone Infinite Gacha assistant with live costume and enhancement detection, side-by-side A/B target lists, one-click unmaxed targets, and configurable stop tiers.
- Excess copies beyond +5 are excluded by default. Choose copy counting or distinct costumes; the default 1000ms reroll interval is adjustable.
- Automatically skips draw animations. Brief file locks, missing reads and UI transitions can recover without restarting the task. Rerolling continues after game popups close.
- Adds size-limited recovery and stop-event logs to help diagnose interruptions.
- Switch between English and Simplified Chinese in the app. Both READMEs are included, and existing targets and rules are preserved.

**Upgrading:** close the previous assistant and restart the game before connecting the new version.

## 下载选择 / Downloads

| 版本 / Build | 包含运行时 / Bundled runtime | 要求 / Requirements |
| --- | --- | --- |
| **Portable（推荐 / Recommended）** | .NET included | Windows x64，无额外安装 / no additional installation |
| **Lite** | No | Windows x64 + .NET 8 Windows Desktop Runtime x64 |

两版均支持双语。EXE可直接启动；ZIP附说明和许可证；`SHA256SUMS.txt`用于完整性校验。
Both builds are bilingual. EXEs run directly; ZIPs include documentation and licenses. Use `SHA256SUMS.txt` to verify downloads.

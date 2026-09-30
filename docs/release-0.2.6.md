# 0.2.6

修复尚未连接游戏时启动窗口就发送停止指令，导致程序打不开。新增真实未连接通信层的成品启动检查。

Fix startup failing because an unconnected window sent a stop command. Packaged startup now tests the real disconnected transport.

| 版本 / Edition | 说明 / Requirements |
|---|---|
| Portable | 内置 .NET / Includes .NET |
| Lite | 需要 .NET 8 Desktop Runtime x64 / Requires .NET 8 Desktop Runtime x64 |

保持游戏运行，停止旧工具后连接新版即可交接。Keep the game running, stop the previous tool and connect the new build.

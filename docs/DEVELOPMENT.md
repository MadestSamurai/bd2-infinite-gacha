# Development / 开发

Windows x64 + .NET 8 SDK. Run `build.ps1 -Locked` to build and test without a game installation; `package.ps1 -Locked` creates both release flavors and runs their GUI checks. Dependency lock files are committed. The CI workflow builds pull requests/main/tags, checks the tag matches `Directory.Build.props`, and creates a draft release for artifact review.

## Layout

- `shared/`: portable contracts, rule evaluation, request state machine and publication recovery.
- `core/`: desktop lease controller, atomic preferences, connection and local diagnostics.
- `hook/`: owned runtime source, compiled locally against the installed client at connection time.
- `compatibility/`: interface-shape matching and one-way method fingerprints. `contract.json` contains metadata, not game IL or tables.
- `desktop/`: WPF UI, group picker and language presentation.
- `localization/`: language catalogs; see [LOCALIZATION.md](LOCALIZATION.md).
- `tests/`, `compatibility-tests/`: game-free rule, recovery, localization and synthetic-interface tests.

## State guarantees

One preview request is outstanding at a time. A new draw requires a successful response, a new native result event, ten recognized items and a ready result UI. Matching results stop before another request. A network timeout never retries an ambiguous draw. Skip clicks are restricted to the active preview's native skip button.

The host renews a 20-second lease at most every two seconds. Short missing snapshots wait up to 15 seconds; known identity changes stop immediately. The runtime also tolerates short missing UI context. If publication fails, it retries the same snapshot and performs no further game action until publication recovers. Popups pause result-animation waiting; successful responses have a separate 120-second result-animation deadline, rather than sharing the 30-second server-response timeout.

Both host and runtime retain bounded event logs (current and previous file, about 1 MiB each). They do not deliberately log inventories, account names, credentials or individual draw contents. Diagnostic exceptions may contain local paths; review logs before sharing.

## Optional local compatibility check

```powershell
dotnet run --project compatibility-cli -c Release -- check "C:\path\to\BrownDust II_Data\Managed" ".build\client-check"
```

This reads installed assemblies and compiles the hook offline. It does not inject or operate the game. The ordinary public build contains no game DLLs and does not require this command. Do not commit local DLLs or captures. Ambiguous or changed interfaces fail closed rather than selecting by declaration order.

## Validation boundary

The preceding private version was observed completing 33 rerolls and 34 native skips. Public 0.2.0 adds recovery behavior verified by deterministic fault-injection tests and two real client metadata/compilation checks. A long unattended game session is still useful to validate the new recovery behavior under naturally occurring faults; unit tests are not represented as server-side gameplay evidence.

## 中文摘要

日常构建不需要游戏。发布时应同时检查规则、恢复状态机、双语词条、两种成品界面和运行时依赖差异；有本机客户端时补充接口匹配与组件编译。服务端请求只允许一笔在途，结果未确定时不会重发。新增容错已做离线故障注入，长期自然实机表现仍需持续观察。

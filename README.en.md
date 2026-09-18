# BD2 Infinite Gacha

English | [简体中文](README.md)

A standalone Infinite Gacha assistant for the Brown Dust 2 Windows PC client. It reads the available costumes and your enhancement levels, rerolls according to priority groups and stop rules, skips draw animations, and stops on a match. You decide whether to keep the result in the game.

[Download the latest release](https://github.com/MadestSamurai/bd2-infinite-gacha/releases/latest)

## Choose a build

Both builds include **English and Simplified Chinese**, switchable in the app.

| Build | Bundles .NET | Additional requirement | Recommended for |
| --- | --- | --- | --- |
| **Portable** | Yes | None | Most users |
| **Lite** | No | .NET 8 Windows Desktop Runtime **x64** | Users who already have the runtime and prefer a smaller download |

Download an EXE and run it. ZIPs also contain both READMEs and licenses. Verify downloads against `SHA256SUMS.txt` with PowerShell: `Get-FileHash file.exe -Algorithm SHA256`.

## Getting started

1. Start the game and log in. Open Infinite Gacha, make the first 10-pull manually, and stay on the result screen.
2. Open the assistant and select **Connect game**. Restart the game first if you upgraded the component or connected another tool in this game session.
3. Select **Add unmaxed → B** to add unowned and +0 through +4 costumes to the left-hand B list.
4. Select priority targets in B and click **To A →**. Ctrl/Shift supports multiple selection; double-click or Enter transfers between groups, and Delete removes a target. Use **Add costumes…** for ungrouped costumes.
5. Set the top A threshold and any lower-tier rules, then select **Start rerolling**. A match stops the assistant. Keeping or purchasing the result remains a manual decision in the game.

Use the top-right language selector at any time. Switching languages does not restart rerolling or change your targets. Costume, character and pool names follow the **game's language**.

The pool selector identifies events by **end date**, newest first. Dates use your computer’s local time zone; hover to see the time zone and the in-game event name. **Current result** marks the pool shown in the game. Pool IDs distinguish matching dates, and pools without a supplied end date stay at the bottom. Reordering preserves your targets and stop rules.

## Stop rules

A contains priority targets; B contains secondary targets. A costume belongs to at most one group. Set an unconditional A threshold from 1 to 10. Lower tiers appear automatically and can each be enabled separately.

For example, set the top threshold to 3:

| Actual A count | Stop requirement |
| --- | --- |
| ≥ 3 | Stop unconditionally |
| = 2 | B ≥ 3 |
| = 1 | B ≥ 5 |
| = 0 | Disabled; keep rerolling |

Any enabled rule can stop the run. Lower tiers require an **exact A count**. A+B cannot exceed 10. Invalid or unfinished input is flagged and preserved, including values in temporarily hidden tiers.

- **Count duplicate copies separately** is enabled by default. Turn it off to count distinct costume IDs instead.
- **Exclude excess copies** is enabled by default in copy-counting mode. Count only copies needed to reach +5: up to 6 if unowned, 5 at +0, 1 at +4, and none at +5. You can disable this independently.
- **Reroll interval** defaults to 1000ms, adjustable from 100 to 60000ms. The previous complete result must be checked before the next request. Network and game animations can make the actual interval longer.
- Targets and rules are saved separately per account and pool. Closing or stopping the assistant does not enable automatic restarting.

## Recovery and diagnostics

Brief missing-state or UI transitions wait up to 15 seconds for recovery. Temporary file locks are retried without discarding the current task. Close game popups to continue automatically. Animation skipping uses the game's normal skip action.

Account, process or pool changes, locked results and explicit server errors still stop the run. A server request with no response after 30 seconds is not blindly resent: it might already have produced a result. Stopping or closing the assistant prevents new requests; an outstanding request can still complete normally.

Local data is stored in `%LOCALAPPDATA%\BD2InfiniteGacha\`:

| File | Purpose |
| --- | --- |
| `settings.json` | Targets and rules per account/pool |
| `language.json` | Interface language |
| `desktop-events.jsonl` / `runtime-events.jsonl` | Recovery, stop and error events; size-limited automatically |
| `snapshot.json` | Latest inventory, result and animation state |
| `connection.json` / `compatibility.json` / `runtime.json` | Connection diagnostics |

When reporting an issue, include the tool version, visible error and relevant event-log lines. Do not publish complete inventories, account files or personal data.

## Build and contribute

Requires Windows x64 and the .NET 8 SDK. Normal builds and regression tests **do not require the game**:

```powershell
.\build.ps1 -Locked
.\package.ps1 -Locked
```

Assets are written to `dist/v0.2.2/`. See [development and compatibility](docs/DEVELOPMENT.md), [translation maintenance](docs/LOCALIZATION.md), and [release notes](docs/RELEASE_NOTES.md).

Client interfaces are resolved locally when connecting. No game assemblies, game data tables, captured accounts or replays are distributed. Interface changes that cannot be matched reliably produce a compatibility error.

Project code is [MIT licensed](LICENSE). Dependencies retain their [third-party licenses](THIRD_PARTY_NOTICES.md). This is an independent community tool, not affiliated with the game's publisher.

# Localization / 翻译维护

Both builds contain the same `zh-CN` and `en-US` catalogs. First launch follows the system language (Chinese systems use Chinese; others use English), then the saved `language.json` preference. Language is independent of account rules and the running command.

The WPF adapter keeps original UI messages and translates at presentation time. Bound costume levels, stop-rule labels and result prefixes use the same catalog; game-provided character/costume/pool names remain in the game's language. Source messages are keys; both catalogs must have identical keys. Full messages take precedence over dynamic message fragments.

Add new user-visible source text to both JSON files. `tests/LocalizationTests.cs` checks source/XAML coverage, nonempty translations, culture defaults and preference isolation. GUI checks switch languages during a fake active run and verify the request owner is unchanged. Inspect both normal and minimum window sizes when translations get longer.

中英文使用同一份程序。词条按源文案维护，新增用户可见文本必须补齐两个JSON；动态片段也需纳入。语言只影响展示，不改变运行指令、计数或分组。每次修改需检查最小窗口和英文长文本，防止裁切。

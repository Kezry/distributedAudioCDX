# Shared

`shared` 目录用于保存跨模块共享的协议、模型和测试资源。

## 计划内容

```text
shared/
├── protocol/             # 协议描述、schema、二进制头定义
├── models/               # 设备、声道、状态等共享模型
├── test-vectors/         # 测试音频、协议样例、同步测试数据
└── README.md
```

## 初始共享概念

- `device_id`: 固定设备识别码。
- `alias`: 用户可修改别名。
- `active_mode`: 当前运行模式。
- `assigned_channels`: 当前设备承担的声道。
- `latency_profile`: 延迟策略。
- `manual_delay_ms`: 手动延迟补偿。
- `stream_id`: 当前音频会话 ID。
- `sequence`: 音频包序号。
- `play_at_timestamp_us`: 目标播放时间。

后续可以将协议定义固化为 JSON Schema、Protobuf 或 FlatBuffers。


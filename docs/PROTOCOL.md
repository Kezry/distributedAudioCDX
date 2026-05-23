# 协议草案

## 1. 协议分层

| 类型 | 建议协议 | 用途 |
|---|---|---|
| 设备发现 | mDNS、SSDP、UDP Broadcast | 查找局域网声音端 |
| 控制信令 | TCP 或 WebSocket | 配置模式、别名、声道、状态 |
| 音频数据 | UDP 或 RTP | 低延迟音频传输 |
| 时间同步 | 自定义 NTP-like 协议 | 多设备同步播放 |
| DLNA | UPnP/DLNA | 标准兼容播放 |

## 2. 设备发现消息

Android 声音端启动后广播自己的能力：

```json
{
  "type": "dacdx.announce",
  "protocol_version": 1,
  "device_id": "A4F9-21C8-77B2",
  "alias": "Living Room Left",
  "ip": "192.168.1.42",
  "control_port": 39000,
  "audio_port": 39001,
  "modes": ["sound_card", "dlna"],
  "active_mode": "sound_card",
  "sample_rates": [44100, 48000],
  "formats": ["pcm_s16le"],
  "channels": ["left", "right", "mono"],
  "version": "0.1.0"
}
```

## 3. 设备状态消息

```json
{
  "type": "dacdx.status",
  "device_id": "A4F9-21C8-77B2",
  "alias": "Living Room Left",
  "active_mode": "sound_card",
  "enabled": true,
  "assigned_channels": ["left"],
  "latency_ms": 48,
  "jitter_ms": 6,
  "packet_loss": 0.002,
  "buffer_ms": 35,
  "clock_offset_us": -1200
}
```

## 4. 配置命令

```json
{
  "type": "dacdx.configure",
  "request_id": "req-001",
  "alias": "Desk Right",
  "active_mode": "sound_card",
  "assigned_channels": ["right"],
  "latency_profile": "balanced",
  "manual_delay_ms": 0
}
```

响应：

```json
{
  "type": "dacdx.configure.result",
  "request_id": "req-001",
  "ok": true
}
```

## 5. 音频包头草案

MVP 阶段可以先用简单二进制头 + PCM payload。

```text
0               7 8             15 16                            31
+----------------+----------------+--------------------------------+
| magic          | version        | header_size                    |
+----------------+----------------+--------------------------------+
| stream_id                                                       |
+-----------------------------------------------------------------+
| sequence                                                        |
+-----------------------------------------------------------------+
| sample_rate                                                     |
+-----------------------------------------------------------------+
| channel_layout                                                  |
+-----------------------------------------------------------------+
| frame_count                                                     |
+-----------------------------------------------------------------+
| capture_timestamp_us                                            |
+-----------------------------------------------------------------+
| play_at_timestamp_us                                            |
+-----------------------------------------------------------------+
| payload ...                                                     |
+-----------------------------------------------------------------+
```

建议字段：

- `magic`: 固定协议标识。
- `version`: 协议版本。
- `stream_id`: 当前音频会话 ID。
- `sequence`: 音频包序号。
- `sample_rate`: 采样率。
- `channel_layout`: 输入声道布局，例如 stereo、5.1、7.1。
- `frame_count`: 当前包包含的采样帧数。
- `capture_timestamp_us`: Windows 捕获时间。
- `play_at_timestamp_us`: Android 目标播放时间。
- `payload`: PCM 或 Opus 数据。

## 6. 推荐 MVP 音频参数

- 采样率：48kHz。
- 位深：16-bit。
- 格式：PCM little-endian。
- 声道：stereo。
- 包时长：5ms 或 10ms。
- jitter buffer：20ms - 80ms 动态调整。

## 7. 声道枚举

```text
front_left
front_right
front_center
low_frequency
rear_left
rear_right
side_left
side_right
```


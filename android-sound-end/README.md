# Android 声音端

Android 声音端运行在 Android 4.4+ 设备上，负责接收 Windows 发送端或 DLNA 控制器的音频并播放。

## 核心职责

- 生成并保存固定设备识别码。
- 广播设备发现信息。
- 提供控制接口，允许 Windows 发送端或 Android 配置 App 修改配置。
- 接收 UDP/RTP 音频流。
- 使用 AudioTrack 低延迟播放。
- 支持声道选择和音量校准。
- 支持 DLNA Renderer 模式。
- 上报延迟、缓冲、丢包和播放状态。

## 模块拆分

```text
android-sound-end/
├── app/
├── build.gradle
├── settings.gradle
└── README.md
```

建议内部服务：

- `DeviceIdentityService`
- `DiscoveryService`
- `ControlServer`
- `AudioReceiver`
- `SyncClock`
- `JitterBuffer`
- `AudioPlayer`
- `DlnaRenderer`

## 当前实现

- 首次启动生成固定 `device_id` 并保存。
- UDP `39002` 响应 `dacdx.probe`。
- TCP `39000` 支持 `dacdx.configure` 和状态查询。
- UDP `39001` 接收 PCM 16-bit 单声道音频包并使用 `AudioTrack.MODE_STREAM` 播放。
- 自研 DLNA 最小 Renderer 支持 SSDP M-SEARCH 响应和 `device.xml`。

## MVP 优先级

1. 固定设备 ID。
2. UDP 设备发现。
3. UDP PCM 接收。
4. AudioTrack 播放。
5. 左/右声道提取。
6. 基础状态上报。

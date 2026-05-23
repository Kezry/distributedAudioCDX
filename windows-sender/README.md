# Windows 发送端

Windows 发送端负责作为系统音频源、设备管理端和音频分发中心。

## 核心职责

- 扫描局域网 Android 声音端。
- 扫描 DLNA Renderer。
- 展示设备信号状态、识别码、别名、模式和声道配置。
- 允许勾选启用设备。
- 配置基础左右声道和高级多声道布局。
- 接收系统音频。
- 对音频进行声道拆分、混音、路由。
- 将音频低延迟分发到多个声音端。
- 维护多设备同步主时钟。

## 模块拆分

```text
windows-sender/
├── src/Dacdx.Windows.App/      # WPF desktop UI
├── src/Dacdx.Windows.Service/  # Background sender service
├── src/Dacdx.Protocol/         # Shared protocol implementation
├── tests/Dacdx.Windows.Tests/  # Protocol and routing tests
├── driver/                    # WDK driver build validation
├── README.md
└── DistributedAudioCDX.sln
```

## 技术路线

原型阶段：

- 使用 WASAPI Loopback 或测试音频源。
- 实现 UDP PCM 推流。
- 先验证扫描、声道映射、延迟和同步。

正式阶段：

- 实现或集成 Windows 虚拟声卡驱动。
- 用户态服务从虚拟声卡接收系统音频。
- 支持 2.0、2.1、5.1、7.1。
- 完成驱动签名和安装器。

## 当前实现

- WPF 主界面支持局域网扫描、设备列表、启用勾选、声道配置和测试音发送。
- 共享协议库实现设备发现 JSON、控制请求 JSON、音频包头、声道矩阵、时钟偏移估算和 jitter buffer。
- 后台 service 周期扫描声音端。
- `driver/` 包含首版 WDK driver build validation 骨架；它不是正式 SysVAD 音频 miniport，也不是可安装的正式签名驱动。

## MVP 优先级

1. 设备扫描。
2. 设备列表 UI。
3. 左右声道配置。
4. UDP PCM 发送。
5. 基础延迟统计。
6. 多设备同步原型。

# distributedAudioCDX

`distributedAudioCDX` 是一个面向局域网分布式音频播放的项目，目标是让 Windows 电脑可以把系统音频作为虚拟声卡输出，并分发到多个 Android 声音端或 DLNA 渲染端。

当前仓库采用同一个根目录承载三个主要模块：

```text
distributedAudioCDX/
├── android-sound-end/      # Android 4.4+ 声音端
├── windows-sender/         # Windows 发送端与虚拟声卡集成
├── android-config-app/     # Android 配置 App
├── shared/                 # 共享协议、数据结构、测试向量
└── docs/                   # 架构、计划、协议文档
```

## 模块目标

### Android 声音端

- 接收 Windows 发送端的低延迟局域网音频。
- 支持固定设备识别码和可修改别名。
- 支持电脑声卡模式、DLNA 模式。
- 支持多设备声道分配，例如左声道、右声道、中置、低音、环绕声道。
- 重点优化 Android 4.4 设备上的延迟、稳定性和音质。

### Windows 发送端

- 扫描局域网内的 Android 声音端和 DLNA 设备。
- 显示设备信号状态、识别码、别名、模式、声道配置。
- 支持勾选启用设备。
- 支持基础左右声道配置。
- 高级模式支持 2.1、5.1、7.1 和自定义声道矩阵。
- 正式版本需要作为系统音频输出设备使用，即集成虚拟声卡驱动。

### Android 配置 App

- 扫描局域网内的声音端。
- 配置声音端运行模式：电脑声卡模式、DLNA 单机模式、DLNA 多机模式。
- 配置多机模式下的声道角色和延迟补偿。
- 管理设备别名、固定识别码展示和诊断状态。

## 当前阶段

当前阶段已经包含首版可编译工程骨架和核心通信实现：

1. Android 声音端：固定设备 ID、UDP 发现、TCP 控制、UDP PCM 接收、jitter buffer、AudioTrack 播放、自研 DLNA 最小 Renderer。
2. Windows 发送端：WPF 设备列表、扫描、声道配置、测试音发送、后台服务、共享协议库、协议测试。
3. Android 配置 App：扫描声音端、展示设备、修改别名、切换模式、配置声道和延迟。
4. CI：GitHub Actions 自动编译 Android APK、Windows app/service、协议测试和 WDK 驱动骨架。

详细计划见 [docs/PLAN.md](docs/PLAN.md)。

## 构建

GitHub Actions 会在 push 和 pull request 时自动构建：

- `android-sound-end`: debug APK。
- `android-config-app`: debug APK。
- `windows-sender`: WPF app、后台 service、协议测试。
- `windows-sender/driver`: WDK driver build validation。

本地构建要求：

- Android: JDK 17、Android SDK 35、Gradle 8.10+。
- Windows: .NET SDK 8、Visual Studio 2022、Windows Driver Kit。

## License

本项目使用 `CC BY-NC 4.0`，禁止商用。注意：这是常见的非商用公共许可证，但不是传统软件开源许可证。

# UBCO Courtyard VR Project

<div align="center">

![Unity](https://img.shields.io/badge/Unity-2022.3+-black.svg?style=flat-square&logo=unity)
![XR Toolkit](https://img.shields.io/badge/XR%20Interaction%20Toolkit-3.0+-blue.svg?style=flat-square)
![Quest 3](https://img.shields.io/badge/Meta%20Quest%203-Compatible-00C2CB.svg?style=flat-square)
![URP](https://img.shields.io/badge/Render%20Pipeline-URP-orange.svg?style=flat-square)
![License](https://img.shields.io/badge/License-MIT-green.svg?style=flat-square)

**一个面向 Meta Quest 3 的交互式虚拟现实项目**

[功能特性](#-功能特性) • [快速开始](#-快速开始) • [使用说明](#-使用说明) • [脚本文档](#-脚本文档) • [开发者指南](#-开发者指南)

</div>

---

## 📖 项目简介

UBCO Courtyard VR 是一个基于 Unity 的虚拟现实项目，专为 Meta Quest 3 头显设计。项目实现了完整的 VR 交互系统，包括菜单界面、物体生成、抓取互动等功能，提供沉浸式的虚拟环境体验。

## ✨ 功能特性

### 🎮 VR 交互系统
- **手柄射线交互** - 使用 XR Interaction Toolkit 实现的射线指向和抓取系统
- **菜单控制** - B 键快速切换空间菜单显示/隐藏
- **物理抓取** - 完整的 VR 抓取功能，支持投掷和重力模拟
- **触觉反馈** - 抓取和释放物体时的手柄震动反馈

### 🍔 食物生成系统
- **多样化选择** - Dropdown 菜单选择 Burger、Pizza、Cola
- **动态生成** - 点击 Order 按钮实时生成食物 Prefab
- **缩放控制** - Slider 滑块实时调整所有已生成物体的大小
- **特效系统** - Boom Toggle 触发粒子特效和物体显示/隐藏

### 🛠️ 开发工具
- **Prefab 配置工具** - 一键批量设置食物 Prefab 的 VR 交互组件
- **编辑器集成** - 右键菜单快速配置场景对象
- **自动化设置** - 运行时自动添加必要的交互组件

## 🚀 快速开始

### 环境要求

- **Unity 版本**: 2022.3 LTS 或更高
- **渲染管线**: Universal Render Pipeline (URP)
- **XR Plugin**: XR Interaction Toolkit 3.0+
- **目标平台**: Meta Quest 3 / Quest 2
- **开发系统**: Windows 10/11

### 安装步骤

1. **克隆或下载项目**
   ```bash
   git clone https://github.com/yourusername/ubco-courtyard-vr.git
   ```

2. **在 Unity Hub 中打开项目**
   - 打开 Unity Hub
   - 点击 "Add" → 选择项目文件夹
   - 使用 Unity 2022.3 LTS 或更高版本打开

3. **安装必需的包**
   
   项目需要以下 Unity Packages：
   - XR Interaction Toolkit (3.0.0 或更高)
   - XR Plugin Management
   - OpenXR Plugin
   - Universal RP

   在 `Window` → `Package Manager` 中确认已安装

4. **配置 XR 设置**
   - `Edit` → `Project Settings` → `XR Plug-in Management`
   - 启用 **OpenXR**
   - 在 OpenXR 设置中添加 **Meta Quest** 支持

5. **启用深度纹理** (可选，用于深度淡化效果)
   - 找到项目中的 URP Asset
   - 勾选 **Depth Texture** 选项

## 🎯 使用说明

### 场景设置

1. 打开场景：`Assets/UBCO Courtyard Assets/Courtyard Setup.unity`
2. 确保场景中有 **XR Origin** 和 **XR Interaction Manager**
3. 运行场景或部署到 Quest 3 设备

### Quest 3 控制器映射

| 按键 | 功能 |
|------|------|
| **B 按钮** (右手) | 切换空间菜单显示/隐藏 |
| **Grip 按钮** | 抓取/释放物体 |
| **Trigger** | UI 交互和选择 |
| **摇杆** | 移动和导航 |

### 使用流程

1. **打开菜单**
   - 按下右手柄的 B 按钮显示 Spatial Panel 菜单

2. **选择食物**
   - 使用射线指向 Dropdown 菜单
   - 按 Trigger 展开选项
   - 选择 Burger、Pizza 或 Cola

3. **生成食物**
   - 指向 "Order" 按钮并点击
   - 食物会在指定位置生成

4. **调整大小**
   - 拖动 Scale 滑块
   - 所有已生成的食物会同步缩放

5. **抓取互动**
   - 用射线指向食物（高亮显示）
   - 按住 Grip 按钮抓取
   - 松开释放或投掷

6. **特效演示**
   - 切换 Boom Toggle
   - **激活**：播放爆炸特效，食物消失
   - **关闭**：播放另一个特效，食物重现

## 📁 项目结构

```
UBCO Courtyard Assets/
├── Materials/              # 材质资源
├── Models/                 # 3D 模型
├── Prefab/                 # 预制体文件
├── Script/                 # C# 脚本
│   ├── TriggerMenu.cs                 # 菜单控制
│   ├── FoodMenuController.cs          # 食物系统控制器
│   ├── FoodGrabbable.cs               # VR 抓取组件
│   └── Editor/
│       └── FoodPrefabSetup.cs         # 编辑器工具
├── Textures/               # 纹理资源
├── Courtyard Setup.unity   # 主场景
└── README.md              # 本文件
```

## 📚 脚本文档

### TriggerMenu.cs

控制空间菜单的显示和隐藏。

**主要功能**：
- 监听 B 按钮输入
- 切换 Spatial Panel 的激活状态
- 提供显示/隐藏的公共方法

**配置参数**：
- `spatialPanel` - 要控制的空间面板 GameObject
- `bButtonAction` - B 按钮的输入动作引用

### FoodMenuController.cs

管理食物生成系统的核心控制器。

**主要功能**：
- Dropdown 菜单管理（Burger/Pizza/Cola）
- Order 按钮事件处理
- Scale 滑块实时缩放
- Boom Toggle 特效触发
- 自动配置生成物体的 VR 交互

**配置参数**：
| 参数 | 类型 | 说明 |
|------|------|------|
| `foodDropdown` | TMP_Dropdown | 食物选择下拉菜单 |
| `orderButton` | Button | 生成食物按钮 |
| `scaleSlider` | Slider | 大小调节滑块 |
| `boomToggle` | Toggle | 爆炸特效开关 |
| `burgerPrefab` | GameObject | 汉堡预制体 |
| `pizzaPrefab` | GameObject | 披萨预制体 |
| `colaPrefab` | GameObject | 可乐预制体 |
| `spawnPoint` | Transform | 生成位置 |
| `boomActivateParticle` | GameObject | 激活粒子特效 |
| `boomDeactivateParticle` | GameObject | 关闭粒子特效 |

### FoodGrabbable.cs

为物体添加 VR 抓取功能。

**主要功能**：
- 自动配置 Rigidbody 和 Collider
- 管理 XRGrabInteractable 组件
- 抓取/释放事件处理
- 手柄触觉反馈
- 保持和更新缩放比例

**可调参数**：
- `maintainScale` - 释放后保持原始大小
- `throwOnRelease` - 启用投掷功能
- `throwVelocityScale` - 投掷速度倍数

### FoodPrefabSetup.cs (编辑器工具)

批量配置 Prefab 的 VR 交互组件。

**使用方法**：
1. 在 Project 窗口选中 Prefab
2. `Tools` → `Setup Food Prefabs for VR Grab`
3. 点击相应按钮进行配置

或右键菜单：`GameObject` → `Setup Food for VR Grab`

## 🔧 开发者指南

### 添加新的食物类型

1. **准备 Prefab**
   ```
   - 确保有 MeshRenderer/MeshFilter
   - 添加合适的 Collider (建议 MeshCollider Convex)
   - 调整好默认大小和旋转
   ```

2. **配置 VR 交互**
   - 右键 Prefab → `Setup Food for VR Grab`
   - 或使用编辑器工具批量配置

3. **添加到菜单**
   - 在 `FoodMenuController` 中添加新的 `public GameObject` 字段
   - 在 `SetupDropdown()` 方法中添加选项
   - 在 `OnOrderButtonClicked()` 的 switch 中添加 case

### 自定义粒子特效

在 `FoodMenuController` 中配置：
- `boomActivateParticle` - 食物消失时的特效
- `boomDeactivateParticle` - 食物出现时的特效

特效会自动播放并在结束后销毁。

### 调整抓取行为

修改 `FoodGrabbable.cs` 中的参数：
```csharp
// 抓取移动类型
movementType = XRBaseInteractable.MovementType.Instantaneous;

// 投掷设置
throwOnDetach = true;
throwVelocityScale = 1.5f;

// 触觉反馈
SendHapticFeedback(interactor, amplitude: 0.3f, duration: 0.1f);
```

### 扩展菜单功能

在 `FoodMenuController` 中可以添加：
- 更多 UI 控件
- 额外的粒子特效
- 物体销毁功能
- 颜色/材质切换

示例方法已提供：
- `ClearAllFood()` - 清除所有生成的物体

## 🐛 故障排除

### 问题：物体无法抓取
**解决方案**：
- 检查物体是否有 Collider 和 XRGrabInteractable 组件
- 确保 Collider 未被禁用
- 验证 Layer 设置，确保射线可以检测到

### 问题：射线不可见
**解决方案**：
- 检查手柄控制器上是否有 Line Renderer 组件
- 确保 XR Ray Interactor 的 Line Type 设置正确
- 检查 Line Renderer 的材质和颜色

### 问题：菜单不响应 B 按钮
**解决方案**：
- 检查 TriggerMenu 的 `bButtonAction` 是否正确配置
- 确保选择了 `XRI RightHand` → `Primary Button`
- 验证脚本已添加到场景中的 GameObject

### 问题：材质显示为品红色
**解决方案**：
- 使用提供的 URP Shader 版本
- 或在 `Edit` → `Render Pipeline` → `Universal Render Pipeline` → `Upgrade Project Materials to URP Materials`

## 🤝 贡献

欢迎贡献代码、报告问题或提出建议！

1. Fork 本项目
2. 创建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 开启 Pull Request

## 📝 更新日志

### v1.0.0 (2025-10-19)
- ✅ 初始版本发布
- ✅ 实现 B 按钮菜单控制
- ✅ 完成食物生成系统
- ✅ 添加 VR 抓取功能
- ✅ 集成粒子特效系统
- ✅ 创建编辑器配置工具

## 📄 许可证

本项目采用 MIT 许可证 - 详见 [LICENSE](LICENSE) 文件

## 📧 联系方式

项目链接: [https://github.com/yourusername/ubco-courtyard-vr](https://github.com/yourusername/ubco-courtyard-vr)

---

<div align="center">

**感谢使用 UBCO Courtyard VR！**

如果这个项目对你有帮助，请给我们一个 ⭐️

Made with ❤️ for VR Development

</div>


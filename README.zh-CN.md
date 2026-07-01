# Unity Inventory System

一个模块化的 Unity 背包系统，附带可直接运行的示例场景。

English Version: [README.md](README.md)

## 功能特点

- 格子背包，支持不同尺寸物品占格
- 支持物品旋转摆放
- 支持同类物品堆叠
- 支持背包格子解锁
- 支持快捷栏绑定、切换、滚轮选择
- 支持基础负重统计和 UI 状态反馈
- 主要配置通过 `ScriptableObject` 管理
- 运行时入口已拆分为多组聚焦接口，便于 UI 和外部系统接入
- 示例场景已接入 Unity Input System

## 当前已实现

### Runtime

- `InventoryGrid` 负责背包格子判定、占用、自动找位、移动、堆叠、合并、拆分
- `PlayerInventory` 负责玩家背包、快捷栏、负重和玩家侧操作封装
- `InventoryManager` 作为默认运行时门面，并对外暴露多组聚焦接口
- `InventoryEventCentre` 负责库存变化、快捷栏变化等事件派发
- `InventorySystemBootstrap` 负责创建运行时管理器并注册静态入口

### 运行时接口

- `IInventoryRuntime`：通用库存运行时入口
- `IInventoryEventSource`：事件注册与派发
- `IBackpackReadOnly`：只读背包状态
- `IQuickBarReadOnly`：只读快捷栏状态
- `IBackpackViewRuntime`：背包 UI 放置与绑定相关运行时接口
- `IBackpackCommandRuntime`：丢弃、拆分、合并等高级命令接口

### UI

- 背包面板
- 快捷栏面板
- 拖拽物品移动
- 放置预览
- 拖拽时旋转物品
- 将物品拖到快捷栏进行绑定
- 将物品拖出背包区域进行丢弃
- 拖到兼容堆叠上进行合并
- 右键拖出半组物品作为拆分交互

### Sample

- 示例场景：`Assets/Scenes/SampleScene.unity`
- 示例输入封装：`Assets/Scripts/Sample/SampleInventoryInput.cs`
- 输入资源文件：`Assets/SampleInventoryInputAction.inputactions`
- 左上角有一个简易调试 GUI，可以直接输入物品 ID 和数量测试添加逻辑

可用的示例物品 ID：

- `001` BlueGem
- `002` Health
- `003` Monitor
- `004` RAM

## 使用方式

### 环境

- Unity `6000.3.9f1`
- URP
- Input System

### 快速开始

1. 用 Unity 打开项目。
2. 打开 `Assets/Scenes/SampleScene.unity`。
3. 进入 Play Mode。
4. 在左上角调试框输入物品 ID 和数量，点击 `Add`。

### 默认操作

- `Tab`：打开 / 关闭背包
- `R`：旋转当前拖拽物品
- `1` 到 `0`：切换快捷栏槽位
- 鼠标滚轮：前后切换快捷栏选中项
- 左键拖拽：拖动整组物品
- 右键拖拽：拖动半组物品
- 拖出背包区域：丢弃物品
- 拖到兼容堆叠上：合并物品
- 拖到快捷栏槽位上：绑定到快捷栏

## 目录结构

```text
Assets
├── ArtRes                   # 物品和 UI 美术资源
├── Configs                  # 物品、背包、快捷栏、视图配置
├── Prefabs/UI               # 背包和快捷栏相关预制体
├── Scenes                   # 示例场景
├── SampleInventoryInputAction.inputactions
└── Scripts
    ├── Runtime
    │   ├── Core             # 系统入口、Bootstrap、事件、接口、共享配置
    │   ├── Data             # ItemDefinition / ItemDatabase / ItemInstance
    │   ├── Inventory
    │   │   ├── Backpack     # 背包数据与背包 UI
    │   │   ├── Internal     # 内部网格与管理器实现
    │   │   ├── Player       # 玩家背包封装
    │   │   └── QuickBar     # 快捷栏数据与 UI
    │   └── UI               # 通用面板基类
    └── Sample               # 示例输入和场景控制脚本
```

## 设计说明

当前结构围绕一个轻量运行时核心和若干功能层展开：

- `ItemDefinition` 描述静态物品数据
- `ItemInstance` 表示运行时物品实例
- `PlacedItem` 记录物品在格子中的位置、尺寸和旋转状态
- `InventoryGrid` 负责底层格子规则和放置判定
- `PlayerInventory` 负责玩家侧库存行为封装
- `InventoryManager` 负责给 UI 和外部系统提供更干净的运行时门面
- `InventorySystemBootstrap` 负责在场景中完成运行时装配

UI 层当前主要依赖聚焦后的运行时接口，而不是直接依赖所有具体实现细节，这样后续更容易替换接入方式。

## UML 类图

![Inventory System UML](inventory-system-uml.svg)

## 后续计划

- 物品实例工厂或生成服务
- 存档与读档
- 更细的事件类型
- 在当前运行时 API 之上扩展更多面向玩法的物品操作

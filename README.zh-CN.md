# Unity Inventory System

一个Unity 背包系统项目

当前版本是一个可玩的初版，已经带了示例场景、基础输入和一套 UI 流程。

## 项目特点

- 格子背包，支持不同尺寸物品占格
- 支持物品旋转摆放
- 支持同类物品堆叠
- 支持背包格子解锁
- 支持快捷栏绑定、切换、滚轮选择
- 支持基础负重统计和 UI 状态反馈
- 主要配置通过 `ScriptableObject` 管理
- UI 和运行时数据有基本分层，后续比较容易继续拆

## 当前已实现

### Runtime

- `InventoryGrid` 负责背包格子判定、占用、移动、自动找位、堆叠
- `PlayerInventory` 负责玩家背包、快捷栏、负重和对外操作
- `InventoryManager` 作为统一入口，给 UI 和外部系统调用
- `InventoryEventCentre` 负责库存变化、快捷栏变化等事件派发

### UI

- 背包面板
- 快捷栏面板
- 拖拽物品
- 放置预览
- 旋转拖拽物品
- 物品拖到快捷栏进行绑定

### Sample

- 示例场景：`Assets/Scenes/SampleScene.unity`
- 左上角有一个简易调试 GUI，可以直接输入物品 ID 和数量测试添加逻辑

可用的示例物品 ID：

- `001` BlueGem
- `002` Health
- `003` Monitor
- `004` RAM

## 运行方式

### 环境

- Unity `6000.3.9f1`
- URP
- Input System

### 快速开始

1. 用 Unity 打开项目
2. 打开 `Assets/Scenes/SampleScene.unity`
3. 进入 Play Mode
4. 在左上角调试框输入物品 ID 和数量，点击 `Add`

### 默认操作

- `Tab`：打开 / 关闭背包
- `R`：旋转当前拖拽物品
- `1` 到 `0`：切换快捷栏槽位
- 鼠标滚轮：前后切换快捷栏选中项
- 鼠标拖拽：移动物品 / 绑定到快捷栏

## 目录结构

```text
Assets
├── Configs                  # 物品、背包、快捷栏、视图配置
├── Prefabs/UI               # 背包和快捷栏相关预制体
├── Scenes                   # 示例场景
└── Scripts
    ├── Runtime
    │   ├── Core             # 系统入口、事件、全局配置
    │   ├── Data             # ItemDefinition / ItemDatabase / ItemInstance
    │   ├── Inventory
    │   │   ├── Backpack     # 背包数据与背包 UI
    │   │   ├── Common       # 通用网格与管理逻辑
    │   │   ├── Player       # 玩家背包封装
    │   │   └── QuickBar     # 快捷栏数据与 UI
    │   └── UI               # 通用面板基类
    └── Sample               # 输入和示例控制脚本
```

## 设计思路

目前的思路比较直接：

- 用 `ItemDefinition` 描述静态物品数据
- 用 `ItemInstance` 表示运行时实例
- 用 `PlacedItem` 记录实例在背包里的位置、尺寸和旋转状态
- 用 `InventoryGrid` 处理“能不能放”“放在哪”“会不会重叠”这类核心判定
- 用 `PlayerInventory` 和 `InventoryManager` 对外提供统一接口，避免 UI 直接碰底层细节

## UML 类图

暂未完成, 会在结构更清晰时补充.

## 后续计划

- 将`InventoryManager`中的一些必要函数抽象为一个接口
- 实现一个用于生成物品实例(`ItemInstance`)的类
- 丢弃 / 拆分 / 合并
- 存档与序列化
- 更细的事件类型


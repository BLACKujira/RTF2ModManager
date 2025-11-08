# R-Type Final 2 Mod 管理器

本程序旨在简化 *R-Type Final 2* 的 *UE4SS* 依赖安装、Mod 文件的部署、管理和卸载流程，为玩家提供开箱即用的 Mod 体验。

![Screenshot_zhs](Screenshot_zhs.png)

## 使用指南

> 更详细的教程请查看 [RTF2ModdingGuide](https://github.com/BLACKujira/RTF2ModdingGuide) 中的 [使用Mod管理器](https://github.com/BLACKujira/RTF2ModdingGuide/blob/master/Chapter1_TheBasics/zhs/%E4%BD%BF%E7%94%A8Mod%E7%AE%A1%E7%90%86%E5%99%A8.md)

### 安装与启动

1. 从 [Releases](https://github.com/BLACKujira/GTypeOrigin/releases) 中下载 `RTF2ModManager.zip`
2. 将压缩包中的内容解压到任意目录 (请不要直接在压缩包内双击运行程序，可能会导致程序找不到Mod文件而出错)
3. 运行 `RTF2ModManager.exe`
4. 如果使用 *Steam* 并未修改默认安装路径，程序会自动定位游戏目录；否则：
   - 点击 `游戏目录设置` → `手动选择`
   - 在弹出的窗口中选择游戏安装目录

### UE4SS 管理

1. 如果未安装 *UE4SS* 或发现问题：
   - 点击 `安装` 或 `修复`
- **注意**：首次安装 *UE4SS* 后，请先**启动一次游戏**生成必要的文件和文件夹
2. 如果 *UE4SS* 无法随游戏启动（已知部分Windows7用户出现过这种情况），请尝试 `静态注入`

### Mod 管理

1. 在左侧列表选择要操作的 Mod
2. 在右侧查看 Mod 信息
3. 点击安装或卸载

### 快捷功能

程序的下方提供了三个快捷功能按钮：

| 按钮 | 功能说明 |
|------|---------|
| 打开 mods.txt | 打开 LUA Mod 注册表。程序会自动维护此文件，通常无需手动修改。 |
| 打开存档文件夹 | 打开游戏存档文件夹，**建议在安装Mod前备份此文件夹内的所有文件**。 |
| 快速启动游戏 | 绕过Steam快速启动游戏，方便调试。但这种方式无法进入任何DLC关卡。 |

## 包含 Mod

| Mod | 简介 |
|------|---------|
| [G-Type Origin](https://github.com/BLACKujira/GTypeOrigin) | 添加3个Gradius风格的致敬关卡 |
| [Simple Boss LifeBar](https://github.com/BLACKujira/SimpleBossLifeBarMod) | 为Boss战添加简易的血条，不适用于BossRush |
| [RTF2 Debug Tools](https://github.com/BLACKujira/RTF2DebugToolsMod) | 用于调试关卡的作弊工具，请勿用于不正当竞争目的 |
| [FPS Player](https://github.com/BLACKujira/FPSPlayerMod)  |  将游戏玩法变为第一人称射击，需使用UE4SS的Mod按钮激活 |

Mod的详细信息、使用说明请参照各自的主页。

## 注意事项

1. 此程序在开发完成后缺少足够的测试，可能存在一些Bug。并且尚未明确在不同的系统环境下的表现
2. 由于当前R-Type Final 2的Mod较少，关于Mod的信息都是硬编码在代码里的，不太方便扩展。Mod本身也是放在程序目录里的，还没有在线更新功能。
3. 虽然程序本身并不复杂，但由于包含了 *.NET 8.0运行时* 、 *UE4SS zDEV-UE4SS_v3.0.1* 和 *G-Type Origin Mod* ，所以压缩包的体积较大，请耐心等待下载。
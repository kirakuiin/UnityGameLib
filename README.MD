# UnityGameLib

存储一些平时开发游戏会使用的Unity通用代码，以便于在不同的项目中进行复用。

## 使用方法

在包管理器通过链接`https://github.com/kirakuiin/UnityGameLib.git?path=/Assets`来加入该包。

## 功能结构

目前分为四个主要模块：

1. **GameLib.Audio**
   * 音效混合器的管理。
   * 简单的音源播放器。
2. **GameLib.Common**
   * 常见的特化`MonoBehaviour`(比如：定时销毁，不会随场景销毁等)
   * 数据结构
     * 默认字典
     * 计数器
   * 常用类型扩展(比如`IList`支持`Swap()`)
   * 游戏模式
     * 单例
     * 定位器
   * 通用功能(比如对象池，序列化，GUID等)
3. **GameLib.Network**
   * 对[Netcode for GameObjects](https://docs-multiplayer.unity3d.com/netcode/1.9.1/about/)网络框架内部功能的封装。
     * 连接管理。
     * 会话管理。
     * 广播信道。
     * 网络对象池。
     * 网络场景管理。
     * 同步管理。
   * 通用网络功能
     * 网络延迟检查
     * 局域网广播
4. **GameLib.UI**
   * 扇形布局 (常用于卡牌游戏的手牌模拟)。
   * 多个布局大小自适应组件。
   * 可拖拽对象组件。
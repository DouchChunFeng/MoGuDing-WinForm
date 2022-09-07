<p align="center"><img src="https://raw.githubusercontent.com/DouchChunFeng/MoGuDing-WinForm/main/form.png" width="400" alt="图片预览"></p>

# 蘑菇丁/可视化自动打卡窗口
- [功能](#gn)
- [特性](#tx)
- [用法](#yf)
- [依赖](#yl)
- [参考](#ck)
- [常见问题](#cjwt)
- [注意事项](#zysx)

<a name="gn"></a>
## 功能

- 自定义上班时间
- 自定义下班时间
- 保存登录信息无需重复登录

<a name="tx"></a>
## 特性

- 简洁窗口
- 快速动态添加用户
- 打卡分钟数可随机，每日0点刷新
- 可隐藏到状态栏 不影响系统正常使用
- 兼容WinXP及以上系统(需安装.Net FrameWork4.0或以上)

<a name="yf"></a>
## 用法

1. 打开项目内文件夹: `gxy\gxy\bin\Debug\`
2. 复制 `gxy.exe` 和 `Newtonsoft.Json.dll` 到其他位置运行或直接运行
3. 点击添加项按钮添加用户
4. 按顺序输入您的账户信息(输入完详细地址后可以点击获取经纬度信息)
5. 选项设置时间，确认无误后开始即可

<a name="yl"></a>
## 依赖

* [.Net FrameWork 4.0](https://referencesource.microsoft.com)
* [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
* 本程序使用VS2013编译

<a name="ck"></a>
## 参考与致谢

* [咕咚咕哒](https://www.bilibili.com/video/BV1RS4y1d7t2)
* [laradocs/moguding-solution](https://github.com/laradocs/moguding-solution)

<a name="cjwt"></a>
## 常见问题

1. `打开无反应`
- 检查exe文件目录有无[Newtonsoft.Json.dll](https://github.com/JamesNK/Newtonsoft.Json/releases?page=4)文件, 若没有请复制补全即可.

2. `.Net FrameWork 初始化失败`
- 安装[.Net FrameWork 4.0或其他更高的版本](https://www.microsoft.com/zh-cn/download/details.aspx?id=17718)

3. `为什么会产生三个txt文本`
- UserData.txt -> 用于保存用户信息
- UserData_Logs.txt -> 用于保存日志信息
- UserData_LoginCache.txt -> 用于保存用户登录信息

<a name="zysx"></a>
## 注意事项

1. **严禁将本项目对外销售或盈利**
2. **严禁利用本软件进行代挂号盈利或其他盈利方式**
3. **该项目仅供个人使用者参考与学习或使用**

---
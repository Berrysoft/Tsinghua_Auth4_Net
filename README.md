# 弃坑通知
本项目已经弃坑，包含的所有功能已经移植到Avalonia、CLI、UWP版本，具体请参考[TsinghuaNet](https://github.com/Berrysoft/TsinghuaNet)仓库。
# TsinghuaNet
能快速连接清华大学校园网，包括有线网与无线网。
## 多种登录方式
支持Auth4、Auth6（不建议使用）、Net方式的登录。
## 多语言支持
默认为简体中文，所有不支持的且存在的语言会以简体中文代替；不存在的语言会以用户电脑的语言或简体中文代替。
### 支持语言列表
* en 英语
* ja 日语
* ko 朝鲜语
* ru 俄语
* vi 越南语
* zh-Hans 简体中文
* zh-Hant 繁体中文
### 多语言须知
实际上我并不会这么多语言，大都是用的机器翻译。

现在已经弃疗，就这么多吧。
## 屏幕截图
|![亮主题](Screenshots/MainWindow_Light.png)|![暗主题](Screenshots/MainWindow_Dark.png)|
|:-:|:-:|
|亮主题|暗主题|

## 依赖项
使用NuGet引用了我的另一个仓库[ClassLibrary](https://github.com/Berrysoft/ClassLibrary)的Berrysoft.Console与Berrysoft.Tsinghua.Net项目。
二者均使用MIT许可证。
### 编译
不幸的是，这两个项目虽然生成了NuGet包，但是并没有发布到[nuget.org](https://www.nuget.org)。
因此想要自行编译本程序，请先从上述仓库下载发布好的nuget包或自行编译。

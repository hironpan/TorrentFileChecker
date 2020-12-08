# TorrentFileChecker / 种子文件校验工具

Check downloaded file of the **.torrent** file, calculate the MD5,SHA1,SHA256.

This program based on [BencodeNET](https://github.com/Krusen/BencodeNET).

校验种子下载下来的文件，并计算其MD5、SHA1、SHA256哈希值。

软件基于 [BencodeNET](https://github.com/Krusen/BencodeNET) 开发。

## User guide / 使用说明

- 选择种子文件和对应的下载下来的文件，点“校验”按钮开始校验。

- 待校验的文件，首尾区块可能需要种子中的其他文件配合才能完成校验，也可以选择跳过首尾区块的校验。

- 进度条蓝色代表验证无误的区块，红色代表数据有问题，黑色代表跳过校验的区块（首尾区块），灰色代表还未校验到的区块。

## Screenshots / 校验结果示例

- 校验出错

![校验出错](https://github.com/hironpan/TorrentFileChecker/raw/main/Images/err.png)

- 跳过首区块校验

![跳过首区块校验](https://github.com/hironpan/TorrentFileChecker/raw/main/Images/skip.png)

- 校验全部通过

![校验全部通过](https://github.com/hironpan/TorrentFileChecker/raw/main/Images/ok.png)
